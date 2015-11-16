using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using Accord.Audio.Formats;
using Accord.Audio.Windows;
using Accord.Audio;
using Accord.Math;
using NAudio.Wave;
using AForge.Math;
using NetSVMLight;
using LibSVMsharp.Helpers;
using LibSVMsharp.Extensions;
using LibSVMsharp;
using LibSVMsharp.Core;
using System.Diagnostics;

namespace Test
{
    public partial class AUDIOCLASSIFIER : Form
    {
        string trainpath = null;
        string testpath = null;
        double[] precision = new double[2];
        double[] recall = new double[2];

        public AUDIOCLASSIFIER()
        {
            InitializeComponent();
        }

        private NAudio.Wave.WaveFileReader wave = null;
        private NAudio.Wave.DirectSoundOut output = null;

        //Get amplitudes of the passed .wav file
        public static IEnumerable<float> GetAmplitude(FileInfo file)
        {
            using (var wave = new AudioFileReader(file.FullName))
            {
                int count = 4096; // arbitrary
                float[] buffer = new float[count];
                int offset = 0;
                int read = 0;

                while ((read = wave.Read(buffer, offset, count)) > 0)
                {
                    foreach (float amplitude in buffer.Take(read))
                    {
                        yield return amplitude;
                    }
                }
            }
        }

        //Time domain values
        public class TimeDomain
        {
            public float AvgEnergy { get; set; }
            public double ZeroCrossingRate { get; set; }
            public int SampleCount { get; set; }

            public static TimeDomain GetTimeFeatures(IList<float> samples)
            {
                int samplesRead = samples.Count;
                var avgene = samples.Average(sample => sample * sample);

                // Zero crossings
                var signs = samples.Select(s => s >= 0 ? 1 : -1).ToList();

                var totalCrossings =
                    signs.Zip(signs.Skip(1), (left, right) => Math.Abs(left - right)).Sum();

                var zeroCrossings = Math.Ceiling(totalCrossings / 2.0);

                var zcRate = (zeroCrossings / samplesRead);

                return new TimeDomain
                {
                    AvgEnergy = avgene,
                    ZeroCrossingRate = zeroCrossings,
                    SampleCount = samplesRead
                };
            }
        }

        //Frequency domain calculations
        public static FrequencyFeatures GetFrequencyFeatures(FileInfo wavFile)
        {
            var reader = new Accord.Audio.Formats.WaveDecoder(wavFile.FullName);
            Signal decoded = reader.Decode();
            int channel = 0;
            IWindow window = null;
            int windowLen = 512;
            ComplexSignal[] ComplexSignals;

            using (Signal compsig = reader.Decode())
            {
                window = window ?? RaisedCosineWindow.Hamming(windowLen);
                windowLen = window.Length;

                // Splits the source signal by taking each windowLen/2 samples, then creating 
                // a windowLen sample window.	

                Signal[] windowedSignals = compsig.Split(window, windowLen / 2);

                ComplexSignals = windowedSignals.Apply(ComplexSignal.FromSignal);
            }

            // Forward to the Fourier domain
            ComplexSignals.ForwardFourierTransform();

            //get magnitudes from complex signal
            double[] meanMags = GetAverage(ComplexSignals,
                Accord.Audio.Tools.GetMagnitudeSpectrum, channel, windowLen);

            ComplexSignal sig = ComplexSignals[0];

            //get frequencies
            double[] freqv = Accord.Audio.Tools.GetFrequencyVector(
                sig.Length, sig.SampleRate);

            FrequencyData data = new FrequencyData(freqv, meanMags);

            return FrequencyFeatures.CalcFreqFeatures(data);
        }

        public static double[] GetAverage(ComplexSignal[] ComplexSignals,
           Func<Complex[], double[]> featureFunc, int channel, int windowLen)
        {
            // Loop through all windowed chunks of signal to get average values
            double[] avgFeatureValues = { };
            int numSignals = 0;
            int len = 0;
            foreach (ComplexSignal sig in ComplexSignals)
            {
                double[] featureValues = featureFunc(sig.GetChannel(channel));
                if (len == 0)
                {
                    len = featureValues.Length;
                    avgFeatureValues = new double[len];
                }
                for (int i = 0; i < len; i++)
                {
                    avgFeatureValues[i] += featureValues[i];
                }
                numSignals++;
            }
            for (int i = 0; i < len; i++)
            {
                avgFeatureValues[i] /= numSignals;
            }
            return avgFeatureValues;
        }


        //Function to calculate frequency features spectral centroid and bandwidth
        public class FrequencyFeatures
        {
            public double SpectralCentroid { get; private set; }
            public double Bandwidth { get; private set; }
            public FrequencyData Data { get; private set; }

            public FrequencyFeatures(double spectralCentroid, double bandwidth, FrequencyData data)
            {
                SpectralCentroid = spectralCentroid;
                Bandwidth = bandwidth;
                Data = data;
            }

