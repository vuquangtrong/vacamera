using System;
using System.Windows.Forms;

namespace VACamera
{
    public partial class FormNewSession : Form
    {
        public SessionInfo SessionInfo;

        public FormNewSession()
        {
            SessionInfo = new SessionInfo();
            InitializeComponent();

            textName1.Text = "Thông tin 1";
            textName2.Text = "Thông tin 2";
            textName3.Text = "Thông tin 3";
            textName4.Text = "Thông tin 4";
            textName5.Text = "Thông tin 5";
        }

        private void FormNewSession_Load(object sender, EventArgs e)
        {
            textDateTime.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
            timer1.Start();
        }

        private void FormNewSession_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
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
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textDateTime.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
        }
    }
}
