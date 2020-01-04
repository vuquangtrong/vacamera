using Microsoft.Win32;
using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace VACamera
{
    public partial class FormNewSession : Form
    {
        public SessionInfo SessionInfo;

        public FormNewSession()
        {
            SessionInfo = new SessionInfo();
            InitializeComponent();

            textName1.Text = "ĐTV Nguyễn Văn Ánh";
            textName2.Text = "ĐV Bộ Thông tin 23";
            textName3.Text = "ĐT Phạm Băng Băng";
            textName4.Text = "Loca Phòng 11 cục 22";
            textName5.Text = "Về việc ông Băng làm tan băng";

            Hide();
        }

        private void FormNewSession_Load(object sender, EventArgs e)
        {
            textDateTime.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
            timer1.Start();

            if (!isLicenseValid())
            {
                MessageBox.Show("Phần mềm chưa được đăng kí!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.Cancel;
                Close();
            }
            else
            {
                Show();
            }
        }

        private void FormNewSession_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }

        private bool isLicenseValid()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\VACamera\\Settings"))
                {
                    if (key != null)
                    {
                        string name = (String)key.GetValue("Name");
                        string company = (String)key.GetValue("Company");

                        Log.WriteLine("Name = " + name);
                        Log.WriteLine("Company = " + company);

                        if (name != null && company != null)
                        {
                            SessionInfo.License = name + " " + company;
                            txtLicense.Text = SessionInfo.License;
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // check inputs
            if (textName1.Text.Equals("")
                || textName2.Text.Equals("")
                || textName3.Text.Equals("")
                || textName4.Text.Equals("")
                || textName5.Text.Equals(""))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các trường thông tin", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                SessionInfo.Name1 = textName1.Text;
                SessionInfo.Name2 = textName2.Text;
                SessionInfo.Name3 = textName3.Text;
                SessionInfo.Name4 = textName4.Text;
                SessionInfo.Name5 = textName5.Text;
                SessionInfo.DateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Thoát ứng dụng và Tắt máy?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Windows.Forms.Application.Exit();
                //string command = "shutdown -s -t 0";
                //Process process = new Process();
                //ProcessStartInfo startInfo = new ProcessStartInfo();

                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //startInfo.FileName = "cmd.exe";
                //startInfo.Arguments = "/C " + command;
                //process.StartInfo = startInfo;
                //process.Start();
                //process.WaitForExit();
            }
            //Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textDateTime.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("osk.exe");
        }
    }
}