            public static FrequencyFeatures CalcFreqFeatures(FrequencyData data)
            {
                double[] meanMags = data.Magnitudes;
                double[] freqv = data.Frequencies;

                // spectral centroid - seems close to orig signal's "energy", at least for sine
                double spectralCentroid = meanMags.Zip(freqv, (m, f) => m * f).Sum() /
                                          meanMags.Sum();

                // bandwidth = highest - smallest freq
                double avgMagnitude = meanMags.Average();

                //use avg magnitude as a threshold to select frequencies
                var nonzeroFreqs = Enumerable.Zip(meanMags, freqv, Tuple.Create)
                    .Where(t => t.Item1 > avgMagnitude)
                    .Select(t => t.Item2).ToList();

                //calculate bandwidth using retrieved frequencies
                double bandwidth = nonzeroFreqs.Max() - nonzeroFreqs.Min();

                return new FrequencyFeatures(spectralCentroid, bandwidth, data);
            }
        }

        public class FrequencyData
        {
            public double[] Frequencies { get; private set; }
            public double[] Magnitudes { get; private set; }
            public FrequencyData(double[] frequencies, double[] magnitudes)
            {
                Frequencies = frequencies;
                Magnitudes = magnitudes;
            }
        }


        //Function to generate training and test data based on audio features
        public void generatedata()
        {
            System.IO.File.WriteAllText(trainpath + "\\Training.txt", string.Empty);//file to store training data
            System.IO.File.WriteAllText(testpath + "\\Test.txt", string.Empty);//file to store test data
            System.IO.File.WriteAllText(testpath + "\\output.txt", string.Empty);//stores filenames for final sorting

            //Pick training files and create training.txt
            FileInfo[] files = openFileDialog2.FileNames.Select(file => new FileInfo(file)).ToArray();
            WriteSvmData(files, trainpath + "\\Training.txt");

            //Pick Test files and create test.txt
            files = openFileDialog1.FileNames.Select(file => new FileInfo(file)).ToArray();
            WriteSvmData(files, testpath + "\\Test.txt");

            //store ground truth for test files to use for comparison with results later
            for (int i = 0; i < files.Length; i++)
            {
                string outputline = null;

                if (files[i].Name.Contains("sp"))
                {
                    outputline = files[i].Name + "   " + "Speech" + "  ";
                }
                else
                {
                    outputline = files[i].Name + "   " + "Music" + "  ";
                }
                // Writing filename and Ground truth into the output file
                addToFile(outputline, testpath + "\\output.txt");
            }
        }

        //Write training and test data
        public void WriteSvmData(FileInfo[] files, string outputfile)
        {
            for (int i = 0; i < files.Length; i++)
            {
                string line = null;
                var amplitudes = GetAmplitude(files[i]).ToList();

                //Get time domain features - zero crossing rate , average energy
                TimeDomain timefeatures = TimeDomain.GetTimeFeatures(amplitudes);

                //Get frequency features - Bandwidth , spectral centroid
                FrequencyFeatures freq = GetFrequencyFeatures(files[i]);

                //Store into the files in format required by libsvm label index1:value1 index2:value2 ...
                if (files[i].Name.Contains("sp"))
                {
                    line = "1" + " " + "1:" + timefeatures.ZeroCrossingRate + " " +
                        "2:" + freq.Bandwidth + " 3:" + timefeatures.AvgEnergy + " 4:" + freq.SpectralCentroid;
                }
                else
                {
                    line = "-1" + " " + "1:" + timefeatures.ZeroCrossingRate + " " +
                        "2:" + freq.Bandwidth + " 3:" + timefeatures.AvgEnergy + " 4:" + freq.SpectralCentroid;
                }
                addToFile(line, outputfile);
            }
        }

        //Data mining of training data to generate a model
        public void datamining()
        {
            int[,] confusionMatrix;
            SVMProblem trainingSet = SVMProblemHelper.Load(trainpath + "\\Training.txt");
            SVMProblem testSet = SVMProblemHelper.Load(testpath + "\\Test.txt");

            System.IO.File.WriteAllText(testpath + "\\model.txt", string.Empty);
            System.IO.File.WriteAllText(testpath + "\\finaloutput.txt", string.Empty);
            System.IO.File.WriteAllText(testpath + "\\targetoutput.txt", string.Empty);

            //  L2 Norm => x / ||x||
            trainingSet = trainingSet.Normalize(SVMNormType.L2);
            testSet = testSet.Normalize(SVMNormType.L2);

            // Select the parameter set
            SVMParameter parameter = new SVMParameter();
            parameter.Type = SVMType.C_SVC;
            parameter.Kernel = SVMKernelType.RBF;
            parameter.C = 2;

            // Do cross validation to check this parameter set is correct for the dataset or not
            //trainingSet.CrossValidation(parameter, nFold, out crossValidationResults);

            // Evaluate the cross validation result
            // If it is not good enough, select the parameter set again
            // double crossValidationAccuracy = trainingSet.EvaluateClassificationProblem(crossValidationResults);

            // Train the model, If your parameter set gives good result on cross validation
            SVMModel model = trainingSet.Train(parameter);

            // Save the model
            SVM.SaveModel(model, testpath + "\\model.txt");

            double[] target = testSet.Predict(model);

            // Evaluate the test results
            double testAccuracy = testSet.EvaluateClassificationProblem(target, model.Labels, out confusionMatrix);

            //store result labels in a text file
            foreach (var item in target)
            {
                addToFile(item.ToString(), testpath + "\\targetoutput.txt");
            }

            //calculate precision and recall for both speech and musin
            precision[0] = 100.0 * ((double)confusionMatrix[0, 0] / (double)(confusionMatrix[0, 0] + confusionMatrix[1, 0]));//speech
            precision[1] = 100.0 * ((double)confusionMatrix[1, 1] / (double)(confusionMatrix[1, 1] + confusionMatrix[0, 1]));//music

            recall[0] = 100.0 * ((double)confusionMatrix[0, 0] / (double)(confusionMatrix[0, 0] + confusionMatrix[0, 1]));//speech
            recall[1] = 100.0 * ((double)confusionMatrix[1, 1] / (double)(confusionMatrix[1, 1] + confusionMatrix[1, 0]));//music

            //store and display final classification results 
            WriteResults(testAccuracy);
        }


