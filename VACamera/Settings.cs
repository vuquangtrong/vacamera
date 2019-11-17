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
        public AudioMode AudioChannel { get; private set; } /* Stereo */
        public int AudioBitRate { get; private set; } /* 320 kbps */
        public int AudioSampleRate { get; private set; } /* 44100 Hz */
        public int AudioFrameSize { get; private set; } /* 8Kb */
        public string AudioInputPath { get; private set; } /* Default */
        public VideoMode VideoMixingMode { get; private set; } /* Single */
        public string Camera1_InputPath { get; private set; } /* Default */
        public string Camera2_InputPath { get; private set; } /* Default */
        public VideoFormat VideoOutputFormat { get; private set; } /* MPEG4 */
        public int VideoBitRate { get; private set; } /* 3 Mbps */
        public int VideoFrameRate { get; private set; } /* 30 */

        // below attributs is automatically set after VideoMixingMode is changed
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
            AudioChannel = mode;
            Log.WriteLine("AudioChannel = " + AudioChannel);
        }

        public void SetAudioChannel(string mode)
        {
            if (mode == null || mode.Equals(""))
            {
                AudioChannel = AudioMode.Mono;
            }
            else
            {
                try
                {
                    AudioChannel = (AudioMode)Enum.Parse(typeof(AudioMode), mode);
                }
                catch (Exception ex)
                {
                    AudioChannel = AudioMode.Mono;
                    Log.WriteLine(ex.ToString());
                }
            }
            SetAudioChannel(AudioChannel);
        }

        public void SetAudioBitRate(int rate)
        {
            AudioBitRate = rate;
            Log.WriteLine("AudioBitRate = " + AudioBitRate);
        }

        public void SetAudioBitRate(string rate)
        {
            if (rate == null || rate.Equals(""))
            {
                AudioBitRate = 320 * 1000;
            }
            else
            {
                try
                {
                    AudioBitRate = int.Parse(rate);
                }
                catch (Exception ex)
                {
                    AudioBitRate = 320 * 1000;
                    Log.WriteLine(ex.ToString());
                }
            }
            SetAudioBitRate(AudioBitRate);
        }

        public void SetAudioSampleRate(int rate)
        {
            AudioSampleRate = rate;
            Log.WriteLine("AudioSampleRate = " + AudioSampleRate);
        }

        public void SetAudioSampleRate(string rate)
        {
            if (rate == null || rate.Equals(""))
            {
                AudioSampleRate = 44100;
            }
            else
            {
                try
                {
                    AudioSampleRate = int.Parse(rate);
                }
                catch (Exception ex)
                {
                    AudioSampleRate = 44100;
                    Log.WriteLine(ex.ToString());
                }
            }
            SetAudioSampleRate(AudioSampleRate);
        }

        public void SetAudioFrameSize(int size)
        {
            AudioFrameSize = size;
            Log.WriteLine("AudioFrameSize = " + AudioFrameSize);
        }

        public void SetAudioFrameSize(string size)
        {
            if (size == null || size.Equals(""))
            {
                AudioFrameSize = 8 * 1024;
            }
            else
            {
                try
                {
                    AudioFrameSize = int.Parse(size);
                }
                catch (Exception ex)
                {
                    AudioFrameSize = 8 * 1024;
                    Log.WriteLine(ex.ToString());
                }
            }
            SetAudioFrameSize(AudioFrameSize);
        }

        public void SetAudioInputPath(string path)
        {
            AudioInputPath = path;
            Log.WriteLine("AudioInputPath = " + AudioInputPath);
        }

        public void SetVideoMixingMode(VideoMode mode)
        {
            VideoMixingMode = mode;
            Log.WriteLine("VideoMixingMode = " + VideoMixingMode);

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
                catch (Exception ex)
                {
                    VideoMixingMode = VideoMode.Single;
                    Log.WriteLine(ex.ToString());
                }
            }
            SetVideoMixingMode(VideoMixingMode);
        }

        public void SetCamera1_InputPath(string path)
        {
            Camera1_InputPath = path;
            Log.WriteLine("Camera1_InputPath = " + Camera1_InputPath);
        }

        public void SetCamera2_InputPath(string path)
        {
            Camera2_InputPath = path;
            Log.WriteLine("Camera2_InputPath = " + Camera2_InputPath);
        }

        public void SetVideoOutputFormat(VideoFormat format)
        {
            VideoOutputFormat = format;
            Log.WriteLine("VideoOutputFormat = " + VideoOutputFormat);
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
                catch (Exception ex)
                {
                    VideoOutputFormat = VideoFormat.MPEG4;
                    Log.WriteLine(ex.ToString());
                }
            }
            SetVideoOutputFormat(VideoOutputFormat);
        }

        public void SetVideoBitRate(int rate)
        {
            VideoBitRate = rate;
            Log.WriteLine("VideoBitRate = " + VideoBitRate);
        }

        public void SetVideoBitRate(string rate)
        {
            if (rate == null || rate.Equals(""))
            {
                VideoBitRate = 3000 * 1000;
            }
            else
            {
                try
                {
                    VideoBitRate = int.Parse(rate);
                }
                catch (Exception ex)
                {
                    VideoBitRate = 3000 * 1000;
                    Log.WriteLine(ex.ToString());
                }
            }
            SetVideoBitRate(VideoBitRate);
        }

        public void SetVideoFrameRate(int rate)
        {
            VideoFrameRate = rate;
            Log.WriteLine("VideoFrameRate = " + VideoFrameRate);
        }

        public void SetVideoFrameRate(string rate)
        {
            if (rate == null || rate.Equals(""))
            {
                VideoFrameRate = 30;
            }
            else
            {
                try
                {
                    VideoFrameRate = int.Parse(rate);
                }
                catch (Exception ex)
                {
                    VideoFrameRate = 30;
                    Log.WriteLine(ex.ToString());
                }
            }
            SetVideoFrameRate(VideoFrameRate);
        }

        public void LoadSettings()
        {
            if (iniFile != null)
            {
                SetAudioInputPath(iniFile.ReadValue("Settings", "AudioInputPath"));
                SetAudioChannel(iniFile.ReadValue("Settings", "AudioChannel"));
                SetAudioBitRate(iniFile.ReadValue("Settings", "AudioBitRate"));
                SetAudioSampleRate(iniFile.ReadValue("Settings", "AudioSampleRate"));
                SetAudioFrameSize(iniFile.ReadValue("Settings", "AudioFrameSize"));
                SetVideoMixingMode(iniFile.ReadValue("Settings", "VideoMixingMode"));
                SetCamera1_InputPath(iniFile.ReadValue("Settings", "Camera1_InputPath"));
                SetCamera2_InputPath(iniFile.ReadValue("Settings", "Camera2_InputPath"));
                SetVideoOutputFormat(iniFile.ReadValue("Settings", "VideoOutputFormat"));
                SetVideoBitRate(iniFile.ReadValue("Settings", "VideoBitRate"));
                SetVideoFrameRate(iniFile.ReadValue("Settings", "VideoFrameRate"));
            }

            Log.WriteLine(ToString());
        }

        public void SaveSettings()
        {
            if (iniFile != null)
            {
                iniFile.WriteValue("Settings", "AudioInputPath", AudioInputPath);
                iniFile.WriteValue("Settings", "AudioChannel", AudioChannel.ToString());
                iniFile.WriteValue("Settings", "AudioBitRate", AudioBitRate.ToString());
                iniFile.WriteValue("Settings", "AudioSampleRate", AudioSampleRate.ToString());
                iniFile.WriteValue("Settings", "AudioFrameSize", AudioFrameSize.ToString());
                iniFile.WriteValue("Settings", "Camera1_InputPath", Camera1_InputPath);
                iniFile.WriteValue("Settings", "Camera2_InputPath", Camera2_InputPath);
                iniFile.WriteValue("Settings", "VideoMixingMode", VideoMixingMode.ToString());
                iniFile.WriteValue("Settings", "VideoOutputFormat", VideoOutputFormat.ToString());
                iniFile.WriteValue("Settings", "VideoBitRate", VideoBitRate.ToString());
                iniFile.WriteValue("Settings", "VideoFrameRate", VideoFrameRate.ToString());
            }

            Log.WriteLine(ToString());
        }

        override public string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Environment.NewLine + "[" + Environment.NewLine);
            stringBuilder.Append("AudioInputPath = " + AudioInputPath + Environment.NewLine);
            stringBuilder.Append("AudioChannel = " + AudioChannel + Environment.NewLine);
            stringBuilder.Append("AudioBitRate = " + AudioBitRate + Environment.NewLine);
            stringBuilder.Append("AudioSampleRate = " + AudioSampleRate + Environment.NewLine);
            stringBuilder.Append("AudioFrameSize = " + AudioFrameSize + Environment.NewLine);
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
            stringBuilder.Append("VideoFrameRate = " + VideoFrameRate + Environment.NewLine);
            stringBuilder.Append("VideoBitRate = " + VideoBitRate + Environment.NewLine);
            stringBuilder.Append("]" + Environment.NewLine);

            return stringBuilder.ToString();
        }
    }
}
