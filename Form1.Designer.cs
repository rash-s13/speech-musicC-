/*namespace Test
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(81, 54);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(137, 42);
            this.button2.TabIndex = 0;
            this.button2.Text = "Open WAV file";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(99, 126);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 29);
            this.button1.TabIndex = 1;
            this.button1.Text = "Play/Pause";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}
*/


namespace Test
{
    partial class AUDIOCLASSIFIER
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button3 = new System.Windows.Forms.Button();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Aquamarine;
            this.button2.Location = new System.Drawing.Point(50, 40);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(169, 30);
            this.button2.TabIndex = 0;
            this.button2.Text = "SELECT TRAINING FILES";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Aquamarine;
            this.button1.Location = new System.Drawing.Point(321, 201);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(145, 30);
            this.button1.TabIndex = 1;
            this.button1.Text = "PLAY SELECTED FILE";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(0, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(490, 13);
            this.textBox1.TabIndex = 3;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Aquamarine;
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(50, 201);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(169, 30);
            this.button4.TabIndex = 4;
            this.button4.Text = "CLASSIFY";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.PaleGreen;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(521, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(401, 433);
            this.listBox1.TabIndex = 6;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.RestoreDirectory = true;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Aquamarine;
            this.button3.Location = new System.Drawing.Point(50, 123);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(169, 31);
            this.button3.TabIndex = 7;
            this.button3.Text = "SELECT TEST FILES";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            this.openFileDialog2.Multiselect = true;
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.Aquamarine;
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(321, 123);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(145, 31);
            this.button5.TabIndex = 8;
            this.button5.Text = "SHOW TEST FILES";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.Aquamarine;
            this.button6.Enabled = false;
            this.button6.Location = new System.Drawing.Point(321, 40);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(145, 30);
            this.button6.TabIndex = 9;
            this.button6.Text = "SHOW TRAINING FILES";
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // AUDIOCLASSIFIER
            // 
            this.ClientSize = new System.Drawing.Size(934, 449);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Name = "AUDIOCLASSIFIER";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}

