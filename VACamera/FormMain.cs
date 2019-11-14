using Accord.Audio;
using Accord.DirectSound;
using Accord.Video.DirectShow;
using Accord.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace VACamera
{
    public partial class FormMain : Form
    {
        string outputFolder = Environment.CurrentDirectory + "\\records";
        string outputFile = "";

        SessionInfo sessionInfo = new SessionInfo();
        Settings settings = new Settings();

        FilterInfoCollection videoDevices;

        VideoCaptureDevice videoDevice1 = null;
        Bitmap frame1 = null;
        static readonly Object syncFrame1 = new Object();
        bool isFrame1_Rendering = false;

        VideoCaptureDevice videoDevice2 = null;
        Bitmap frame2 = null;
        static readonly Object syncFrame2 = new Object();
        bool isFrame2_Rendering = false;

        List<AudioDeviceInfo> audioDevices = null;
        AudioCaptureDevice audioDevice = null;

        Bitmap videoFrame = null;
        Graphics graphics = null;
        Font textFont = new Font("Tahoma", 18);
        StringFormat textDirectionRTL = new StringFormat(StringFormatFlags.DirectionRightToLeft);
        Rectangle textLine1 = new Rectangle(10, 10, Settings.VideoWidth - 20, 40);
        Rectangle textLine2 = new Rectangle(10, Settings.VideoHeight - 10 - 40 - 40, Settings.VideoWidth - 20, 40);
        Rectangle textLine3 = new Rectangle(10, Settings.VideoHeight - 10 - 40, Settings.VideoWidth - 20, 40);

        Stopwatch stopWatchFPS = null;

        enum VideoRecordState
        {
            IDLE,
            RECORDING,
            PAUSE
        }
        VideoRecordState videoRecordState = VideoRecordState.IDLE;
        VideoFileWriter videoFileWriter = new VideoFileWriter();
        static readonly Object syncRender = new Object();

        int frameSkipAt = 0;
        int frameCount = 0;
        int actualFrameRate = 30;

        //Thread videoRender;
        //bool isTickRender = false;

        int recordTime = 0;
        int runtime = 0;

        public FormMain()
        {
            InitializeComponent();

            try
            {
                Directory.CreateDirectory(outputFolder);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // disable buttons for new session
            btnRecord.Enabled = false;
            btnPause.Enabled = false;
            btnStop.Enabled = false;
            btnReplay.Enabled = false;
            btnWriteDisk.Enabled = false;

            // show new session
            using (FormNewSession formNewSession = new FormNewSession())
            {
                if (formNewSession.ShowDialog(this) == DialogResult.OK)
                {
                    sessionInfo = formNewSession.SessionInfo;

                    Log.WriteLine(sessionInfo.ToString());
                    Log.WriteLine(settings.ToString());
                }
                else
                {
                    Close();
                }
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            // init render buffer
            videoFrame = new Bitmap(Settings.VideoWidth, Settings.VideoHeight, PixelFormat.Format24bppRgb);
            graphics = Graphics.FromImage(videoFrame);
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;

            // init render thread
            //videoRender = new Thread(VideoRenderWorker);
            //videoRender.Start();

            // start devices at startup
            InitDevices();

            // debug runtime
            //timer1.Start();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopRecording();
            StopDevices();
            //StopRender();

            // debug runtime
            //timer1.Stop();

            settings.SaveSettings();
        }

        //private void VideoRenderWorker()
        //{
        //    while (true)
        //    {
        //        if (isTickRender)
        //        {
        //            RenderFrame();
        //        }
        //        else
        //        {
        //            Thread.Sleep(30);
        //        }
        //    }
        //}

        private void InitDevices()
        {
            // stop running devices
            try
            {
                StopDevices();
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            // start audio device
            try
            {
                audioDevices = new List<AudioDeviceInfo>(new AudioDeviceCollection(AudioDeviceCategory.Capture));
                if (audioDevices != null && audioDevices.Count > 0)
                {
                    AudioDeviceInfo audioDeviceInfo = null;

                    // find last used device
                    foreach (AudioDeviceInfo device in audioDevices)
                    {
                        if (settings.AudioInputPath.Equals(device.Description + "|" + device.Guid.ToString()))
                        {
                            audioDeviceInfo = device;
                        }
                    }

                    // if nothing matched, use default device
                    if (audioDeviceInfo == null)
                    {
                        audioDeviceInfo = audioDevices[0];
                    }

                    // setup audio device

                    audioDevice = new AudioCaptureDevice(audioDeviceInfo)
                    {
                        Format = SampleFormat.Format32BitIeeeFloat, // 32bit Float
                        // NumberOfChannels = 1, // current, Accord only supports Mono
                        SampleRate = settings.AudioSampleRate, // 44100 Hz
                        DesiredFrameSize = settings.AudioFrameSize // 40 Kb
                    };
                    Log.WriteLine("audioDevice.Format = " + audioDevice.Format.ToString());
                    Log.WriteLine("audioDevice.SampleRate = " + audioDevice.SampleRate);
                    Log.WriteLine("audioDevice.DesiredFrameSize = " + audioDevice.DesiredFrameSize);
                    Log.WriteLine("audioDevice.NumberOfChannels = " + audioDevice.NumberOfChannels);

                    audioDevice.NewFrame += AudioDevice_NewFrame;
                    audioDevice.Start();
                    Log.WriteLine(">>> START audioDevice: " + audioDeviceInfo.Description + "|" + audioDeviceInfo.Guid.ToString());
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thiết bị thu âm thanh!");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            // start video devices
            string device1_MonikerString = "";
            string device2_MonikerString = "";
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices != null && videoDevices.Count > 0)
                {
                    // find last used devices
                    foreach (FilterInfo device in videoDevices)
                    {
                        if (settings.Camera1_InputPath.Equals(device.MonikerString))
                        {
                            device1_MonikerString = device.MonikerString;
                        }

                        if (settings.Camera2_InputPath.Equals(device.MonikerString))
                        {
                            device2_MonikerString = device.MonikerString;
                        }
                    }

                    // not matched, use default device
                    if (device1_MonikerString.Equals(""))
                    {
                        device1_MonikerString = videoDevices[0].MonikerString;
                    }
                    videoDevice1 = new VideoCaptureDevice(device1_MonikerString);
                    videoDevice1.DesiredAverageTimePerFrame = 330000; // ns
                    videoDevice1.NewFrame += VideoDevice1_NewFrame;
                    videoDevice1.Start();
                    Log.WriteLine(">>> START videoDevice1: " + device1_MonikerString);

                    // open second device if needed
                    if (videoDevices.Count > 1)
                    {
                        if (device2_MonikerString.Equals(""))
                        {
                            device2_MonikerString = videoDevices[1].MonikerString;
                        }
                    }
                    else
                    {
                        // fallback to Single mode
                        settings.SetVideoMixingMode(Settings.VideoMode.Single);
                    }

                    if (settings.VideoMixingMode != Settings.VideoMode.Single)
                    {
                        videoDevice2 = new VideoCaptureDevice(device2_MonikerString);
                        videoDevice2.DesiredAverageTimePerFrame = 330000; // ns
                        videoDevice2.NewFrame += VideoDevice2_NewFrame;
                        videoDevice2.Start();
                        Log.WriteLine(">>> START videoDevice2: " + device2_MonikerString);
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thiết bị thu hình ảnh!");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            // change buttons state
            if (videoDevice1 != null)
            {
                btnRecord.Enabled = true;
                btnPause.Enabled = false;
                btnStop.Enabled = false;
                btnReplay.Enabled = false;
                btnWriteDisk.Enabled = false;

                // reset record time
                UpdateRecordTime(0);
            }

            // TODO: if framerate input is 30fps, need to set drop frame ratio
            //if (settings.VideoFrameRate == 24)
            //{
            //    frameSkipAt = 5;
            //    frameCount = 0;
            //}
            //else if (settings.VideoFrameRate == 15)
            //{
            //    frameSkipAt = 2;
            //    frameCount = 0;
            //}
            //else
            //{
            //    frameSkipAt = 0;
            //    frameCount = 0;
            //}

            // monitor actual framerate
            stopWatchFPS = null;
            timerFPS.Start();

            Log.WriteLine(settings.ToString());
        }

        private void StopDevices()
        {
            try
            {
                if (audioDevice != null)
                {
                    audioDevice.NewFrame -= AudioDevice_NewFrame;

                    audioDevice.SignalToStop();
                    audioDevice.WaitForStop();
                    Thread.Sleep(1000);

                    audioDevice.Dispose();
                    audioDevice = null;
                    Log.WriteLine(">>> STOP audioDevice");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            try
            {
                if (videoDevice1 != null)
                {
                    videoDevice1.NewFrame -= VideoDevice1_NewFrame;
                    videoDevice1.SignalToStop();
                    videoDevice1.WaitForStop();
                    Thread.Sleep(1000);

                    videoDevice1 = null;
                    Log.WriteLine(">>> STOP videoDevice1");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            try
            {
                if (videoDevice2 != null)
                {
                    videoDevice2.NewFrame -= VideoDevice2_NewFrame;
                    videoDevice2.SignalToStop();
                    videoDevice2.WaitForStop();
                    Thread.Sleep(1000);

                    videoDevice2 = null;
                    Log.WriteLine(">>> STOP videoDevice2");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            timerFPS.Stop();
        }

        delegate void UpdateLiveImageInvokerCallBack(PictureBox pictureBox, Bitmap bitmap);

        private void UpdateLiveImageInvoker(PictureBox pictureBox, Bitmap bitmap)
        {
            if (InvokeRequired)
            {
                UpdateLiveImageInvokerCallBack invoker = new UpdateLiveImageInvokerCallBack(UpdateLiveImageInvoker);
                Invoke(invoker, new object[] { pictureBox, bitmap });
            }
            else
            {
                UpdateLiveImage(pictureBox, bitmap);
            }
        }

        private void UpdateLiveImage(PictureBox pictureBox, Bitmap bitmap)
        {
            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
            }
            pictureBox.Image = bitmap;
        }

        private void AudioDevice_NewFrame(object sender, NewFrameEventArgs e)
        {
            lock (syncRender)
            {
                try
                {
                    if (videoRecordState == VideoRecordState.RECORDING)
                    {
                        videoFileWriter.WriteAudioFrame(e.Signal);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.ToString());
                }
            }
        }

        private void VideoDevice1_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            bool freeToGo = false;
            lock (syncFrame1)
            {
                freeToGo = !isFrame1_Rendering;
            }
            if (freeToGo)
            {
                lock (syncFrame1)
                {
                    if (frame1 != null)
                    {
                        frame1.Dispose();
                    }
                    frame1 = (Bitmap)eventArgs.Frame.Clone();
                    RenderFrame();
                }
            }
            else
            {
                //Log.WriteLine("skip frame 1");
            }
        }

        private void VideoDevice2_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            bool freeToGo = false;
            lock (syncFrame2)
            {
                freeToGo = !isFrame2_Rendering;
            }
            if (freeToGo)
            {
                lock (syncFrame2)
                {
                    if (frame2 != null)
                    {
                        frame2.Dispose();
                    }
                    frame2 = (Bitmap)eventArgs.Frame.Clone();
                }
            }
            else
            {
                //Log.WriteLine("skip frame 2");
            }
        }

        private void RenderFrame()
        {
            // drop frame if needed
            if (frameSkipAt > 0)
            {
                frameCount++;
                if (frameCount == frameSkipAt)
                {
                    frameCount = 0;
                    return;
                }
            }

            // render frame 1
            lock (syncFrame1)
            {
                isFrame1_Rendering = true;
            }

            if (frame1 != null)
            {
                try
                {

                    Bitmap frame1_Clone = null;
                    lock (syncFrame1)
                    {
                        frame1_Clone = (Bitmap)frame1.Clone();
                    }
                    graphics.DrawImage(frame1_Clone,
                        new Rectangle(settings.Frame1_X, settings.Frame1_Y, settings.Frame1_Width, settings.Frame1_Height),
                        new Rectangle(0, 0, frame1.Width, frame1.Height),
                        GraphicsUnit.Pixel);
                    if (frame1_Clone != null)
                    {
                        frame1_Clone.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.ToString());
                    lock (syncFrame1)
                    {
                        isFrame1_Rendering = false;
                    }
                    return;
                }
            }

            lock (syncFrame1)
            {
                isFrame1_Rendering = false;
            }

            // render frame 2
            if (settings.VideoMixingMode != Settings.VideoMode.Single)
            {
                lock (syncFrame2)
                {
                    isFrame2_Rendering = true;
                }

                if (frame2 != null)
                {
                    try
                    {
                        Bitmap frame2_Clone = null;
                        lock (syncFrame1)
                        {
                            frame2_Clone = (Bitmap)frame2.Clone();
                        }
                        graphics.DrawImage(frame2_Clone,
                            new Rectangle(settings.Frame2_X, settings.Frame2_Y, settings.Frame2_Width, settings.Frame2_Height),
                            new Rectangle(0, 0, frame2.Width, frame2.Height),
                            GraphicsUnit.Pixel);
                        if (frame2_Clone != null)
                        {
                            frame2_Clone.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                        lock (syncFrame2)
                        {
                            isFrame2_Rendering = false;
                        }
                        return;
                    }
                }
                lock (syncFrame2)
                {
                    isFrame2_Rendering = false;
                }
            }

            // add text
            // TopLeft: Name1
            graphics.DrawString(sessionInfo.Name1, textFont, Brushes.Red, textLine1);
            // TopRight: Name 2
            graphics.DrawString(sessionInfo.Name2, textFont, Brushes.Red, textLine1, textDirectionRTL);
            // BottomLeft: Name 3, Name 4
            graphics.DrawString(sessionInfo.Name3, textFont, Brushes.Red, textLine2);
            graphics.DrawString(sessionInfo.Name4, textFont, Brushes.Red, textLine3);
            // BottomRight: Name 5 and DateTime
            graphics.DrawString(sessionInfo.Name5, textFont, Brushes.Red, textLine2, textDirectionRTL);
            graphics.DrawString(DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), textFont, Brushes.Red, textLine3, textDirectionRTL);

            // show preview
            UpdateLiveImage(pictureFrame, (Bitmap)videoFrame.Clone());

            // write file
            if (videoRecordState == VideoRecordState.RECORDING)
            {
                lock (syncRender)
                {
                    try
                    {
                        using (Bitmap videoFrame_Clone = (Bitmap)videoFrame.Clone())
                        {
                            videoFileWriter.WriteVideoFrame(videoFrame_Clone);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                    }
                }
            }
        }

        private void StartRecording()
        {
            if (videoRecordState == VideoRecordState.IDLE)
            {
                VideoCodec videoCodec = VideoCodec.Mpeg4;
                string videoExtension = ".mp4";

                if (settings.VideoOutputFormat == Settings.VideoFormat.MPEG2)
                {
                    videoCodec = VideoCodec.Mpeg2;
                    videoExtension = ".mpg";
                }

                outputFile = outputFolder + "\\" + sessionInfo.DateTime + videoExtension;

                videoFileWriter = new VideoFileWriter();
                videoFileWriter.BitRate = settings.VideoBitRate;
                videoFileWriter.FrameRate = actualFrameRate; /* settings.VideoFrameRate; */
                videoFileWriter.Width = Settings.VideoWidth;
                videoFileWriter.Height = Settings.VideoHeight;
                videoFileWriter.VideoCodec = videoCodec;

                // advanced settings
                //videoFileWriter.VideoOptions["crf"] = "18"; // visually lossless
                //videoFileWriter.VideoOptions["preset"] = "veryfast";
                //videoFileWriter.VideoOptions["tune"] = "zerolatency";

                if (audioDevice != null)
                {
                    videoFileWriter.AudioBitRate = settings.AudioBitRate; // 160 kbps
                    videoFileWriter.AudioCodec = AudioCodec.Aac;
                    videoFileWriter.AudioLayout = AudioLayout.Mono; // (settings.AudioChannel == Settings.AudioMode.Mono) ? AudioLayout.Mono : AudioLayout.Stereo; // Accord only supports Mono
                    videoFileWriter.FrameSize = settings.AudioFrameSize; // 40 kb
                    videoFileWriter.SampleRate = settings.AudioSampleRate; // 44100 Hz

                    Log.WriteLine("RECORD HAS AUDIO");
                }

                // open file to write
                videoFileWriter.Open(outputFile);

                timerRecord.Start();
                videoRecordState = VideoRecordState.RECORDING;
                Log.WriteLine(">>> START recording");

            }
            else if (videoRecordState == VideoRecordState.PAUSE)
            {
                timerRecord.Start();
                videoRecordState = VideoRecordState.RECORDING;
                Log.WriteLine(">>> RESUME recording");
            }
        }

        private void PauseRecording()
        {
            if (videoRecordState == VideoRecordState.RECORDING)
            {
                videoRecordState = VideoRecordState.PAUSE;

                lock (syncRender)
                {
                    if (videoFileWriter.IsOpen)
                    {
                        videoFileWriter.Flush();

                    }
                }

                timerRecord.Stop();
                Log.WriteLine(">>> PAUSE recording");
            }
        }

        private void StopRecording()
        {
            if (videoRecordState == VideoRecordState.RECORDING
                || videoRecordState == VideoRecordState.PAUSE)
            {
                videoRecordState = VideoRecordState.IDLE;

                lock (syncRender)
                {
                    if (videoFileWriter.IsOpen)
                    {
                        videoFileWriter.Flush();
                        videoFileWriter.Close();
                        videoFileWriter.Dispose();
                        videoFileWriter = null;
                    }
                }

                timerRecord.Stop();
                Log.WriteLine(">>> STOP recording");
            }
        }

        //private void StopRender()
        //{
        //    if (videoRender != null)
        //    {
        //        if (videoRender.IsAlive)
        //        {
        //            videoRender.Abort();
        //        }
        //    }
        //}

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Thoát ứng dụng?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void newSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hide();

            using (FormNewSession formNewSession = new FormNewSession())
            {
                if (formNewSession.ShowDialog(this) == DialogResult.OK)
                {
                    sessionInfo = formNewSession.SessionInfo;

                    // debug
                    Log.WriteLine(sessionInfo.ToString());
                    Log.WriteLine(settings.ToString());
                }
            }

            if (videoDevice1 != null)
            {
                btnRecord.Enabled = true;
                btnPause.Enabled = false;
                btnStop.Enabled = false;
                btnReplay.Enabled = false;
                btnWriteDisk.Enabled = false;
            }

            UpdateRecordTime(0);
            InitDevices();
            Show();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FormSettings formSettings = new FormSettings())
            {
                if (formSettings.ShowDialog(this) == DialogResult.OK)
                {
                    settings = formSettings.Settings;
                    InitDevices();
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FormAbout formAbout = new FormAbout())
            {
                if (formAbout.ShowDialog(this) == DialogResult.OK)
                {

                }
            }
        }

        private void FormMain_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (videoRecordState == VideoRecordState.IDLE
                || videoRecordState == VideoRecordState.PAUSE)
            {
                btnRecord.Enabled = false;
                btnPause.Enabled = true;
                btnStop.Enabled = true;
                btnReplay.Enabled = false;
                btnWriteDisk.Enabled = false;

                settingsToolStripMenuItem.Enabled = false; // do not change settings during recording
                signalRecord.BackgroundImage = global::VACamera.Properties.Resources.rec_on;

                StartRecording();
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (videoRecordState == VideoRecordState.RECORDING)
            {
                btnRecord.Enabled = true;
                btnPause.Enabled = false;
                btnStop.Enabled = true;
                btnReplay.Enabled = true;
                btnWriteDisk.Enabled = false;

                signalRecord.BackgroundImage = global::VACamera.Properties.Resources.rec_pause;

                PauseRecording();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (videoRecordState == VideoRecordState.RECORDING
                || videoRecordState == VideoRecordState.PAUSE)
            {
                if (MessageBox.Show("Kết thúc ghi hình và ghi DVD?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    btnRecord.Enabled = false; // must start new session
                    btnPause.Enabled = false;
                    btnStop.Enabled = false;
                    btnReplay.Enabled = true;
                    btnWriteDisk.Enabled = true;

                    settingsToolStripMenuItem.Enabled = true; // can change settings again
                    signalRecord.BackgroundImage = null;

                    StopRecording();
                    Thread.Sleep(1000);

                    StopDevices();
                    btnWriteDisk_Click(new object(), new EventArgs());
                }
            }
        }

        private void btnReplay_Click(object sender, EventArgs e)
        {
            if (videoRecordState == VideoRecordState.IDLE
                || videoRecordState == VideoRecordState.PAUSE)
            {
                if (File.Exists(outputFile))
                {
                    Process.Start(outputFile);
                }
            }
        }

        private void btnWriteDisk_Click(object sender, EventArgs e)
        {
            Log.WriteLine(sessionInfo.ToString());
            Log.WriteLine("outputFile = " + outputFile);
            using (FormDvdWriter formDvdWriter = new FormDvdWriter(sessionInfo.DateTime, outputFile))
            {
                DialogResult result = formDvdWriter.ShowDialog();
                if (result == DialogResult.Yes)
                {
                    newSessionToolStripMenuItem_Click(new object(), new EventArgs());
                }
                else if (result == DialogResult.No)
                {
                    MessageBox.Show("Kết thúc phiên làm việc và dừng chương trình!");
                    Close();
                }
            }
        }

        private void timerRecord_Tick(object sender, EventArgs e)
        {
            recordTime++;
            UpdateRecordTime();
        }

        private void UpdateRecordTime(int start = -1)
        {
            if (start >= 0)
            {
                recordTime = start;
            }

            TimeSpan timeRun = TimeSpan.FromSeconds(recordTime);
            txtTimeRun.Text = timeRun.ToString("hh':'mm':'ss");

            TimeSpan timeLeft = TimeSpan.FromSeconds(sessionInfo.MaxTime - recordTime);
            txtTimeLeft.Text = timeLeft.ToString("hh':'mm':'ss");

            if (videoRecordState == VideoRecordState.RECORDING)
            {
                if (recordTime % 2 == 0)
                {
                    signalRecord.BackgroundImage = global::VACamera.Properties.Resources.rec_on;
                }
                else
                {
                    signalRecord.BackgroundImage = global::VACamera.Properties.Resources.rec_off;
                }
            }
        }

        private void timerFPS_Tick(object sender, EventArgs e)
        {
            //isTickRender = true;

            int framesReceived1 = 0;
            int framesReceived2 = 0;

            // get number of frames for the last second
            if (videoDevice1 != null)
            {
                framesReceived1 = videoDevice1.FramesReceived;
            }

            if (videoDevice2 != null)
            {
                framesReceived2 = videoDevice2.FramesReceived;
            }

            if (stopWatchFPS == null)
            {
                stopWatchFPS = new Stopwatch();
                stopWatchFPS.Start();
            }
            else
            {
                stopWatchFPS.Stop();

                float fps1 = 1000.0f * framesReceived1 / stopWatchFPS.ElapsedMilliseconds;
                float fps2 = 1000.0f * framesReceived2 / stopWatchFPS.ElapsedMilliseconds;

                txtCamFps1.Text = fps1.ToString("F2") + " fps";
                txtCamFps2.Text = fps2.ToString("F2") + " fps";

                stopWatchFPS.Reset();
                stopWatchFPS.Start();

                actualFrameRate = (int)fps1;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            runtime++;
            TimeSpan timeRun = TimeSpan.FromSeconds(runtime);
            Log.WriteLine(timeRun.ToString("hh':'mm':'ss"));
        }
    }
}