        //Function to display store and display final results
        public void WriteResults(double testaccuracy)
        {

            string finaloutputline = null;
            // To read each line from output file and model output file
            ArrayList selectedOutput = new ArrayList(); ;
            ArrayList modelOutput = new ArrayList(); ;

            readFile(selectedOutput, testpath + "\\output.txt");
            readFile(modelOutput, testpath + "\\targetoutput.txt");

            addToFile("Filename" + "  " + "Groundtruth" + "  " + "Model", testpath + "\\finaloutput.txt");

            for (int i = 0; i < selectedOutput.Count; i++ )
                {
                    if (modelOutput[i].Equals("1"))
                        finaloutputline = selectedOutput[i] + "     " + "Speech";
                    else if (modelOutput[i].Equals("-1"))
                        finaloutputline = selectedOutput[i] + "     " + "Music";

                    addToFile(finaloutputline, testpath + "\\finaloutput.txt");
                    finaloutputline = null;
                }

            string newline = "\r\n";

            addToFile(newline + "Precision values for Speech: " + precision[0] + "%", testpath + "\\finaloutput.txt");
            addToFile(newline + "Precision values for Music: " + precision[1] + "%", testpath + "\\finaloutput.txt");

            addToFile(newline + "Recall values for Speech: " + recall[0] + "%", testpath + "\\finaloutput.txt");
            addToFile(newline + "Recall values for Music: " + recall[1] + "%", testpath + "\\finaloutput.txt");
            addToFile(newline + "Accuracy of the classification: " + testaccuracy, testpath + "\\finaloutput.txt");

            //display final output
            Process.Start("notepad.exe", testpath + "\\finaloutput.txt");
        }

        //function to read a file in from memory and save each line as a string in the array list
        public void readFile(ArrayList fileList, string file)
        {
            StreamReader tr = new StreamReader(file);
            string line;
            while ((line = tr.ReadLine()) != null)
            {
                fileList.Add(line);
            }
            tr.Close();
        }

        //write into text file
        public void addToFile(string line, string file)
        {
            FileStream fileWriter = new FileStream(file, FileMode.Append);
            StreamWriter tw = new StreamWriter(fileWriter);
            tw.WriteLine(line);
            tw.Close();
            fileWriter.Close();
        }


        /* Design Elements Section , action definitoions of all buttons */

        //select training files buuton
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.Filter = "Wave Sound|*.wav";
            openFileDialog2.ShowDialog();
            listBox1.DataSource = openFileDialog2.FileNames;
            FileInfo newFile = new FileInfo(openFileDialog2.FileNames[0]);
            trainpath = newFile.DirectoryName;
            button6.Enabled = true;
        }

        //select test files button
        private void button3_Click_1(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Wave Sound|*.wav";
            openFileDialog1.ShowDialog();
            listBox1.DataSource = openFileDialog1.FileNames;
            button4.Enabled = true;
            button5.Enabled = true;
            FileInfo newFile = new FileInfo(openFileDialog1.FileNames[0]);
            testpath = newFile.DirectoryName;
        }

        //Play selected file button
        private void button1_Click(object sender, EventArgs e)
        {
            string filename = null;
            try { filename = listBox1.SelectedItem.ToString(); }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("NO FILE SELECTED!", "SELECT FILE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Play selected .wav file using Naudio methods
            wave = new NAudio.Wave.WaveFileReader(filename);
            output = new NAudio.Wave.DirectSoundOut();
            output.Init(new NAudio.Wave.WaveChannel32(wave));
            output.Play();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Clear the wave
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing) output.Stop();
                output.Dispose();
                output = null;
            }
            if (wave != null)
            {
                wave.Dispose();
                wave = null;
            }
        }


        //Classify button function
        private void button4_Click(object sender, EventArgs e)
        {
            //Generate training and test data
            generatedata();
            //Classify the files according to generated model
            datamining();
        }

        //show training files
        private void button6_Click(object sender, EventArgs e)
        {
            listBox1.DataSource = openFileDialog2.FileNames;
        }

        //show test files
        private void button5_Click(object sender, EventArgs e)
        {
            listBox1.DataSource = openFileDialog1.FileNames;
        }

    }

}
