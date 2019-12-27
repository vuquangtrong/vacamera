using System;

namespace VACamera
{
    partial class FormMain
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTimeRun = new System.Windows.Forms.Label();
            this.txtTimeLeft = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.timerRecord = new System.Windows.Forms.Timer(this.components);
            this.signalRecord = new System.Windows.Forms.PictureBox();
            this.btnWriteDisk = new System.Windows.Forms.Button();
            this.btnReplay = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnRecord = new System.Windows.Forms.Button();
            this.pictureFrame = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.signalRecord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 29);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSessionToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 25);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newSessionToolStripMenuItem
            // 
            this.newSessionToolStripMenuItem.Name = "newSessionToolStripMenuItem";
            this.newSessionToolStripMenuItem.Size = new System.Drawing.Size(212, 26);
            this.newSessionToolStripMenuItem.Text = "&Phiên làm việc mới";
            this.newSessionToolStripMenuItem.Click += new System.EventHandler(this.newSessionToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(212, 26);
            this.exitToolStripMenuItem.Text = "&Thoát";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(70, 25);
            this.settingsToolStripMenuItem.Text = "&Cài đặt";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(91, 25);
            this.aboutToolStripMenuItem.Text = "&Phiên bản";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(720, 514);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "Thời gian đã ghi:";
            // 
            // txtTimeRun
            // 
            this.txtTimeRun.AutoSize = true;
            this.txtTimeRun.Location = new System.Drawing.Point(851, 514);
            this.txtTimeRun.Name = "txtTimeRun";
            this.txtTimeRun.Size = new System.Drawing.Size(70, 21);
            this.txtTimeRun.TabIndex = 4;
            this.txtTimeRun.Text = "00:00:00";
            // 
            // txtTimeLeft
            // 
            this.txtTimeLeft.AutoSize = true;
            this.txtTimeLeft.Location = new System.Drawing.Point(851, 535);
            this.txtTimeLeft.Name = "txtTimeLeft";
            this.txtTimeLeft.Size = new System.Drawing.Size(70, 21);
            this.txtTimeLeft.TabIndex = 4;
            this.txtTimeLeft.Text = "00:00:00";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(720, 535);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 21);
            this.label5.TabIndex = 4;
            this.label5.Text = "Thời gian còn lại:";
            // 
            // timerRecord
            // 
            this.timerRecord.Interval = 1000;
            this.timerRecord.Tick += new System.EventHandler(this.timerRecord_Tick);
            // 
            // signalRecord
            // 
            this.signalRecord.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.signalRecord.Location = new System.Drawing.Point(937, 31);
            this.signalRecord.Name = "signalRecord";
            this.signalRecord.Size = new System.Drawing.Size(48, 48);
            this.signalRecord.TabIndex = 6;
            this.signalRecord.TabStop = false;
            // 
            // btnWriteDisk
            // 
            this.btnWriteDisk.Image = global::VACamera.Properties.Resources.burn;
            this.btnWriteDisk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnWriteDisk.Location = new System.Drawing.Point(956, 514);
            this.btnWriteDisk.Name = "btnWriteDisk";
            this.btnWriteDisk.Size = new System.Drawing.Size(40, 40);
            this.btnWriteDisk.TabIndex = 5;
            this.btnWriteDisk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnWriteDisk.UseVisualStyleBackColor = true;
            this.btnWriteDisk.Visible = false;
            this.btnWriteDisk.Click += new System.EventHandler(this.btnWriteDisk_Click);
            // 
            // btnReplay
            // 
            this.btnReplay.Image = global::VACamera.Properties.Resources.play;
            this.btnReplay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReplay.Location = new System.Drawing.Point(572, 517);
            this.btnReplay.Name = "btnReplay";
            this.btnReplay.Size = new System.Drawing.Size(140, 40);
            this.btnReplay.TabIndex = 4;
            this.btnReplay.Text = "Xem lại (F7)";
            this.btnReplay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReplay.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnReplay.UseVisualStyleBackColor = true;
            this.btnReplay.Click += new System.EventHandler(this.btnReplay_Click);
            // 
            // btnStop
            // 
            this.btnStop.Image = global::VACamera.Properties.Resources.stop;
            this.btnStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStop.Location = new System.Drawing.Point(407, 517);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(140, 40);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Kết thúc (F8)";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPause
            // 
            this.btnPause.Image = global::VACamera.Properties.Resources.pause;
            this.btnPause.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPause.Location = new System.Drawing.Point(242, 517);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(140, 40);
            this.btnPause.TabIndex = 2;
            this.btnPause.Text = "Dừng (F6)";
            this.btnPause.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPause.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnRecord
            // 
            this.btnRecord.Image = global::VACamera.Properties.Resources.rec;
            this.btnRecord.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRecord.Location = new System.Drawing.Point(77, 517);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(140, 40);
            this.btnRecord.TabIndex = 1;
            this.btnRecord.Text = "Ghi (F5)";
            this.btnRecord.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRecord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // pictureFrame
            // 
            this.pictureFrame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureFrame.Location = new System.Drawing.Point(77, 31);
            this.pictureFrame.MaximumSize = new System.Drawing.Size(854, 480);
            this.pictureFrame.MinimumSize = new System.Drawing.Size(854, 480);
            this.pictureFrame.Name = "pictureFrame";
            this.pictureFrame.Size = new System.Drawing.Size(854, 480);
            this.pictureFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureFrame.TabIndex = 2;
            this.pictureFrame.TabStop = false;
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1008, 561);
            this.ControlBox = false;
            this.Controls.Add(this.signalRecord);
            this.Controls.Add(this.txtTimeLeft);
            this.Controls.Add(this.txtTimeRun);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnWriteDisk);
            this.Controls.Add(this.btnReplay);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.pictureFrame);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1024, 600);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1024, 600);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VA Camera";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.signalRecord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureFrame)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnReplay;
        private System.Windows.Forms.Button btnWriteDisk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label txtTimeRun;
        private System.Windows.Forms.Label txtTimeLeft;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureFrame;
        private System.Windows.Forms.Timer timerRecord;
        private System.Windows.Forms.PictureBox signalRecord;
    }
}

