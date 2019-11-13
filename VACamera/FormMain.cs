using Accord.Video.DirectShow;
using Accord.Video.FFMPEG;
using System;
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
        VideoCaptureDeviceForm videoCaptureDeviceForm = null;

        VideoCaptureDevice videoDevice1 = null;
        Bitmap frame1 = null;
        static readonly Object syncFrame1 = new Object();
        bool isFrame1_Rendering = false;

        VideoCaptureDevice videoDevice2 = null;
        Bitmap frame2 = null;
        static readonly Object syncFrame2 = new Object();
        bool isFrame2_Rendering = false;

        //FilterInfoCollection audioDevices;
        //AudioDeviceInfo audioDeviceInfo;
        //AudioCaptureDevice audioDevice;

        Bitmap videoFrame = null;
        Graphics graphics = null;
        Font textFont = new Font("Tahoma", 20);
        Rectangle textPosition = new Rectangle(10, Settings.VideoHeight - 40, 200, 40);
        Rectangle textPosition1 = new Rectangle(10, 10, 200, 40);
        Rectangle textPosition2 = new Rectangle(10, 50, 200, 40);
        Rectangle textPosition3 = new Rectangle(10, 90, 200, 40);
        Rectangle textPosition4 = new Rectangle(10, 130, 200, 40);
        Rectangle textPosition5 = new Rectangle(10, 170, 200, 40);

        private Stopwatch stopWatchFPS = null;

        enum VideoFileState
        {
            IDLE,
            RECORDING,
            PAUSE
        }
        VideoFileState videoFileState = VideoFileState.IDLE;
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
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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

                    // debug
                    Console.WriteLine(sessionInfo.ToString());
                    Console.WriteLine(settings.ToString());
                }
                else
                {
                    Close();
                }
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            videoCaptureDeviceForm = new VideoCaptureDeviceForm();

            videoFrame = new Bitmap(Settings.VideoWidth, Settings.VideoHeight, PixelFormat.Format24bppRgb);
            graphics = Graphics.FromImage(videoFrame);
            graphics.CompositingMode = CompositingMode.SourceOver;

            //videoRender = new Thread(VideoRenderWorker);
            //videoRender.Start();

            InitDevices();

            timer1.Start();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopRecording();
            StopDevices();
            StopVideoFileWriter();
            //StopRender();

            timer1.Stop();
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

        private void VideoDevice1_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            bool freeToGo = false;
            lock (syncFrame1)
            {
                freeToGo = !isFrame1_Rendering;
            }
            if (freeToGo)
            {
                if (frame1 != null)
                {
                    frame1.Dispose();
                }
                frame1 = (Bitmap)eventArgs.Frame.Clone();
                RenderFrame();
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
                if (!isFrame2_Rendering)
                {
                    if (frame2 != null)
                    {
                        frame2.Dispose();
                    }
                    frame2 = (Bitmap)eventArgs.Frame.Clone();
                }
            }
        }

        private void RenderFrame()
        {
            if (frameSkipAt > 0)
            {
                frameCount++;
                if (frameCount == frameSkipAt)
                {
                    frameCount = 0;
                    return;
                }
            }

            lock (syncFrame1)
            {
                isFrame1_Rendering = true;
            }

            if (frame1 != null)
            {
                try
                {
                    using (Bitmap frame1_Clone = (Bitmap)frame1.Clone())
                    {
                        graphics.DrawImage(frame1_Clone,
                            new Rectangle(settings.Frame1_X, settings.Frame1_Y, settings.Frame1_Width, settings.Frame1_Height),
                            new Rectangle(0, 0, frame1.Width, frame1.Height),
                            GraphicsUnit.Pixel);
                    }
                }
                catch (Exception)
                {
                    //Console.WriteLine(e.ToString());
                    return;
                }
            }

            lock (syncFrame1)
            {
                isFrame1_Rendering = false;
            }

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
                        using (Bitmap frame2_Clone = (Bitmap)frame2.Clone())
                        {
                            graphics.DrawImage(frame2_Clone,
                            new Rectangle(settings.Frame2_X, settings.Frame2_Y, settings.Frame2_Width, settings.Frame2_Height),
                            new Rectangle(0, 0, frame2.Width, frame2.Height),
                            GraphicsUnit.Pixel);
                        }
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine(e.ToString());
                        return;
                    }
                }
                lock (syncFrame2)
                {
                    isFrame2_Rendering = false;
                }
            }

            // add text
            graphics.DrawString(DateTime.Now.ToString("HH:mm:ss"), textFont, Brushes.Red, textPosition);
            graphics.DrawString(sessionInfo.Name1, textFont, Brushes.Red, textPosition1);
            graphics.DrawString(sessionInfo.Name2, textFont, Brushes.Red, textPosition2);
            graphics.DrawString(sessionInfo.Name3, textFont, Brushes.Red, textPosition3);
            graphics.DrawString(sessionInfo.Name4, textFont, Brushes.Red, textPosition4);
            graphics.DrawString(sessionInfo.Name5, textFont, Brushes.Red, textPosition5);

            // show preview
            UpdateLiveImage(pictureFrame, (Bitmap)videoFrame.Clone());

            // write file
            if (videoFileState == VideoFileState.RECORDING)
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
                    catch (Exception)
                    {
                        //Console.WriteLine(e.ToString());
                        return;
                    }
                }
            }
        }

        //private void AudioDevice_NewFrame(object sender, NewFrameEventArgs e)
        //{
        //    //lock (syncRender)
        //    //{
        //    if (videoFileState == VideoFileState.RECORDING)
        //    {
        //        videoFileWriter.WriteAudioFrame(e.Signal.RawData);
        //    }
        //    //}
        //}

        private void StartRecording()
        {
            if (videoFileState == VideoFileState.IDLE)
            {
                VideoCodec videoCodec = VideoCodec.MPEG4;
                string videoExtension = ".mp4";

                if (settings.VideoOutputFormat == Settings.VideoFormat.MPEG2)
                {
                    videoCodec = VideoCodec.MPEG2;
                    videoExtension = ".mpg";
                }

                //if (audioDevice != null)
                //{
                //    //public void Open(string fileName, 
                //    //    int width, int height, Rational frameRate, VideoCodec codec, int bitRate, 
                //    //    AudioCodec audioCodec, int audioBitrate, int sampleRate, int channels);
                //    videoFileWriter.Open(System.Environment.CurrentDirectory + "\\" + sessionInfo.DateTime + "." + settings.GetVideoOutputFormat().ToString(),
                //        Settings.VideoWidth, Settings.VideoHeight, 30, VideoCodec.MPEG4, settings.BitRate,
                //        AudioCodec.MP3, 320 * 1000, audioDevice.SampleRate, (int)settings.GetAudioChannel());
                //    Console.WriteLine("HAS AUDIO");
                //}
                //else
                //{
                outputFile = outputFolder + "\\" + sessionInfo.DateTime + videoExtension;
                videoFileWriter.Open(
                   outputFile,
                   Settings.VideoWidth,
                   Settings.VideoHeight,
                   /* settings.FrameRate, */
                   actualFrameRate,
                   videoCodec,
                   settings.BitRate);
                //}

                Console.WriteLine(">>> START recording");
            }

            timerRecord.Start();
            videoFileState = VideoFileState.RECORDING;
        }

        private void PauseRecording()
        {
            lock (syncRender)
            {
                if (videoFileWriter.IsOpen)
                {
                    videoFileWriter.Flush();
                    Console.WriteLine(">>> PAUSE recording");
                }
            }

            timerRecord.Stop();
            videoFileState = VideoFileState.PAUSE;
        }

        private void StopRecording()
        {
            lock (syncRender)
            {
                if (videoFileWriter.IsOpen)
                {
                    videoFileWriter.Flush();
                    videoFileWriter.Close();
                    Console.WriteLine(">>> STOP recording");
                }
            }

            timerRecord.Stop();
            videoFileState = VideoFileState.IDLE;
        }

        private void StopDevices()
        {
            if (videoDevice1 != null)
            {
                videoDevice1.SignalToStop();
                videoDevice1.WaitForStop();
                videoDevice1.NewFrame -= VideoDevice1_NewFrame;

                Console.WriteLine(">>> STOP videoDevice1");
            }

            if (videoDevice2 != null)
            {
                videoDevice2.SignalToStop();
                videoDevice2.WaitForStop();
                videoDevice2.NewFrame -= VideoDevice2_NewFrame;

                Console.WriteLine(">>> STOP videoDevice2");
            }

            //if (audioDevice != null)
            //{
            //    audioDevice.SignalToStop();
            //    audioDevice.NewFrame -= AudioDevice_NewFrame;
            //    audioDevice.Dispose();
            //}

            timerFPS.Stop();
        }

        private void StopVideoFileWriter()
        {
            if (videoFileWriter != null)
            {
                if (videoFileWriter.IsOpen)
                {
                    videoFileWriter.Close();
                }
                videoFileWriter.Dispose();
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

        private void InitDevices()
        {
            StopDevices();

            // audio device
            //try
            //{
            //    audioDevices = new FilterInfoCollection(FilterCategory.AudioInputDevice);
            //    if (audioDevices != null && audioDevices.Count > 0)
            //    {
            //        string deviceMonikerString = "";

            //        foreach (FilterInfo device in audioDevices)
            //        {

            //            if (settings.AudioInputPath.Equals(device.Name))
            //            {
            //                deviceMonikerString = device.MonikerString;
            //            }
            //        }

            //        if (deviceMonikerString.Equals(""))
            //        {
            //            deviceMonikerString = audioDevices[0].MonikerString;
            //        }

            //        // open
            //        Console.WriteLine("::: AudioDevice = " + deviceMonikerString);
            //        //audioDevice = new AudioCaptureDevice(audioDeviceInfo.Guid);
            //        //audioDevice.Format = SampleFormat.Format16Bit;
            //        //audioDevice.DesiredFrameSize = 4096;
            //        //audioDevice.NewFrame += AudioDevice_NewFrame; ;
            //        //audioDevice.Start();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //}

            // audio channels
            //if (audioDevice != null)
            //{
            //    if (settings.GetAudioChannel() == Settings.AudioMode.Mono)
            //    {
            //        audioDevice.Channels = 1;
            //    }
            //    else if (settings.GetAudioChannel() == Settings.AudioMode.Stereo)
            //    {
            //        audioDevice.Channels = 2;
            //    }
            //    else
            //    {
            //        audioDevice.Channels = 2;
            //    }
            //}

            // video devices
            string device1_MonikerString = "";
            string device2_MonikerString = "";
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices != null && videoDevices.Count > 0)
                {
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

                    if (device1_MonikerString.Equals(""))
                    {
                        device1_MonikerString = videoDevices[0].MonikerString;
                    }
                    Console.WriteLine("::: VideoDevice1 = " + device1_MonikerString);
                    videoDevice1 = new VideoCaptureDevice(device1_MonikerString);
                    videoDevice1.DesiredAverageTimePerFrame = 330000; // ns
                    videoDevice1.NewFrame += VideoDevice1_NewFrame;
                    videoDevice1.Start();

                    if (videoDevices.Count >= 2)
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
                        Console.WriteLine("::: VideoDevice2 = " + device2_MonikerString);
                        videoDevice2 = new VideoCaptureDevice(device2_MonikerString);
                        videoDevice2.DesiredAverageTimePerFrame = 330000; // ns
                        videoDevice2.NewFrame += VideoDevice2_NewFrame;
                        videoDevice2.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            if (videoDevice1 != null)
            {
                // set buttons state
                btnRecord.Enabled = true;
                btnPause.Enabled = false;
                btnStop.Enabled = false;
                btnReplay.Enabled = false;
                btnWriteDisk.Enabled = false;

                // reset record time
                UpdateRecordTime(0);
            }

            if (settings.FrameRate == 24)
            {
                frameSkipAt = 5;
                frameCount = 0;
            }
            else if (settings.FrameRate == 15)
            {
                frameSkipAt = 2;
                frameCount = 0;
            }
            else
            {
                frameSkipAt = 0;
                frameCount = 0;
            }

            stopWatchFPS = null;
            timerFPS.Start();

            Console.WriteLine(settings.ToString());
        }

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
                    Console.WriteLine(sessionInfo.ToString());
                    Console.WriteLine(settings.ToString());
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
            if (videoFileState == VideoFileState.IDLE
                || videoFileState == VideoFileState.PAUSE)
            {
                btnRecord.Enabled = false;
                btnPause.Enabled = true;
                btnStop.Enabled = true;
                btnReplay.Enabled = false;
                btnWriteDisk.Enabled = false;

                settingsToolStripMenuItem.Enabled = false; // do not change settings during recording

                StartRecording();
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (videoFileState == VideoFileState.RECORDING)
            {
                videoFileState = VideoFileState.PAUSE;
                btnRecord.Enabled = true;
                btnPause.Enabled = false;
                btnStop.Enabled = true;
                btnReplay.Enabled = false;
                btnWriteDisk.Enabled = false;
                PauseRecording();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (videoFileState == VideoFileState.RECORDING
                || videoFileState == VideoFileState.PAUSE)
            {
                if (MessageBox.Show("Kết thúc ghi hình và ghi DVD?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    videoFileState = VideoFileState.IDLE;
                    btnRecord.Enabled = false; // must start new session
                    btnPause.Enabled = false;
                    btnStop.Enabled = false;
                    btnReplay.Enabled = true;
                    btnWriteDisk.Enabled = true;

                    settingsToolStripMenuItem.Enabled = true; // can change settings again

                    StopRecording();
                    Thread.Sleep(1000);

                    StopDevices();
                    Thread.Sleep(1000);

                    btnWriteDisk_Click(new object(), new EventArgs());
                }
            }
        }

        private void btnReplay_Click(object sender, EventArgs e)
        {

        }

        private void btnWriteDisk_Click(object sender, EventArgs e)
        {
            Console.WriteLine(sessionInfo.ToString());
            Console.WriteLine("outputFile = " + outputFile);
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
            Console.WriteLine(timeRun.ToString("hh':'mm':'ss"));
        }
    }
}
