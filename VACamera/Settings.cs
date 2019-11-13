using System;
using System.Text;

namespace VACamera
{
    public class Settings
    {
        public static int VideoWidth = 1280;
        public static int VideoHeight = 720;
        public static int VideoOverlayWidth = 426;
        public static int VideoOverlayHeight = 240;
        public static int VideoOverlayX = 1280 - 10 - VideoOverlayWidth;
        public static int VideoOverlayY = 720 - 10 - VideoOverlayHeight;

        public enum AudioMode
        {
            Mono,
            Stereo
        }
        public enum VideoMode
        {
            Single, /* Camera 1 is full */
            SideBySide, /* Camera 1 is left, Camera 2 is right */
            Overlay /* Camera1 is main, Camera 2 is overlay */
        }
        public enum VideoFormat
        {
            MPEG4,
            MPEG2
        }

        // attributes
        public AudioMode AudioChannel { get; private set; }
        public string AudioInputPath { get; private set; }
        public VideoMode VideoMixingMode { get; private set; }
        public string Camera1_InputPath { get; private set; }
        public string Camera2_InputPath { get; private set; }
        public VideoFormat VideoOutputFormat { get; private set; }
        public int BitRate { get; private set; }
        public int FrameRate { get; private set; }
        public int Frame1_Width { get; private set; }
        public int Frame1_Height { get; private set; }
        public int Frame1_X { get; private set; }
        public int Frame1_Y { get; private set; }
        public int Frame2_Width { get; private set; }
        public int Frame2_Height { get; private set; }
        public int Frame2_X { get; private set; }
        public int Frame2_Y { get; private set; }

        // private
        private IniFile iniFile;

        public Settings()
        {
            iniFile = new IniFile(Environment.CurrentDirectory + "\\Settings.ini");
            LoadSettings();
        }

        // methods
        public void SetAudioChannel(AudioMode mode)
        {
            Console.WriteLine("AudioChannel = " + AudioChannel);
            AudioChannel = mode;
        }

        public void SetAudioChannel(string mode)
        {
            if (mode == null || mode.Equals(""))
            {
                AudioChannel = AudioMode.Stereo;
            }
            else
            {
                try
                {
                    AudioChannel = (AudioMode)Enum.Parse(typeof(AudioMode), mode);
                }
                catch (Exception e)
                {
                    AudioChannel = AudioMode.Stereo;
                    Console.WriteLine(e.ToString());
                }
            }
            SetAudioChannel(AudioChannel);
        }

        public void SetAudioInputPath(string path)
        {
            AudioInputPath = path;
        }

        public void SetVideoMixingMode(VideoMode mode)
        {
            VideoMixingMode = mode;
            Console.WriteLine("VideoMixingMode = " + VideoMixingMode);

            if (VideoMixingMode == VideoMode.SideBySide)
            {
                Frame1_Width = VideoWidth / 2;
                Frame1_Height = VideoHeight;
                Frame1_X = 0;
                Frame1_Y = 0;

                Frame2_Width = VideoWidth / 2;
                Frame2_Height = VideoHeight;
                Frame2_X = VideoWidth / 2;
                Frame2_Y = 0;
            }
            else if (VideoMixingMode == VideoMode.Overlay)
            {
                Frame1_Width = VideoWidth;
                Frame1_Height = VideoHeight;
                Frame1_X = 0;
                Frame1_Y = 0;

                Frame2_Width = VideoOverlayWidth;
                Frame2_Height = VideoOverlayHeight;
                Frame2_X = VideoOverlayX;
                Frame2_Y = VideoOverlayY;
            }
            else
            {
                Frame1_Width = VideoWidth;
                Frame1_Height = VideoHeight;
                Frame1_X = 0;
                Frame1_Y = 0;

                Frame2_Width = 0;
                Frame2_Height = 0;
                Frame2_X = 0;
                Frame2_Y = 0;
            }
        }

        public void SetVideoMixingMode(string mode)
        {
            if (mode == null || mode.Equals(""))
            {
                VideoMixingMode = VideoMode.Single;
            }
            else
            {
                try
                {
                    VideoMixingMode = (VideoMode)Enum.Parse(typeof(VideoMode), mode);
                }
                catch (Exception e)
                {
                    VideoMixingMode = VideoMode.Single;
                    Console.WriteLine(e.ToString());
                }
            }
            SetVideoMixingMode(VideoMixingMode);
        }

        public void SetCamera1_InputPath(string path)
        {
            Camera1_InputPath = path;
            Console.WriteLine("Camera1_InputPath = " + Camera1_InputPath);
        }

        public void SetCamera2_InputPath(string path)
        {
            Camera2_InputPath = path;
            Console.WriteLine("Camera2_InputPath = " + Camera2_InputPath);
        }

