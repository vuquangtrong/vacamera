namespace VACamera
{
    partial class FormSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.listAudioSource = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.listAudioChannel = new System.Windows.Forms.ComboBox();
            this.txtCamera1 = new System.Windows.Forms.Label();
            this.listCamera1 = new System.Windows.Forms.ComboBox();
            this.listCamera2 = new System.Windows.Forms.ComboBox();
            this.txtCamera2 = new System.Windows.Forms.Label();
            this.listVideoMixingMode = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.listVideoFormat = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.listBitRate = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.listFrameRate = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nguồn Audio:";
            // 
            // listAudioSource
            // 
            this.listAudioSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listAudioSource.FormattingEnabled = true;
            this.listAudioSource.Location = new System.Drawing.Point(149, 6);
            this.listAudioSource.Name = "listAudioSource";
            this.listAudioSource.Size = new System.Drawing.Size(335, 29);
            this.listAudioSource.TabIndex = 1;
            this.listAudioSource.SelectedIndexChanged += new System.EventHandler(this.listAudioSource_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 21);
            this.label2.TabIndex = 0;
            this.label2.Text = "Số kênh Audio:";
            // 
            // listAudioChannel
            // 
            this.listAudioChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listAudioChannel.FormattingEnabled = true;
            this.listAudioChannel.Items.AddRange(new object[] {
            "Mono",
            "Stereo"});
            this.listAudioChannel.Location = new System.Drawing.Point(149, 49);
            this.listAudioChannel.Name = "listAudioChannel";
            this.listAudioChannel.Size = new System.Drawing.Size(335, 29);
            this.listAudioChannel.TabIndex = 1;
            this.listAudioChannel.SelectedIndexChanged += new System.EventHandler(this.listAudioChannel_SelectedIndexChanged);
            // 
            // txtCamera1
            // 
            this.txtCamera1.AutoSize = true;
            this.txtCamera1.Location = new System.Drawing.Point(12, 138);
            this.txtCamera1.Name = "txtCamera1";
            this.txtCamera1.Size = new System.Drawing.Size(112, 21);
            this.txtCamera1.TabIndex = 0;
            this.txtCamera1.Text = "Camera Chính:";
            // 
            // listCamera1
            // 
            this.listCamera1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listCamera1.FormattingEnabled = true;
            this.listCamera1.Location = new System.Drawing.Point(149, 135);
            this.listCamera1.Name = "listCamera1";
            this.listCamera1.Size = new System.Drawing.Size(335, 29);
            this.listCamera1.TabIndex = 1;
            this.listCamera1.SelectedIndexChanged += new System.EventHandler(this.listCamera1_SelectedIndexChanged);
            // 
            // listCamera2
            // 
            this.listCamera2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listCamera2.FormattingEnabled = true;
            this.listCamera2.Location = new System.Drawing.Point(149, 178);
            this.listCamera2.Name = "listCamera2";
            this.listCamera2.Size = new System.Drawing.Size(335, 29);
            this.listCamera2.TabIndex = 1;
            this.listCamera2.SelectedIndexChanged += new System.EventHandler(this.listCamera2_SelectedIndexChanged);
            // 
            // txtCamera2
            // 
            this.txtCamera2.AutoSize = true;
            this.txtCamera2.Location = new System.Drawing.Point(12, 181);
            this.txtCamera2.Name = "txtCamera2";
            this.txtCamera2.Size = new System.Drawing.Size(98, 21);
            this.txtCamera2.TabIndex = 2;
            this.txtCamera2.Text = "Camera Phụ:";
            // 
            // listVideoMixingMode
            // 
            this.listVideoMixingMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listVideoMixingMode.FormattingEnabled = true;
            this.listVideoMixingMode.Items.AddRange(new object[] {
            "Đơn",
            "Song Song",
            "Thu nhỏ"});
            this.listVideoMixingMode.Location = new System.Drawing.Point(149, 92);
            this.listVideoMixingMode.Name = "listVideoMixingMode";
            this.listVideoMixingMode.Size = new System.Drawing.Size(335, 29);
            this.listVideoMixingMode.TabIndex = 1;
            this.listVideoMixingMode.SelectedIndexChanged += new System.EventHandler(this.listVideoMixingMode_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 21);
            this.label6.TabIndex = 2;
            this.label6.Text = "Chế độ hiển thị:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(364, 319);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 40);
            this.button1.TabIndex = 3;
            this.button1.Text = "&OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 224);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 21);
            this.label3.TabIndex = 2;
            this.label3.Text = "Định dạng Video:";
            // 
            // listVideoFormat
            // 
            this.listVideoFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listVideoFormat.FormattingEnabled = true;
            this.listVideoFormat.Items.AddRange(new object[] {
            "MPEG4",
            "MPEG2"});
            this.listVideoFormat.Location = new System.Drawing.Point(149, 221);
            this.listVideoFormat.Name = "listVideoFormat";
            this.listVideoFormat.Size = new System.Drawing.Size(335, 29);
            this.listVideoFormat.TabIndex = 1;
            this.listVideoFormat.SelectedIndexChanged += new System.EventHandler(this.listVideoFormat_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(255, 268);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 21);
            this.label4.TabIndex = 2;
            this.label4.Text = "Bitrate:";
            // 
            // listBitRate
            // 
            this.listBitRate.FormattingEnabled = true;
            this.listBitRate.Items.AddRange(new object[] {
            "1500000",
            "3000000"});
            this.listBitRate.Location = new System.Drawing.Point(319, 265);
            this.listBitRate.Name = "listBitRate";
            this.listBitRate.Size = new System.Drawing.Size(165, 29);
            this.listBitRate.TabIndex = 4;
            this.listBitRate.SelectedIndexChanged += new System.EventHandler(this.listBitRate_SelectedIndexChanged);
            this.listBitRate.TextChanged += new System.EventHandler(this.listBitRate_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 268);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 21);
            this.label5.TabIndex = 2;
            this.label5.Text = "Số khung hình:";
            // 
            // listFrameRate
            // 
            this.listFrameRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listFrameRate.FormattingEnabled = true;
            this.listFrameRate.Items.AddRange(new object[] {
            "15",
            "24",
            "30"});
            this.listFrameRate.Location = new System.Drawing.Point(147, 265);
            this.listFrameRate.Name = "listFrameRate";
            this.listFrameRate.Size = new System.Drawing.Size(102, 29);
            this.listFrameRate.TabIndex = 5;
            this.listFrameRate.SelectedIndexChanged += new System.EventHandler(this.listFrameRate_SelectedIndexChanged);
            // 
            // FormSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(496, 381);
            this.Controls.Add(this.listFrameRate);
            this.Controls.Add(this.listBitRate);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCamera2);
            this.Controls.Add(this.listVideoMixingMode);
            this.Controls.Add(this.listAudioChannel);
            this.Controls.Add(this.listVideoFormat);
            this.Controls.Add(this.listCamera2);
            this.Controls.Add(this.listCamera1);
            this.Controls.Add(this.listAudioSource);
            this.Controls.Add(this.txtCamera1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(512, 420);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(512, 420);
            this.Name = "FormSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cài đặt";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox listAudioSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox listAudioChannel;
        private System.Windows.Forms.Label txtCamera1;
        private System.Windows.Forms.ComboBox listCamera1;
        private System.Windows.Forms.ComboBox listCamera2;
        private System.Windows.Forms.Label txtCamera2;
        private System.Windows.Forms.ComboBox listVideoMixingMode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox listVideoFormat;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox listBitRate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox listFrameRate;
    }
}