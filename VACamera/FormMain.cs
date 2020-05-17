#define USE_SLOW_PC // do not write log on slow PC

#define USE_DIRECT_MEMORY_ACCESS // for faster access to large image, use unsafe pointer to access bitmap data directly in memory

#if USE_DIRECT_MEMORY_ACCESS // below settings are available only if USE_DIRECT_MEMORY_ACCESS enabled
    #define USE_PINNED_MEMORY_BITMAP // using pinned memory will allocate fixed amount of memory for final bitmap, no need to decompress bitmap multiple times
    #define USE_DOUBLE_BUFFER // while attaching a bitmap to picturebox, double buffer ensures that picture only show image when image is completely filled
#endif

/*

Time to render 1 video frame

FrameSize: 1280x720
Mode    Single      Side2Side       Overlay
GDI+    3.7 ms      7   ms          5.4 ms
Direct  2.6 ms      3.9 ms          3.6 ms

FrameSize: Dynamic, auto get framesize 1280x720, 640x480, 320x240
Mode    Single      Side2Side       Overlay
GDI+    3.7 ms      7   ms          5.4 ms
Direct  2.6 ms      2.7 ms          2.9 ms
Pinned  2.5 ms      1.7 ms          2.5 ms

---

Time to write 1 video frame

Write H264
Mode        Single      Side2Side       Overlay
CPU         7.5 ms      7.7 ms          7.8 ms
GPU         6.6 ms      6.6 ms          6.6 ms
GPU+Pinned  2.8 ms      2.0 ms          3.0 ms    << Max FPS = 1000/3 = 333 fps

*/

using Accord.DirectSound;
using Accord.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VACamera.Properties;

namespace VACamera
{
    public partial class FormMain : Form
    {
        enum VideoRecordState
        {
            IDLE,
            RECORDING
        }

        VideoRecordState videoRecordState = VideoRecordState.IDLE;
        Process ffmpeg;
        Stream ffmpegInputStream;
        Thread videoRenderThread = null;
        int recordTime = 0;
        int recordPart = 0;
        static readonly Object syncRender = new Object();

        string outputFolder = "C:\\records";
        string recordTxt = "C:\\records\\records.txt";
        string outputFile = "";
        string replayFile = "";
        string videoExtension = ".mp4";

        SessionInfo sessionInfo = new SessionInfo();
        Settings settings = new Settings();

        List<AudioDeviceInfo> audioDevices = null;
        string audioDeviceName = "";

        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoDevice1 = null;
        VideoCaptureDevice videoDevice2 = null;
        bool isPreviewing = true;

        Bitmap frame1 = null;
        static readonly Object syncFrame1 = new Object();

        Bitmap frame2 = null;
        static readonly Object syncFrame2 = new Object();

#if USE_PINNED_MEMORY_BITMAP
        byte[] _videoFramePixels;
        IntPtr _videoFrameFirstPixelAddr;
        int _videoFrameStride;
#if USE_DOUBLE_BUFFER
        byte[] _videoFramePixels2;
        IntPtr _videoFrameFirstPixelAddr2;
        Bitmap _videoFrame2 = null;
        bool _isFilledBackBuffer = false;
#endif
#endif
        Bitmap _videoFrame = null;
        Graphics _graphics = null;

        Stopwatch stopWatch = new Stopwatch();
#if !USE_SLOW_PC
        int renderCount = 0;
        double totalRenderTime = 0;
#endif

        Font textFont = new Font("Tahoma", 18);
        StringFormat textDirectionRTL = new StringFormat(StringFormatFlags.DirectionRightToLeft);
        Rectangle textLine1 = new Rectangle(10, 10, Settings.VideoWidth - 20, 40);
        Rectangle textLine2 = new Rectangle(10, Settings.VideoHeight - 10 - 40 - 40, Settings.VideoWidth - 20, 40);
        Rectangle textLine3 = new Rectangle(10, Settings.VideoHeight - 10 - 40, Settings.VideoWidth - 20, 40);

        SoundPlayer beepPlayer = new SoundPlayer(Resources.beep);

        internal const int CTRL_C_EVENT = 0;
        [DllImport("kernel32.dll")]
        internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);

        // Delegate type to be used as the Handler Routine for SCCH
        delegate bool ConsoleCtrlDelegate(uint CtrlType);

