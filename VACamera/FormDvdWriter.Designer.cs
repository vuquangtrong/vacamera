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
            this.SuspendLayout();
            // 
            // btnWrite1
            // 
            this.btnWrite1.Location = new System.Drawing.Point(364, 36);
            this.btnWrite1.Name = "btnWrite1";
            this.btnWrite1.Size = new System.Drawing.Size(120, 40);
            this.btnWrite1.TabIndex = 0;
            this.btnWrite1.Text = "&Ghi đĩa chính";
            this.btnWrite1.UseVisualStyleBackColor = true;
            this.btnWrite1.Visible = false;
            this.btnWrite1.Click += new System.EventHandler(this.btnWrite1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(364, 269);
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
            this.listDrive1.Location = new System.Drawing.Point(119, 43);
            this.listDrive1.Name = "listDrive1";
            this.listDrive1.Size = new System.Drawing.Size(239, 29);
            this.listDrive1.TabIndex = 1;
            this.listDrive1.SelectedIndexChanged += new System.EventHandler(this.listDrive1_SelectedIndexChanged);
            this.listDrive1.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.listDrive_Format);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ổ đĩa chính:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ổ dự phòng:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(16, 100);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(468, 23);
            this.progressBar1.TabIndex = 3;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(16, 204);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(468, 23);
            this.progressBar2.TabIndex = 3;
            // 
            // txtFilename
            // 
            this.txtFilename.AutoSize = true;
            this.txtFilename.Location = new System.Drawing.Point(100, 9);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(73, 21);
            this.txtFilename.TabIndex = 2;
            this.txtFilename.Text = "Filename";
            // 
            // txtStatus1
            // 
            this.txtStatus1.AutoSize = true;
            this.txtStatus1.Location = new System.Drawing.Point(12, 76);
            this.txtStatus1.Name = "txtStatus1";
            this.txtStatus1.Size = new System.Drawing.Size(46, 21);
            this.txtStatus1.TabIndex = 2;
            this.txtStatus1.Text = "Xong";
            // 
            // txtStatus2
            // 
            this.txtStatus2.AutoSize = true;
            this.txtStatus2.Location = new System.Drawing.Point(12, 180);
            this.txtStatus2.Name = "txtStatus2";
            this.txtStatus2.Size = new System.Drawing.Size(46, 21);
            this.txtStatus2.TabIndex = 2;
            this.txtStatus2.Text = "Xong";
            // 
            // listDrive2
            // 
            this.listDrive2.FormattingEnabled = true;
            this.listDrive2.Location = new System.Drawing.Point(119, 144);
            this.listDrive2.Name = "listDrive2";
            this.listDrive2.Size = new System.Drawing.Size(239, 29);
            this.listDrive2.TabIndex = 1;
            this.listDrive2.SelectedIndexChanged += new System.EventHandler(this.listDrive2_SelectedIndexChanged);
            this.listDrive2.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.listDrive_Format);
            // 
            // backgroundBurnWorker1
            // 
            this.backgroundBurnWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundBurnWorker1_DoWork);
            this.backgroundBurnWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundBurnWorker1_ProgressChanged);
            this.backgroundBurnWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundBurnWorker1_RunWorkerCompleted);
            // 
            // backgroundBurnWorker2
            // 
            this.backgroundBurnWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundBurnWorker2_DoWork);
            this.backgroundBurnWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundBurnWorker2_ProgressChanged);
            this.backgroundBurnWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundBurnWorker2_RunWorkerCompleted);
            // 
            // btnWrite2
            // 
            this.btnWrite2.Enabled = false;
            this.btnWrite2.Location = new System.Drawing.Point(364, 137);
            this.btnWrite2.Name = "btnWrite2";
            this.btnWrite2.Size = new System.Drawing.Size(120, 40);
            this.btnWrite2.TabIndex = 0;
            this.btnWrite2.Text = "&Ghi đĩa phụ";
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
            this.btnWriteAll.Location = new System.Drawing.Point(16, 269);
            this.btnWriteAll.Name = "btnWriteAll";
            this.btnWriteAll.Size = new System.Drawing.Size(120, 40);
            this.btnWriteAll.TabIndex = 5;
            this.btnWriteAll.Text = "&Ghi đĩa";
            this.btnWriteAll.UseVisualStyleBackColor = true;
            this.btnWriteAll.Click += new System.EventHandler(this.btnWriteAll_Click);
            // 
            // FormDvdWriter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(496, 321);
            this.ControlBox = false;
            this.Controls.Add(this.btnWriteAll);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.txtStatus2);
            this.Controls.Add(this.txtStatus1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listDrive2);
            this.Controls.Add(this.listDrive1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnWrite2);
            this.Controls.Add(this.btnWrite1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximumSize = new System.Drawing.Size(512, 360);
            this.MinimumSize = new System.Drawing.Size(512, 360);
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
    }
}