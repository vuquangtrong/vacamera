namespace VACamera
{
    partial class FormDvdWriter
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
            this.btnWrite1 = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.listDrive1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.txtFilename = new System.Windows.Forms.Label();
            this.txtStatus1 = new System.Windows.Forms.Label();
            this.txtStatus2 = new System.Windows.Forms.Label();
            this.listDrive2 = new System.Windows.Forms.ComboBox();
            this.backgroundBurnWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundBurnWorker2 = new System.ComponentModel.BackgroundWorker();
            this.btnWrite2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnWriteAll = new System.Windows.Forms.Button();
            this.txtFileSize = new System.Windows.Forms.Label();
            this.txtTimeLeft2 = new System.Windows.Forms.Label();
            this.txtTimeLeft1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnWrite3 = new System.Windows.Forms.Button();
            this.listDrive3 = new System.Windows.Forms.ComboBox();
            this.progressBar3 = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.txtStatus3 = new System.Windows.Forms.Label();
            this.txtTimeLeft3 = new System.Windows.Forms.Label();
            this.backgroundBurnWorker3 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // btnWrite1
            // 
            this.btnWrite1.Location = new System.Drawing.Point(193, 221);
            this.btnWrite1.Name = "btnWrite1";
            this.btnWrite1.Size = new System.Drawing.Size(40, 40);
            this.btnWrite1.TabIndex = 0;
            this.btnWrite1.Text = "1";
            this.btnWrite1.UseVisualStyleBackColor = true;
            this.btnWrite1.Visible = false;
            this.btnWrite1.Click += new System.EventHandler(this.btnWrite1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(364, 221);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 40);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "&Đóng";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // listDrive1
            // 
            this.listDrive1.FormattingEnabled = true;
            this.listDrive1.Location = new System.Drawing.Point(147, 228);
            this.listDrive1.Name = "listDrive1";
            this.listDrive1.Size = new System.Drawing.Size(40, 29);
            this.listDrive1.TabIndex = 1;
            this.listDrive1.Visible = false;
            this.listDrive1.SelectedIndexChanged += new System.EventHandler(this.listDrive1_SelectedIndexChanged);
            this.listDrive1.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.listDrive_Format);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "DVD 1:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(296, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "DVD 2:";
            this.label2.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(16, 108);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(468, 23);
            this.progressBar1.TabIndex = 3;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(212, 2);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(78, 23);
            this.progressBar2.TabIndex = 3;
            this.progressBar2.Visible = false;
            // 
            // txtFilename
            // 
            this.txtFilename.AutoSize = true;
            this.txtFilename.Location = new System.Drawing.Point(114, 9);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(73, 21);
            this.txtFilename.TabIndex = 2;
            this.txtFilename.Text = "Filename";
            // 
            // txtStatus1
            // 
            this.txtStatus1.AutoSize = true;
            this.txtStatus1.Location = new System.Drawing.Point(78, 82);
            this.txtStatus1.Name = "txtStatus1";
            this.txtStatus1.Size = new System.Drawing.Size(73, 21);
            this.txtStatus1.TabIndex = 2;
            this.txtStatus1.Text = "DVD-RW";
            // 
            // txtStatus2
            // 
            this.txtStatus2.AutoSize = true;
            this.txtStatus2.Location = new System.Drawing.Point(360, 2);
            this.txtStatus2.Name = "txtStatus2";
            this.txtStatus2.Size = new System.Drawing.Size(73, 21);
            this.txtStatus2.TabIndex = 2;
            this.txtStatus2.Text = "DVD-RW";
            this.txtStatus2.Visible = false;
            // 
            // listDrive2
            // 
            this.listDrive2.FormattingEnabled = true;
            this.listDrive2.Location = new System.Drawing.Point(239, 228);
            this.listDrive2.Name = "listDrive2";
            this.listDrive2.Size = new System.Drawing.Size(40, 29);
            this.listDrive2.TabIndex = 1;
            this.listDrive2.Visible = false;
            this.listDrive2.SelectedIndexChanged += new System.EventHandler(this.listDrive2_SelectedIndexChanged);
            this.listDrive2.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.listDrive_Format);
            // 
            // backgroundBurnWorker1
            // 
            this.backgroundBurnWorker1.WorkerReportsProgress = true;
            this.backgroundBurnWorker1.WorkerSupportsCancellation = true;
            this.backgroundBurnWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundBurnWorker1_DoWork);
            this.backgroundBurnWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundBurnWorker1_ProgressChanged);
            this.backgroundBurnWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundBurnWorker1_RunWorkerCompleted);
            // 
            // backgroundBurnWorker2
            // 
            this.backgroundBurnWorker2.WorkerReportsProgress = true;
            this.backgroundBurnWorker2.WorkerSupportsCancellation = true;
            this.backgroundBurnWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundBurnWorker2_DoWork);
            this.backgroundBurnWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundBurnWorker2_ProgressChanged);
            this.backgroundBurnWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundBurnWorker2_RunWorkerCompleted);
            // 
            // btnWrite2
            // 
            this.btnWrite2.Location = new System.Drawing.Point(285, 221);
            this.btnWrite2.Name = "btnWrite2";
            this.btnWrite2.Size = new System.Drawing.Size(40, 40);
            this.btnWrite2.TabIndex = 0;
            this.btnWrite2.Text = "2";
            this.btnWrite2.UseVisualStyleBackColor = true;
            this.btnWrite2.Visible = false;
            this.btnWrite2.Click += new System.EventHandler(this.btnWrite2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 21);
            this.label3.TabIndex = 4;
            this.label3.Text = "Nội dung: ";
            // 
            // btnWriteAll
            // 
            this.btnWriteAll.Location = new System.Drawing.Point(16, 221);
            this.btnWriteAll.Name = "btnWriteAll";
            this.btnWriteAll.Size = new System.Drawing.Size(120, 40);
            this.btnWriteAll.TabIndex = 5;
            this.btnWriteAll.Text = "&Ghi đĩa";
            this.btnWriteAll.UseVisualStyleBackColor = true;
            this.btnWriteAll.Click += new System.EventHandler(this.btnWriteAll_Click);
            // 
            // txtFileSize
            // 
            this.txtFileSize.AutoSize = true;
            this.txtFileSize.Location = new System.Drawing.Point(114, 36);
            this.txtFileSize.Name = "txtFileSize";
            this.txtFileSize.Size = new System.Drawing.Size(46, 21);
            this.txtFileSize.TabIndex = 2;
            this.txtFileSize.Text = "0 MB";
            // 
            // txtTimeLeft2
            // 
            this.txtTimeLeft2.AutoSize = true;
            this.txtTimeLeft2.Location = new System.Drawing.Point(435, 2);
            this.txtTimeLeft2.Name = "txtTimeLeft2";
            this.txtTimeLeft2.Size = new System.Drawing.Size(49, 21);
            this.txtTimeLeft2.TabIndex = 2;
            this.txtTimeLeft2.Text = "00:00";
            this.txtTimeLeft2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.txtTimeLeft2.Visible = false;
            // 
            // txtTimeLeft1
            // 
            this.txtTimeLeft1.AutoSize = true;
            this.txtTimeLeft1.Location = new System.Drawing.Point(435, 82);
            this.txtTimeLeft1.Name = "txtTimeLeft1";
            this.txtTimeLeft1.Size = new System.Drawing.Size(49, 21);
            this.txtTimeLeft1.TabIndex = 2;
            this.txtTimeLeft1.Text = "00:00";
            this.txtTimeLeft1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtTimeLeft1.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 21);
            this.label7.TabIndex = 4;
            this.label7.Text = "Dung lượng:";
            // 
            // btnWrite3
            // 
            this.btnWrite3.Enabled = false;
            this.btnWrite3.Location = new System.Drawing.Point(424, 26);
            this.btnWrite3.Name = "btnWrite3";
            this.btnWrite3.Size = new System.Drawing.Size(60, 40);
            this.btnWrite3.TabIndex = 0;
            this.btnWrite3.Text = "USB";
            this.btnWrite3.UseVisualStyleBackColor = true;
            this.btnWrite3.Visible = false;
            this.btnWrite3.Click += new System.EventHandler(this.btnWrite3_Click);
            // 
            // listDrive3
            // 
            this.listDrive3.FormattingEnabled = true;
            this.listDrive3.Location = new System.Drawing.Point(378, 33);
            this.listDrive3.Name = "listDrive3";
            this.listDrive3.Size = new System.Drawing.Size(40, 29);
            this.listDrive3.TabIndex = 1;
            this.listDrive3.Visible = false;
            // 
            // progressBar3
            // 
            this.progressBar3.Location = new System.Drawing.Point(16, 168);
            this.progressBar3.Name = "progressBar3";
            this.progressBar3.Size = new System.Drawing.Size(468, 23);
            this.progressBar3.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 21);
            this.label4.TabIndex = 2;
            this.label4.Text = "USB:";
            // 
            // txtStatus3
            // 
            this.txtStatus3.AutoSize = true;
            this.txtStatus3.Location = new System.Drawing.Point(78, 142);
            this.txtStatus3.Name = "txtStatus3";
            this.txtStatus3.Size = new System.Drawing.Size(88, 21);
            this.txtStatus3.TabIndex = 2;
            this.txtStatus3.Text = "Removable";
            // 
            // txtTimeLeft3
            // 
            this.txtTimeLeft3.AutoSize = true;
            this.txtTimeLeft3.Location = new System.Drawing.Point(435, 142);
            this.txtTimeLeft3.Name = "txtTimeLeft3";
            this.txtTimeLeft3.Size = new System.Drawing.Size(49, 21);
            this.txtTimeLeft3.TabIndex = 2;
            this.txtTimeLeft3.Text = "00:00";
            this.txtTimeLeft3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.txtTimeLeft3.Visible = false;
            // 
            // backgroundBurnWorker3
            // 
            this.backgroundBurnWorker3.WorkerReportsProgress = true;
            this.backgroundBurnWorker3.WorkerSupportsCancellation = true;
            this.backgroundBurnWorker3.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundBurnWorker3_DoWork);
            this.backgroundBurnWorker3.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundBurnWorker3_ProgressChanged);
            this.backgroundBurnWorker3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundBurnWorker3_RunWorkerCompleted);
            // 
            // FormDvdWriter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(496, 281);
            this.ControlBox = false;
            this.Controls.Add(this.progressBar3);
            this.Controls.Add(this.btnWriteAll);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtFileSize);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.txtTimeLeft1);
            this.Controls.Add(this.txtTimeLeft3);
            this.Controls.Add(this.txtStatus3);
            this.Controls.Add(this.txtTimeLeft2);
            this.Controls.Add(this.txtStatus2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtStatus1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listDrive3);
            this.Controls.Add(this.listDrive2);
            this.Controls.Add(this.listDrive1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnWrite3);
            this.Controls.Add(this.btnWrite2);
            this.Controls.Add(this.btnWrite1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximumSize = new System.Drawing.Size(512, 320);
            this.MinimumSize = new System.Drawing.Size(512, 320);
            this.Name = "FormDvdWriter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ghi đĩa DVD";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDvdWriter_FormClosing);
            this.Load += new System.EventHandler(this.FormDvdWriter_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnWrite1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox listDrive1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Label txtFilename;
        private System.Windows.Forms.Label txtStatus1;
        private System.Windows.Forms.Label txtStatus2;
        private System.Windows.Forms.ComboBox listDrive2;
        private System.ComponentModel.BackgroundWorker backgroundBurnWorker1;
        private System.ComponentModel.BackgroundWorker backgroundBurnWorker2;
        private System.Windows.Forms.Button btnWrite2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnWriteAll;
        private System.Windows.Forms.Label txtFileSize;
        private System.Windows.Forms.Label txtTimeLeft2;
        private System.Windows.Forms.Label txtTimeLeft1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnWrite3;
        private System.Windows.Forms.ComboBox listDrive3;
        private System.Windows.Forms.ProgressBar progressBar3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label txtStatus3;
        private System.Windows.Forms.Label txtTimeLeft3;
        private System.ComponentModel.BackgroundWorker backgroundBurnWorker3;
    }
}