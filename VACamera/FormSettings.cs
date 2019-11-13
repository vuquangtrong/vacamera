using Accord.Video.DirectShow;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace VACamera
{
    public partial class FormSettings : Form
    {
        public Settings Settings;
        private FilterInfoCollection videoDevices;
        private FilterInfoCollection audioDevices;
        private bool isChanged = false;

        public FormSettings()
        {
            Settings = new Settings();
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            // audio devices
            try
            {
                audioDevices = new FilterInfoCollection(FilterCategory.AudioInputDevice);
                if (audioDevices != null && audioDevices.Count > 0)
                {
                    int index = 0;
                    foreach (FilterInfo audioDevice in audioDevices)
                    {
                        listAudioSource.Items.Add(audioDevice.Name);
                        if (Settings.AudioInputPath.Equals(audioDevice.MonikerString))
                        {
                            listAudioSource.SelectedIndex = index;
                            Settings.SetAudioInputPath(audioDevice.MonikerString);
                        }
                        index++;
                    }
                    if (listAudioSource.SelectedIndex < 0)
                    {
                        listAudioSource.SelectedIndex = 0;
                    }
                }
                else
                {
                    listAudioSource.Items.Add("Không có microphone");
                    listAudioSource.SelectedIndex = 0;
                    Settings.SetAudioInputPath("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            // audio channel
            listAudioChannel.SelectedIndex = (int)Settings.AudioChannel;

            // video devices
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices != null && videoDevices.Count > 0)
                {
                    int index1 = 0;
                    int index2 = 0;
                    foreach (FilterInfo videoDevice in videoDevices)
                    {
                        listCamera1.Items.Add(videoDevice.Name);
                        listCamera2.Items.Add(videoDevice.Name);

                        if (Settings.Camera1_InputPath.Equals(videoDevice.MonikerString))
                        {
                            listCamera1.SelectedIndex = index1;
                        }
                        index1++;

                        if (Settings.Camera2_InputPath.Equals(videoDevice.MonikerString))
                        {
                            listCamera2.SelectedIndex = index2;
                        }
                        index2++;
                    }

                    if (listCamera1.SelectedIndex < 0)
                    {
                        listCamera1.SelectedIndex = 0;
                    }

                    if (listCamera2.SelectedIndex < 0)
                    {
                        if (videoDevices.Count >= 2)
                        {
                            listCamera2.SelectedIndex = 1;
                        }
                        else
                        {
                            listCamera2.Items.Clear();
                            listCamera2.Items.Add("Chỉ có một webcam");
                            listCamera2.SelectedIndex = 0;
                            listCamera2.Enabled = false;
                            // fallback to Single mode
                            Settings.SetVideoMixingMode(Settings.VideoMode.Single);
                        }
                    }
                }
                else
                {
                    listCamera1.Items.Add("Không tìm thấy webcam");
                    listCamera1.SelectedIndex = 0;
                    listCamera1.Enabled = false;

                    listCamera2.Items.Add("Không tìm thấy webcam");
                    listCamera2.SelectedIndex = 0;
                    listCamera2.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            // video mixing mode
            if (videoDevices.Count >= 2)
            {
                listVideoMixingMode.SelectedIndex = (int)Settings.VideoMixingMode;
            } else
            {
                listVideoMixingMode.SelectedIndex = 0;
            }

            // video output
            listVideoFormat.SelectedIndex = (int)Settings.VideoOutputFormat;

            // video bitrate
            listBitRate.Items.Add(Settings.BitRate.ToString());

            // video framerate
            listFrameRate.Text = Settings.FrameRate.ToString();

            // update UI
            ToggleVideoSettings();

            // reset flag
            isChanged = false;
        }

        private void listAudioSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("listAudioSource = " + listAudioSource.Text);
            Settings.SetAudioInputPath(listAudioSource.Text);
            isChanged = true;
        }

        private void listAudioChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("listAudioChannel = " + GetAudioMode(listAudioChannel.SelectedIndex));
            Settings.SetAudioChannel(GetAudioMode(listAudioChannel.SelectedIndex));
            isChanged = true;
        }

        private void listVideoMixingMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (videoDevices.Count >= 2)
            {
                Console.WriteLine("listVideoMixingMode = " + GetVideoMode(listVideoMixingMode.SelectedIndex));
                Settings.SetVideoMixingMode(GetVideoMode(listVideoMixingMode.SelectedIndex));
                
            } else
            {
                Settings.SetVideoMixingMode(Settings.VideoMode.Single);
            }
            ToggleVideoSettings();
            isChanged = true;
        }

        private void listCamera1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("listCamera1 = " + listCamera1.Text);
            Settings.SetCamera1_InputPath(videoDevices[listCamera1.SelectedIndex].MonikerString);
            isChanged = true;
        }

        private void listCamera2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("listCamera2 = " + listCamera2.Text);
            Settings.SetCamera2_InputPath(videoDevices[listCamera2.SelectedIndex].MonikerString);
            isChanged = true;
        }

        private void listVideoFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("listVideoFormat = " + listVideoFormat.Text);
            Settings.SetVideoOutputFormat(listVideoFormat.Text);
            isChanged = true;
        }

        private void listBitRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("listBitRate = " + listBitRate.Text);
            Settings.SetBitRate(listBitRate.Text);
            isChanged = true;
        }

        private void listFrameRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("listFrameRate = " + listFrameRate.Text);
            Settings.SetFrameRate(listFrameRate.Text);
            isChanged = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isChanged)
            {
                //Settings.SetAudioInputPath(listAudioSource.Text);
                //Settings.SetAudioChannel(GetAudioMode(listAudioChannel.SelectedIndex));
                //Settings.SetVideoMixingMode(GetVideoMode(listVideoMixingMode.SelectedIndex));
                //Settings.SetCamera1_InputPath(listCamera1.Text);
                //Settings.SetCamera2_InputPath(listCamera2.Text);
                //Settings.SetVideoOutputFormat(listVideoFormat.Text);
                //Settings.SetBitRate(listBitRate.Text);
                //Settings.SetFrameRate(listFrameRate.Text);
                Settings.SaveSettings();

                DialogResult = DialogResult.OK;
            }
            Close();
        }

        private string GetAudioMode(int index)
        {
            switch (index)
            {
                case 0:
                    return "Stereo";
                case 1:
                    return "Mono";
                default:
                    return "Stereo";
            }
        }

        private string GetVideoMode(int index)
        {
            switch (index)
            {
                case 0:
                    return "Single";
                case 1:
                    return "SideBySide";
                case 2:
                    return "Overlay";
                default:
                    return "Single";
            }
        }

        private void ToggleVideoSettings()
        {
            if (Settings.VideoMixingMode == Settings.VideoMode.Single)
            {
                listCamera1.Enabled = true;
                txtCamera1.ForeColor = Color.Black;

                listCamera2.Enabled = false;
                txtCamera2.ForeColor = Color.Gray;
            }
            else
            {
                listCamera1.Enabled = true;
                txtCamera1.ForeColor = Color.Black;

                listCamera2.Enabled = true;
                txtCamera2.ForeColor = Color.Black;
            }
        }
    }
}
