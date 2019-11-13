using System;
using System.Windows.Forms;

namespace VACamera
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log.Init();
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Log.Close();
        }
    }
}