        [DllImport("msvcrt.dll", SetLastError = false)]
        static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);

        public FormMain()
        {
            InitializeComponent();
            this.KeyPreview = true;
            var fi = new FileInfo(Application.ExecutablePath);
            Directory.SetCurrentDirectory(fi.DirectoryName);
            Log.WriteLine("DIR NAME = " + fi.DirectoryName);
            try
            {
                Directory.CreateDirectory(outputFolder);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

#if USE_DIRECT_MEMORY_ACCESS && USE_PINNED_MEMORY_BITMAP
            Log.WriteLine("Running with USE_DIRECT_MEMORY_ACCESS & USE_PINNED_MEMORY_BITMAP");
#elif USE_PINNED_MEMORY_BITMAP
            Log.WriteLine("Running with USE_PINNED_MEMORY_BITMAP");
#elif USE_DIRECT_MEMORY_ACCESS
            Log.WriteLine("Running with USE_DIRECT_MEMORY_ACCESS");
#else
            Log.WriteLine("Running with GDI+");
#endif
            Hide();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // disable buttons for new session
            btnRecord.Enabled = false;
            btnPause.Enabled = false;
            btnStop.Enabled = false;
            btnReplay.Enabled = false;
            btnWriteDisk.Enabled = false;

            // init bitmap buffer
#if USE_PINNED_MEMORY_BITMAP
            int pixelFormatSize = Image.GetPixelFormatSize(PixelFormat.Format24bppRgb) / 8;
            _videoFrameStride = Settings.VideoWidth * pixelFormatSize;

            // pin memory
            _videoFramePixels = new byte[_videoFrameStride * Settings.VideoHeight];
            GCHandle.Alloc(_videoFramePixels, GCHandleType.Pinned);
            _videoFrameFirstPixelAddr = Marshal.UnsafeAddrOfPinnedArrayElement(_videoFramePixels, 0);
            _videoFrame = new Bitmap(Settings.VideoWidth, Settings.VideoHeight, _videoFrameStride, PixelFormat.Format24bppRgb, _videoFrameFirstPixelAddr);

#if USE_DOUBLE_BUFFER
            // pin memory in back-buffer
            _videoFramePixels2 = new byte[_videoFrameStride * Settings.VideoHeight];
            GCHandle.Alloc(_videoFramePixels2, GCHandleType.Pinned);
            _videoFrameFirstPixelAddr2 = Marshal.UnsafeAddrOfPinnedArrayElement(_videoFramePixels2, 0);
            _videoFrame2 = new Bitmap(Settings.VideoWidth, Settings.VideoHeight, _videoFrameStride, PixelFormat.Format24bppRgb, _videoFrameFirstPixelAddr2);
            // binding preview to back-buffer
            pictureFrame.Image = _videoFrame2;
            pictureFrame.Invalidated += PictureFrame_Invalidated;
#else
            // binding preview
            pictureFrame.Image = _videoFrame;
#endif

#else
            _videoFrame = new Bitmap(Settings.VideoWidth, Settings.VideoHeight, PixelFormat.Format24bppRgb);
#endif
            // init grapics
            _graphics = Graphics.FromImage(_videoFrame);
#if USE_DIRECT_MEMORY_ACCESS
            _graphics.CompositingMode = CompositingMode.SourceOver;
#else
            _graphics.CompositingMode = CompositingMode.SourceCopy;
#endif
            _graphics.CompositingQuality = CompositingQuality.HighSpeed;
            _graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            _graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            _graphics.PixelOffsetMode = PixelOffsetMode.None;
            _graphics.SmoothingMode = SmoothingMode.None;

            // show new session
            using (FormNewSession formNewSession = new FormNewSession())
            {
                if (formNewSession.ShowDialog(this) == DialogResult.OK)
                {
                    sessionInfo = formNewSession.SessionInfo;
                    Log.WriteLine(sessionInfo.ToString());
                    Show();

                    PrepareRecord();

                    // show setting if it's the first time
                    if (!File.Exists(Environment.CurrentDirectory + "\\Settings.ini"))
                    {
                        using (FormSettings formSettings = new FormSettings())
                        {
                            formSettings.ShowDialog();
                            settings = formSettings.Settings;
                            settings.SaveSettings();
                            Log.WriteLine(settings.ToString());
                        }
                    }
                }
                else
                {
                    Close();
                }
            }
        }

        private void PictureFrame_Invalidated(object sender, InvalidateEventArgs e)
        {
            if(_isFilledBackBuffer)
            {
                _isFilledBackBuffer = false;
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            // init render thread
            videoRenderThread = new Thread(VideoRenderWorker);
            videoRenderThread.Start();

            // start devices at startup
            InitDevices();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoRecordState != VideoRecordState.IDLE)
            {
                StopRecording(false);
            }
            StopDevices();
            StopRender();
            //Delete all file when exit
            DirectoryInfo di = new DirectoryInfo(outputFolder);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            RunCommand("shutdown -s -t 0");
        }

        private void VideoRenderWorker()
        {
            while (true)
            {
                if (isPreviewing)
                {
                    //Task.Factory.StartNew(() =>
                    //{
                    RenderVideoFrame();
                    //});
                }
            }
        }

        private void StopRender()
        {
            try
            {
                if (videoRenderThread != null)
                {
                    if (videoRenderThread.IsAlive)
                    {
                        videoRenderThread.Abort();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }
        }

        private VideoCapabilities selectResolution(VideoCaptureDevice device, int width, int height, int fps)
        {
            Log.WriteLine("Looking for: " + width + "x" + height + " @ " + fps);

            int[] bias = new int[device.VideoCapabilities.Length];

            int i = 0;
            foreach (VideoCapabilities capability in device.VideoCapabilities)
            {
                bias[i] = (width - capability.FrameSize.Width) * (width - capability.FrameSize.Width) +
                          (height - capability.FrameSize.Height) * (height - capability.FrameSize.Height) +
                          (fps - capability.AverageFrameRate) * (fps - capability.AverageFrameRate);
                i++;
            }

            int selectedIndex = GetMinIndex(bias);
            Log.WriteLine("selectedIndex = " + selectedIndex);
            return device.VideoCapabilities[selectedIndex];
        }

        private int GetMinIndex(int[] array)
        {
            if (array.Length == 0)
            {
                return -1;
            }
            else
            {
                int i = 0;
                int min = array[0];
                for (int x = 1; x < array.Length; x++)
                {
                    if (array[x] < min)
                    {
                        i = x;
                        min = array[x];
                    }
                }

                return i;
            }
        }

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
                        Log.WriteLine("Checking: " + device.Description + "|" + device.Guid.ToString());

                        // ffmpeg do not handle virtual primary audio device 
                        if (device.Guid.ToString().Equals("00000000-0000-0000-0000-000000000000"))
                        {
                            continue;
                        }

                        if (settings.AudioInputPath.Equals(/* device.Description + "|" + */ device.Guid.ToString()))
                        {
                            audioDeviceInfo = device;
                        }
                    }

                    // if nothing matched, show dialog
                    if (audioDeviceInfo == null)
                    {
                        MessageBox.Show("Không tìm thấy thiết bị thu âm" +
                            (settings.AudioInputPath.Equals("") ? "" : ": " + settings.AudioInputPath.Split(new char[] { '|' })[0]) +
                            "." + Environment.NewLine + "Hãy chọn thiết bị khác ở mục Cài Đặt.",
                            "Microphone");
                    }
                    else
                    {
                        audioDeviceName = audioDeviceInfo.Description;
                        Log.WriteLine(">>> START audioDevice: " + audioDeviceInfo.Description + "|" + audioDeviceInfo.Guid.ToString());
                    }
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
            string device1_Name = "";
            string device1_MonikerString = "";
            string device2_Name = "";
            string device2_MonikerString = "";
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices != null && videoDevices.Count > 0)
                {
                    // find last used devices
                    foreach (FilterInfo device in videoDevices)
                    {
                        if (settings.Camera1_InputPath.Equals(/* device.Name + "|" + */ device.MonikerString))
                        {
                            device1_Name = device.Name;
                            device1_MonikerString = device.MonikerString;
                        }

                        if (settings.Camera2_InputPath.Equals(/* device.Name + "|" + */ device.MonikerString))
                        {
                            device2_Name = device.Name;
                            device2_MonikerString = device.MonikerString;
                        }
                    }

                    // not matched, use default device
                    if (device1_Name.Equals(""))
                    {
                        MessageBox.Show("Không tìm thấy camera" +
                            (settings.Camera1_InputPath.Equals("") ? "" : ": " + settings.Camera1_InputPath.Split(new char[] { '|' })[0]) +
                            "." + Environment.NewLine +
                            "Phần mêm đã tự động chọn: " + videoDevices[0].Name + "." + Environment.NewLine +
                            "Để chọn thiết bị khác, vui lòng vào Cài Đặt.",
                            "Camera 1");
                        device1_Name = videoDevices[0].Name;
                        device1_MonikerString = videoDevices[0].MonikerString;
                    }

                    videoDevice1 = new VideoCaptureDevice(device1_MonikerString);
                    Log.WriteLine("videoDevice1: " + device1_Name + "|" + device1_MonikerString);
                    for (int i = 0; i < videoDevice1.VideoCapabilities.Length; i++)
                    {
                        Log.WriteLine("[" + i + "]" +
                            videoDevice1.VideoCapabilities[i].FrameSize.ToString() + " @ " +
                            videoDevice1.VideoCapabilities[i].AverageFrameRate.ToString());
                    }

                    if (settings.VideoMixingMode == Settings.VideoMode.Single
                        || settings.VideoMixingMode == Settings.VideoMode.Overlay)
                    {
                        videoDevice1.VideoResolution = selectResolution(videoDevice1, Settings.VideoWidth, Settings.VideoHeight, settings.VideoFrameRate);
                    }
                    else if (settings.VideoMixingMode == Settings.VideoMode.SideBySide)
                    {
                        videoDevice1.VideoResolution = selectResolution(videoDevice1, Settings.VideoWidth / 2, Settings.VideoHeight / 2, settings.VideoFrameRate);
                    }

                    //videoDevice1.DesiredAverageTimePerFrame = 330000; // ns
                    videoDevice1.NewFrame += VideoDevice1_NewFrame;
                    videoDevice1.Start();
                    Log.WriteLine(">>> START videoDevice1: " + device1_Name + "|" + device1_MonikerString);

                    if (settings.VideoMixingMode != Settings.VideoMode.Single)
                    {
                        // open second device if needed
                        if (videoDevices.Count > 1)
                        {
                            if (device2_Name.Equals(""))
                            {
                                MessageBox.Show("Không tìm thấy camera" +
                                (settings.Camera2_InputPath.Equals("") ? "" : ": " + settings.Camera2_InputPath.Split(new char[] { '|' })[0]) +
                                "." + Environment.NewLine +
                                "Phần mêm đã tự động chọn: " + videoDevices[1].Name + "." + Environment.NewLine +
                                "Để chọn thiết bị khác, vui lòng vào Cài Đặt.",
                                "Camera 2");
                                device2_Name = videoDevices[1].Name;
                                device2_MonikerString = videoDevices[1].MonikerString;
                            }
                        }
                        else
                        {
                            // fallback to Single mode
                            settings.SetVideoMixingMode(Settings.VideoMode.Single);
                        }

                        videoDevice2 = new VideoCaptureDevice(device2_MonikerString);
                        Log.WriteLine("videoDevice2: " + device2_Name + "|" + device2_MonikerString);
                        for (int i = 0; i < videoDevice2.VideoCapabilities.Length; i++)
                        {
                            Log.WriteLine("[" + i + "]" +
                                videoDevice2.VideoCapabilities[i].FrameSize.ToString() + " @ " +
                                videoDevice2.VideoCapabilities[i].AverageFrameRate.ToString());
                        }

                        if (settings.VideoMixingMode == Settings.VideoMode.SideBySide)
                        {
                            videoDevice2.VideoResolution = selectResolution(videoDevice2, Settings.VideoWidth / 2, Settings.VideoHeight / 2, settings.VideoFrameRate);
                        }
                        else if (settings.VideoMixingMode == Settings.VideoMode.Overlay)
                        {
                            videoDevice2.VideoResolution = selectResolution(videoDevice2, Settings.VideoWidth / 4, Settings.VideoHeight / 4, settings.VideoFrameRate);
                        }

                        //videoDevice2.DesiredAverageTimePerFrame = 330000; // ns
                        videoDevice2.NewFrame += VideoDevice2_NewFrame;
                        videoDevice2.Start();
                        Log.WriteLine(">>> START videoDevice2: " + device2_Name + "|" + device2_MonikerString);
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

                isPreviewing = true;
            }

            Log.WriteLine(settings.ToString());
        }

        private void PausePreview()
        {
            if (videoDevice1 != null)
            {
                videoDevice1.NewFrame -= VideoDevice1_NewFrame;
            }
            if (videoDevice2 != null)
            {
                videoDevice2.NewFrame -= VideoDevice2_NewFrame;
            }
            isPreviewing = false;
        }

        private void ResumePreview()
        {
            if (isDevicesStopped())
            {
                InitDevices();
            }
            else
            {
                if (videoDevice1 != null)
                {
                    videoDevice1.NewFrame += VideoDevice1_NewFrame;
                }
                if (videoDevice2 != null)
                {
                    videoDevice2.NewFrame += VideoDevice2_NewFrame;
                }
            }
            isPreviewing = true;
        }

        private void StopDevices()
        {
            PausePreview();

            try
            {
                if (videoDevice1 != null)
                {
                    Log.WriteLine(">>> STOP videoDevice1: start");
                    videoDevice1.NewFrame -= VideoDevice1_NewFrame;
                    videoDevice1.SignalToStop();
                    Thread.Sleep(1000);

                    videoDevice1 = null;
                    Log.WriteLine(">>> STOP videoDevice1: done");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                videoDevice1 = null;
            }

            try
            {
                if (videoDevice2 != null)
                {
                    Log.WriteLine(">>> STOP videoDevice2: start");
                    videoDevice2.NewFrame -= VideoDevice2_NewFrame;
                    videoDevice2.SignalToStop();
                    Thread.Sleep(1000);

                    videoDevice2 = null;
                    Log.WriteLine(">>> STOP videoDevice2: done");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                videoDevice2 = null;
            }
        }

        private bool isDevicesStopped()
        {
            return ((videoDevice1 != null) || (videoDevice2 != null));
        }

#if !USE_PINNED_MEMORY_BITMAP
        delegate void UpdateLiveImageInvokerCallBack(PictureBox pictureBox, Bitmap bitmap);

        private void UpdateLiveImageInvoker(PictureBox pictureBox, Bitmap bitmap)
        {
            try
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
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }
        }

        private void UpdateLiveImage(PictureBox pictureBox, Bitmap bitmap)
        {
            try
            {
                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                }
                pictureBox.Image = bitmap;
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }
        }
#endif

        private void VideoDevice1_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            // WHY DOESN'T CAMERA REPORT FRAME DURATION? :(
            //if (true) // debug
            //{
            //    Log.WriteLine("CaptureStarted = " + eventArgs.CaptureStarted.ToString());
            //    Log.WriteLine("CaptureFinished = " + eventArgs.CaptureFinished.ToString());
            //    Log.WriteLine("FrameIndex = " + eventArgs.FrameIndex);
            //}

            lock (syncFrame1)
            {
                try
                {
                    if (frame1 != null)
                    {
                        frame1.Dispose();
                    }
                    frame1 = (Bitmap)eventArgs.Frame.Clone();
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.ToString());
                }
                //Log.WriteLine("Frame 1: " + frame1.Width + "x" + frame1.Height);
            }
        }

        private void VideoDevice2_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            lock (syncFrame2)
            {
                try
                {
                    if (frame2 != null)
                    {
                        frame2.Dispose();
                    }
                    frame2 = (Bitmap)eventArgs.Frame.Clone();
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.ToString());
                }
                //Log.WriteLine("Frame 2: " + frame2.Width + "x" + frame2.Height);
            }
        }

        private void RenderVideoFrame()
        {
            stopWatch.Reset();
            stopWatch.Start();
#if !USE_SLOW_PC
            renderCount++;
#endif

#if USE_DIRECT_MEMORY_ACCESS
            // use unsafe and parallel
            unsafe
            {
#if USE_PINNED_MEMORY_BITMAP
                byte* ptrFirstPixel_videoFrame = (byte*)_videoFrameFirstPixelAddr;
#else
                // lock video frame into memory
                BitmapData bitmapData_videoFrame = _videoFrame.LockBits(
                    new Rectangle(0, 0, _videoFrame.Width, _videoFrame.Height),
                    ImageLockMode.WriteOnly,
                    _videoFrame.PixelFormat);
                byte* ptrFirstPixel_videoFrame = (byte*)bitmapData_videoFrame.Scan0;
#endif

#else
                _graphics.CompositingMode = CompositingMode.SourceCopy;
#endif
                // render frame1 which comes from videoDevive1's thread, we need to lock it
                Bitmap _frame1 = null;
                lock (syncFrame1)
                {
                    try
                    {
                        if (frame1 != null)
                        {
                            _frame1 = (Bitmap)frame1.Clone();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                    }
                }

                if (_frame1 != null)
                {
                    try
                    {
#if USE_DIRECT_MEMORY_ACCESS
                        // get raw data of frame 1
                        BitmapData bitmapData_frame1 = _frame1.LockBits(
                            new Rectangle(0, 0, _frame1.Width, _frame1.Height),
                            ImageLockMode.ReadOnly,
                            _frame1.PixelFormat);
                        byte* ptrFirstPixel_frame1 = (byte*)bitmapData_frame1.Scan0;

                        if ((settings.VideoMixingMode == Settings.VideoMode.Single
                            || settings.VideoMixingMode == Settings.VideoMode.Overlay)
                            && _frame1.Width == videoDevice1.VideoResolution.FrameSize.Width
                            && _frame1.Height == videoDevice1.VideoResolution.FrameSize.Height)
                        {
                            // copy all pixels of frame 1 to video frame
                            Parallel.For(0, Settings.VideoHeight, y =>
                            {
#if USE_PINNED_MEMORY_BITMAP
                                byte* currentLine_videoFrame = ptrFirstPixel_videoFrame + (y * _videoFrameStride);
#else
                                byte* currentLine_videoFrame = ptrFirstPixel_videoFrame + (y * bitmapData_videoFrame.Stride);
#endif
                                byte* currentLine_frame1 = ptrFirstPixel_frame1 + (y * bitmapData_frame1.Stride);
#if USE_PINNED_MEMORY_BITMAP
                                for (int x = 0; x < _videoFrameStride; x++)
#else
                                for (int x = 0; x < bitmapData_videoFrame.Stride; x++)
#endif
                                {
                                    currentLine_videoFrame[x] = currentLine_frame1[x];
                                };
                            });
                        }
                        else if (settings.VideoMixingMode == Settings.VideoMode.SideBySide
                            && _frame1.Width == videoDevice1.VideoResolution.FrameSize.Width
                            && _frame1.Height == videoDevice1.VideoResolution.FrameSize.Height)
                        {
                            // copy a half of pixels of frame 1 to video frame
                            Parallel.For(0, Settings.VideoHeight, y =>
                            {
#if USE_PINNED_MEMORY_BITMAP
                                byte* currentLine_videoFrame = ptrFirstPixel_videoFrame + (y * _videoFrameStride);
#else
                                byte* currentLine_videoFrame = ptrFirstPixel_videoFrame + (y * bitmapData_videoFrame.Stride);
#endif
                                byte* currentLine_frame1 = ptrFirstPixel_frame1 + ((y * videoDevice1.VideoResolution.FrameSize.Height / Settings.VideoHeight) * bitmapData_frame1.Stride);
                                int u = 0;
                                int v = 0;
                                for (int x = 0; x < Settings.VideoWidth / 2; x++)
                                {
                                    currentLine_videoFrame[u++] = currentLine_frame1[v++];
                                    currentLine_videoFrame[u++] = currentLine_frame1[v++];
                                    currentLine_videoFrame[u++] = currentLine_frame1[v++];
                                };
                            });
                        }
                        _frame1.UnlockBits(bitmapData_frame1);
#else
                    _graphics.DrawImage(_frame1, settings.Frame1_X, settings.Frame1_Y, settings.Frame1_Width, settings.Frame1_Height);
#endif
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                    }
                    finally
                    {
                        _frame1.Dispose();
                    }
                }

                if (settings.VideoMixingMode != Settings.VideoMode.Single)
                {
                    // render frame2 which comes from videoDevive2's thread, we need to lock it
                    Bitmap _frame2 = null;
                    lock (syncFrame2)
                    {
                        try
                        {
                            if (frame2 != null)
                            {
                                _frame2 = (Bitmap)frame2.Clone();
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLine(ex.ToString());
                        }
                    }

                    if (_frame2 != null)
                    {
                        try
                        {
#if USE_DIRECT_MEMORY_ACCESS
                            // get raw data of frame 2
                            BitmapData bitmapData_frame2 = _frame2.LockBits(
                                new Rectangle(0, 0, _frame2.Width, _frame2.Height),
                                ImageLockMode.ReadOnly,
                                _frame2.PixelFormat);
                            byte* ptrFirstPixel_frame2 = (byte*)bitmapData_frame2.Scan0;

                            if (settings.VideoMixingMode == Settings.VideoMode.SideBySide
                                && _frame2.Width == videoDevice2.VideoResolution.FrameSize.Width
                                && _frame2.Height == videoDevice2.VideoResolution.FrameSize.Height)
                            {
                                // copy a half of pixels of frame 2 to video frame
                                Parallel.For(0, Settings.VideoHeight, y =>
                                {
#if USE_PINNED_MEMORY_BITMAP
                                    byte* currentLine_videoFrame = ptrFirstPixel_videoFrame + (y * _videoFrameStride);
#else
                                    byte* currentLine_videoFrame = ptrFirstPixel_videoFrame + (y * bitmapData_videoFrame.Stride);
#endif
                                    byte* currentLine_frame2 = ptrFirstPixel_frame2 + ((y * videoDevice2.VideoResolution.FrameSize.Height / Settings.VideoHeight) * bitmapData_frame2.Stride);
                                    int u = (Settings.VideoWidth / 2) * 3;
                                    int v = 0;
                                    for (int x = 0; x < Settings.VideoWidth / 2; x++)
                                    {
                                        currentLine_videoFrame[u++] = currentLine_frame2[v++];
                                        currentLine_videoFrame[u++] = currentLine_frame2[v++];
                                        currentLine_videoFrame[u++] = currentLine_frame2[v++];
                                    };
                                });
                            }
                            else if (settings.VideoMixingMode == Settings.VideoMode.Overlay
                                && _frame2.Width == videoDevice2.VideoResolution.FrameSize.Width
                                && _frame2.Height == videoDevice2.VideoResolution.FrameSize.Height)
                            {
                                // start copy at overlay position only
                                Parallel.For(0, videoDevice2.VideoResolution.FrameSize.Height, y =>
                                {
#if USE_PINNED_MEMORY_BITMAP
                                    byte* currentLine_videoFrame = ptrFirstPixel_videoFrame + ((Settings.VideoHeight - videoDevice2.VideoResolution.FrameSize.Height - 10 + y) * _videoFrameStride);
#else
                                    byte* currentLine_videoFrame = ptrFirstPixel_videoFrame + ((Settings.VideoHeight - videoDevice2.VideoResolution.FrameSize.Height - 10 + y) * bitmapData_videoFrame.Stride);
#endif
                                    byte* currentLine_frame2 = ptrFirstPixel_frame2 + (y * bitmapData_frame2.Stride);
                                    int u = (Settings.VideoWidth - videoDevice2.VideoResolution.FrameSize.Width - 10) * 3;
                                    int v = 0;
                                    for (int x = 0; x < videoDevice2.VideoResolution.FrameSize.Width; x++)
                                    {
                                        currentLine_videoFrame[u++] = currentLine_frame2[v++];
                                        currentLine_videoFrame[u++] = currentLine_frame2[v++];
                                        currentLine_videoFrame[u++] = currentLine_frame2[v++];
                                    };
                                });
                            }
                            _frame2.UnlockBits(bitmapData_frame2);
#else
                        _graphics.DrawImage(_frame2, settings.Frame2_X, settings.Frame2_Y, settings.Frame2_Width, settings.Frame2_Height);
#endif
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLine(ex.ToString());
                        }
                        finally
                        {
                            _frame2.Dispose();
                        }
                    }
                }
#if USE_DIRECT_MEMORY_ACCESS
#if !USE_PINNED_MEMORY_BITMAP
                _videoFrame.UnlockBits(bitmapData_videoFrame);
#endif
            }
#endif
            // add text
#if !USE_DIRECT_MEMORY_ACCESS
            _graphics.CompositingMode = CompositingMode.SourceOver;
#endif
            try
            {
                // TopLeft: Name1
                _graphics.DrawString(sessionInfo.Name1, textFont, Brushes.Red, textLine1);
                // TopRight: Name 2
                _graphics.DrawString(sessionInfo.Name2, textFont, Brushes.Red, textLine1, textDirectionRTL);
                // BottomLeft: Name 3, Name 4
                _graphics.DrawString(sessionInfo.Name3, textFont, Brushes.Red, textLine2);
                _graphics.DrawString(sessionInfo.Name4, textFont, Brushes.Red, textLine3);
                // BottomRight: Name 5 and DateTime
                _graphics.DrawString(sessionInfo.Name5, textFont, Brushes.Red, textLine2, textDirectionRTL);
                _graphics.DrawString(DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), textFont, Brushes.Red, textLine3, textDirectionRTL);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            // write frame if needed
            if (videoRecordState == VideoRecordState.RECORDING)
            {
                //Task.Factory.StartNew(() =>
                //{
                lock (syncRender)
                {
                    try
                    {
                        // still need to optimize, because it takes time to write to memory stream
#if USE_PINNED_MEMORY_BITMAP
                        ffmpegInputStream.Write(_videoFramePixels, 0, 2764800 /* 1280x720x3 */);
#else
                        using (var ms = new MemoryStream())
                        {
                            _videoFrame.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                            ms.WriteTo(ffmpegInputStream);
                        }
#endif
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                    }
                }
                //});
            }

            // show preview in UI thread, need to clone it, and then MUST invoke the UI thread
            if (pictureFrame != null)
            {
#if USE_PINNED_MEMORY_BITMAP
#if USE_DOUBLE_BUFFER
                /*
                if (_isFilledBackBuffer)
                {
                    pictureFrame.Invalidate();
                }
                else
                {
                    memcpy(_videoFrameFirstPixelAddr2, _videoFrameFirstPixelAddr, _videoFrameStride * Settings.VideoHeight);
                }
                _isFilledBackBuffer = !_isFilledBackBuffer;
                */

                if (!_isFilledBackBuffer)
                {
                    memcpy(_videoFrameFirstPixelAddr2, _videoFrameFirstPixelAddr, _videoFrameStride * Settings.VideoHeight);
                    _isFilledBackBuffer = true;
                    pictureFrame.Invalidate();
                }
#else
                pictureFrame.Invalidate();
#endif
#else
                UpdateLiveImageInvoker(pictureFrame, (Bitmap)_videoFrame.Clone());
#endif
            }

            stopWatch.Stop();
            int timeLeft = settings.VideoFrameDuration - (int)stopWatch.Elapsed.TotalMilliseconds;

#if !USE_SLOW_PC
            totalRenderTime += stopWatch.Elapsed.TotalMilliseconds;
            if (renderCount == 100)
            {
                double average_frame_time = totalRenderTime / renderCount;
                Log.WriteLine(renderCount + " frames in average " + average_frame_time + " ms");
                renderCount = 0;
                totalRenderTime = 0;
            }
#endif
            if (timeLeft > 0)
            {
                Thread.Sleep(timeLeft);
            }
        }

        private void RunCommand(string command)
        {
            Log.WriteLine(command);

            Process process = new Process();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C " + command;
            process.Start();
            process.WaitForExit();
        }

        private void AddFileToRecordList(string file, bool reset)
        {
            string command = "echo file '" + file + "' " + (reset ? ">" : ">>") + recordTxt;
            RunCommand(command);
        }

        private void PrepareRecord()
        {
            recordPart = 0;
            try
            {
                File.Delete(recordTxt);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }
        }

        private void StartRecording()
        {
            if (videoRecordState == VideoRecordState.IDLE)
            {
                videoExtension = ".mp4";

                if (settings.VideoOutputFormat == Settings.VideoFormat.MPEG2)
                {
                    videoExtension = ".mpg";
                }

                outputFile = outputFolder + "\\" + sessionInfo.DateTime + "_" + recordPart.ToString() + videoExtension;
                replayFile = outputFile;

                // start new process
                ffmpeg = new Process();
                ffmpeg.StartInfo.FileName = @"ffmpeg.exe";
                ffmpeg.StartInfo.Arguments = String.Format(
#if USE_PINNED_MEMORY_BITMAP
                    // set video input: use system clock as timestamp, remove 0.5s at begining of video stream
                    "-f rawvideo -use_wallclock_as_timestamps 1 -pix_fmt bgr24 -video_size 1280x720 -thread_queue_size 64 -ss 0.5 -i - " +
#else
                    "-f image2pipe -use_wallclock_as_timestamps 1 -thread_queue_size 64 -ss 0.5 -i pipe:.bmp " +
#endif
                    // set audio input: use system clock as timestamp
                    (audioDeviceName.Equals("") ? "" : "-f dshow -use_wallclock_as_timestamps 1 -thread_queue_size 64 -i audio=\"{0}\" ") +
                    // audio filter: re-sample, add statistic, and then print audio level to ffmpeg stream (ffmpeg uses error stream)
                    "-af astats=metadata=1:reset=1:length=1:measure_perchannel=none,ametadata=mode=print:key=lavfi.astats.Overall.RMS_level " +
                    // set video output: use hw encoder for intel quick-sync
                    "-r {1} -b:v {2}k -c:v h264_qsv -preset veryfast -y {3} ",
                    // or use hw encoder for nvidia
                    //"-r {1} -b:v {2}k -c:v h264_amf -preset fast -y {3} ",
                    audioDeviceName,
                    settings.VideoFrameRate,
                    settings.VideoBitRate / 1000,
                    outputFile);
                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.CreateNoWindow = true;
                ffmpeg.StartInfo.RedirectStandardInput = true;
                ffmpeg.StartInfo.RedirectStandardError = true;
                ffmpeg.StartInfo.RedirectStandardOutput = true;
                ffmpeg.ErrorDataReceived += Ffmpeg_ErrorDataReceived;
                ffmpeg.OutputDataReceived += Ffmpeg_OutputDataReceived;

                Log.WriteLine(ffmpeg.StartInfo.Arguments);
                ffmpeg.Start();
                ffmpegInputStream = ffmpeg.StandardInput.BaseStream;
                ffmpeg.BeginErrorReadLine();
                ffmpeg.BeginOutputReadLine();

                // track new file
                AddFileToRecordList(outputFile, false);

                timerRecord.Start();
                videoRecordState = VideoRecordState.RECORDING;
                Log.WriteLine(">>> START recording");
            }
        }

        Regex regex = new Regex(@"RMS_level=(.*)");

        private void Ffmpeg_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            // capture RMS_level
            if (e != null && e.Data != null)
            {
                Match match = regex.Match(e.Data);
                if (match.Success)
                {
                    try
                    {
                        int value = (int)Convert.ToDouble(match.Groups[1].Value);
                        value = 100 + 2 * value;
                        value = value < 0 ? 0 : value;
                        value = value > 100 ? 100 : value;

                        Log.WriteLine(match.Groups[1].Value + " -> " + value);
                        if (prgAudioLevel.InvokeRequired)
                        {
                            prgAudioLevel.Invoke(new MethodInvoker(() => prgAudioLevel.Value = value));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                    }
                }
            }
        }
       
        private void Ffmpeg_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log.WriteLine("[FFMPEG Output] " + e.Data);
        }

        private void PauseRecording()
        {
            if (videoRecordState == VideoRecordState.RECORDING)
            {
                videoRecordState = VideoRecordState.IDLE;

                lock (syncRender)
                {
                    try
                    {
                        if (ffmpegInputStream != null && ffmpeg != null)
                        {
                            ffmpegInputStream.Flush();
                            ffmpegInputStream.Close();
                            Log.WriteLine("Flush FFMPEG");

                            Log.WriteLine("sending Ctrl+C to " + ffmpeg.SessionId);

                            if (AttachConsole((uint)ffmpeg.Id))
                            {
                                SetConsoleCtrlHandler(null, true);
                                try
                                {
                                    if (!GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0))
                                    {
                                        Log.WriteLine("Cannot break ffmpeg process");
                                    }
                                    else
                                    {
                                        ffmpeg.WaitForExit();
                                    }
                                }
                                finally
                                {
                                    FreeConsole();
                                    SetConsoleCtrlHandler(null, false);
                                }
                            }

                            Log.WriteLine("Exit FFMPEG");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                    }
                }

                timerRecord.Stop();
                Log.WriteLine(">>> PAUSE recording");
                recordPart++;
            }
        }

        private void StopRecording(bool temporary)
        {
            //TODO: Check sometime this hold file, did not finish file, so merge file not correct
            PauseRecording();

            if (temporary)
                Thread.Sleep(1000);

            Log.WriteLine(">>> MERGE VIDEO FILE");
            //outputFile = outputFolder + "\\" + sessionInfo.DateTime + "_" + recordPart.ToString() + videoExtension;

            // join files
            if (File.Exists("ffmpeg.exe") && recordPart > 1)
            {
                //String command = "ffmpeg.exe -f concat -safe 0 -i records.txt -c copy \"" + outputFile + "\"";
                //execv(command);
                replayFile = outputFolder + "\\" + sessionInfo.DateTime + "_final" + videoExtension;
                if (File.Exists(replayFile))
                {
                    File.Delete(replayFile);
                }
                String command_replay = "ffmpeg.exe -f concat -safe 0 -i " + recordTxt + " -c copy \"" + replayFile + "\"";
                RunCommand(command_replay);


                //add_file_to_record_list(outputFile, true);
                //recordPart++;

                Thread.Sleep(1000);

                //Update correct Duration @@!
                string outPut = "";

                System.Text.RegularExpressions.Regex re = null;
                System.Text.RegularExpressions.Match m = null;

                System.IO.StreamReader SROutput = null;

                //Get ready with ProcessStartInfo
                ffmpeg = new Process();
                ffmpeg.StartInfo.FileName = @"ffmpeg.exe";
                ffmpeg.StartInfo.Arguments = String.Format("-i \"{0}\" ", replayFile);
                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.CreateNoWindow = true;
                ffmpeg.StartInfo.RedirectStandardError = true;

                ffmpeg.Start();
                // Divert output
                SROutput = ffmpeg.StandardError;

                // Read all
                outPut = SROutput.ReadToEnd();

                // Please donot forget to call WaitForExit() after calling SROutput.ReadToEnd

                ffmpeg.WaitForExit();
                ffmpeg.Close();
                ffmpeg.Dispose();
                SROutput.Close();
                SROutput.Dispose();

                //get duration

                re = new System.Text.RegularExpressions.Regex("[D|d]uration:.((\\d|:|\\.)*)");
                m = re.Match(outPut);

                if (m.Success)
                {
                    //Means the output has cantained the string "Duration"
                    string temp = m.Groups[1].Value;
                    string[] timepieces = temp.Split('.');
                    txtTimeRun.Text = timepieces[0];
                    recordTime = (int)TimeSpan.Parse(timepieces[0]).TotalSeconds;
                }
            }
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

            if (timeLeft.TotalSeconds < 600) /* less than 10 mins */
            {
                // we can use default windows sound, select one of below
                // user can disable default windows sound via sound settings !!!
                //SystemSounds.Asterisk.Play();
                //SystemSounds.Beep.Play();
                //SystemSounds.Exclamation.Play();
                //SystemSounds.Hand.Play();
                //SystemSounds.Question.Play();

                // use our sound
                beepPlayer.Play();
                txtTimeLeft.ForeColor = Color.Red;
            }

            if (timeLeft.TotalSeconds == 0)
            {
                btnPause.PerformClick();
                btnRecord.Enabled = false;
                MessageBox.Show("Đã quá thời lượng ghi tối đa." + Environment.NewLine + "Vui lòng nhấn kết thúc!", "Thông báo", MessageBoxButtons.OK);
            }

            //if (videoRecordState == VideoRecordState.RECORDING)
            //{
            //    signalrecord.backgroundimage = global::vacamera.properties.resources.blink;
            //    if (recordtime % 2 == 0)
            //    {
            //        signalrecord.backgroundimage = global::vacamera.properties.resources.rec_on;
            //    }
            //    else
            //    {
            //        signalrecord.backgroundimage = global::vacamera.properties.resources.rec_off;
            //    }
            //}
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsFileLocked())
            {
                return;
            }
            if (MessageBox.Show("Thoát ứng dụng và tắt máy?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void newSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsFileLocked())
            {
                return;
            }

            if (MessageBox.Show("Tạo phiên làm việc mới?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Hide();

                using (FormNewSession formNewSession = new FormNewSession())
                {
                    if (formNewSession.ShowDialog(this) == DialogResult.OK)
                    {
                        sessionInfo = formNewSession.SessionInfo;
                        settingsToolStripMenuItem.Enabled = true;
                        PrepareRecord();

                        // debug
                        Log.WriteLine(sessionInfo.ToString());
                        Log.WriteLine(settings.ToString());
                    }
                }

                //Delete all file when create new session
                DirectoryInfo di = new DirectoryInfo(outputFolder);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                Show();

                if (videoDevice1 != null)
                {
                    btnRecord.Enabled = true;
                    btnPause.Enabled = false;
                    btnStop.Enabled = false;
                    btnReplay.Enabled = false;
                    btnWriteDisk.Enabled = false;
                }

                UpdateRecordTime(0);

                //InitDevices(); // this process is slow
                ResumePreview();
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PausePreview();

            using (FormSettings formSettings = new FormSettings())
            {
                DialogResult result = formSettings.ShowDialog(this);
                settings = formSettings.Settings;
                if (result == DialogResult.OK)
                {
                    InitDevices();
                }
                else
                {
                    ResumePreview();
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

        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (IsFileLocked())
            {
                return;
            }

            if (videoRecordState == VideoRecordState.IDLE)
            {
                btnRecord.Enabled = false;
                btnPause.Enabled = true;
                btnStop.Enabled = false;
                btnReplay.Enabled = false;
                btnWriteDisk.Enabled = false;

                settingsToolStripMenuItem.Enabled = false; // do not change settings during recording
                signalRecord.Image = global::VACamera.Properties.Resources.blink;
                prgAudioLevel.Visible = true;

                StartRecording();
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (videoRecordState == VideoRecordState.RECORDING)
            {

                btnPause.Enabled = false;
                signalRecord.Image = global::VACamera.Properties.Resources.rec_pause;
                prgAudioLevel.Value = 0;

                StopRecording(true);
                //PauseRecording();

                btnRecord.Enabled = true;
                btnStop.Enabled = true;
                btnReplay.Enabled = true;
                btnWriteDisk.Enabled = false;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Kết thúc ghi hình và ghi DVD?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                btnRecord.Enabled = false; // must start new session
                btnPause.Enabled = false;
                btnStop.Enabled = true;
                btnReplay.Enabled = true;
                btnWriteDisk.Enabled = true;

                settingsToolStripMenuItem.Enabled = true; // can change settings again
                signalRecord.BackgroundImage = null;
                prgAudioLevel.Visible = false;

                //StopRecording(false);
                //Thread.Sleep(1000);

                StopDevices(); // this method is slow
                PausePreview();

                btnWriteDisk_Click(btnWriteDisk, EventArgs.Empty);
            }
        }

        private void btnReplay_Click(object sender, EventArgs e)
        {
            Console.WriteLine(replayFile);
            if (videoRecordState == VideoRecordState.IDLE)
            {
                if (File.Exists(replayFile))
                {
                    Process.Start(replayFile);
                }
            }
        }

        private void btnWriteDisk_Click(object sender, EventArgs e)
        {
            Log.WriteLine(sessionInfo.ToString());
            //Log.WriteLine("outputFile = " + outputFile);
            Log.WriteLine("outputFile = " + replayFile);
            using (FormDvdWriter formDvdWriter = new FormDvdWriter(sessionInfo.DateTime, replayFile))
            {
                DialogResult result = formDvdWriter.ShowDialog();
                if (result == DialogResult.Yes)
                {
                    newSessionToolStripMenuItem_Click(newSessionToolStripMenuItem, EventArgs.Empty);
                }
                else if (result == DialogResult.No)
                {
                    // Can return main form and click to write DVD again if one DVD fail
                    //MessageBox.Show("Kết thúc phiên làm việc và dừng chương trình!");
                    //Close();
                }
                else if (result == DialogResult.Cancel)
                {
                    MessageBox.Show("Vui lòng tạo phiên làm việc mới!");
                }
            }
        }

        private void timerRecord_Tick(object sender, EventArgs e)
        {
            recordTime++;
            UpdateRecordTime();
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            Log.WriteLine(e.KeyCode.ToString());
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnRecord.PerformClick(); // won't perform if button is disabled
                    break;
                case Keys.F6:
                    btnPause.PerformClick();
                    break;
                case Keys.F7:
                    btnReplay.PerformClick();
                    break;
                case Keys.F8:
                    btnStop.PerformClick();
                    break;
                default:
                    break;
            }
        }

        private bool IsFileLocked()
        {
            DirectoryInfo di = new DirectoryInfo(outputFolder);
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        stream.Close();
                    }
                }
                catch (IOException)
                {
                    MessageBox.Show("File đang được sử dụng, hãy tắt chương trình đang sử dụng khác!");
                    return true;
                }
            }
            return false;
        }
    }
}