        public void SetVideoOutputFormat(VideoFormat format)
        {
            VideoOutputFormat = format;
            Console.WriteLine("VideoOutputFormat = " + VideoOutputFormat);
        }
        public void SetVideoOutputFormat(string format)
        {
            if (format == null || format.Equals(""))
            {
                VideoOutputFormat = VideoFormat.MPEG4;
            }
            else
            {
                try
                {
                    VideoOutputFormat = (VideoFormat)Enum.Parse(typeof(VideoFormat), format);
                }
                catch (Exception e)
                {
                    VideoOutputFormat = VideoFormat.MPEG4;
                    Console.WriteLine(e.ToString());
                }
            }
            SetVideoOutputFormat(VideoOutputFormat);
        }

        public void SetBitRate(int rate)
        {
            BitRate = rate;
            Console.WriteLine("BitRate = " + BitRate);
        }

        public void SetBitRate(string rate)
        {
            if (rate == null || rate.Equals(""))
            {
                BitRate = 3 * 1024 * 1024;
            }
            else
            {
                try
                {
                    BitRate = int.Parse(rate);
                }
                catch (Exception ex)
                {
                    BitRate = 3 * 1024 * 1024;
                    Console.WriteLine(ex.StackTrace);
                }
            }
            SetBitRate(BitRate);
        }

        public void SetFrameRate(int rate)
        {
            FrameRate = rate;
            Console.WriteLine("FrameRate = " + FrameRate);
        }

        public void SetFrameRate(string rate)
        {
            if (rate == null || rate.Equals(""))
            {
                FrameRate = 30;
            }
            else
            {
                try
                {
                    FrameRate = int.Parse(rate);
                }
                catch (Exception ex)
                {
                    FrameRate = 30;
                    Console.WriteLine(ex.StackTrace);
                }
            }
            SetFrameRate(FrameRate);
        }

        public void LoadSettings()
        {
            if (iniFile != null)
            {
                SetAudioInputPath(iniFile.ReadValue("Settings", "AudioInputPath"));
                SetAudioChannel(iniFile.ReadValue("Settings", "AudioChannel"));
                SetVideoMixingMode(iniFile.ReadValue("Settings", "VideoMixingMode"));
                SetCamera1_InputPath(iniFile.ReadValue("Settings", "Camera1_InputPath"));
                SetCamera2_InputPath(iniFile.ReadValue("Settings", "Camera2_InputPath"));
                SetVideoOutputFormat(iniFile.ReadValue("Settings", "VideoOutputFormat"));
                SetBitRate(iniFile.ReadValue("Settings", "BitRate"));
                SetFrameRate(iniFile.ReadValue("Settings", "FrameRate"));
            }

            Console.WriteLine(ToString());
        }

        public void SaveSettings()
        {
            if (iniFile != null)
            {
                iniFile.WriteValue("Settings", "AudioInputPath", AudioInputPath);
                iniFile.WriteValue("Settings", "AudioChannel", AudioChannel.ToString());
                iniFile.WriteValue("Settings", "Camera1_InputPath", Camera1_InputPath);
                iniFile.WriteValue("Settings", "Camera2_InputPath", Camera2_InputPath);
                iniFile.WriteValue("Settings", "VideoMixingMode", VideoMixingMode.ToString());
                iniFile.WriteValue("Settings", "VideoOutputFormat", VideoOutputFormat.ToString());
                iniFile.WriteValue("Settings", "BitRate", BitRate.ToString());
                iniFile.WriteValue("Settings", "FrameRate", FrameRate.ToString());
            }
        }

        override public string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Environment.NewLine + "[" + Environment.NewLine);
            stringBuilder.Append("AudioInputPath = " + AudioInputPath + Environment.NewLine);
            stringBuilder.Append("AudioChannel = " + AudioChannel + Environment.NewLine);
            stringBuilder.Append("Camera1_InputPath = " + Camera1_InputPath + Environment.NewLine);
            stringBuilder.Append("Camera2_InputPath = " + Camera2_InputPath + Environment.NewLine);
            stringBuilder.Append("VideoMixingMode = " + VideoMixingMode + Environment.NewLine);
            stringBuilder.Append("Frame1_Width = " + Frame1_Width + Environment.NewLine);
            stringBuilder.Append("Frame1_Height = " + Frame1_Height + Environment.NewLine);
            stringBuilder.Append("Frame1_X = " + Frame1_X + Environment.NewLine);
            stringBuilder.Append("Frame1_Y = " + Frame1_Y + Environment.NewLine);
            stringBuilder.Append("Frame2_Width = " + Frame2_Width + Environment.NewLine);
            stringBuilder.Append("Frame2_Height = " + Frame2_Height + Environment.NewLine);
            stringBuilder.Append("Frame2_X = " + Frame2_X + Environment.NewLine);
            stringBuilder.Append("Frame2_Y = " + Frame2_Y + Environment.NewLine);
            stringBuilder.Append("VideoOutputFormat = " + VideoOutputFormat + Environment.NewLine);
            stringBuilder.Append("FrameRate = " + FrameRate + Environment.NewLine);
            stringBuilder.Append("BitRate = " + BitRate + Environment.NewLine);
            stringBuilder.Append("]" + Environment.NewLine);

            return stringBuilder.ToString();
        }
    }
}
