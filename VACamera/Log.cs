using System;
using System.Diagnostics;
using System.IO;

namespace VACamera
{
    static class Log
    {
        static FileStream fileStream = null;
        static StreamWriter streamWriter = null;

        public static void Init()
        {
            string logPath = Environment.CurrentDirectory + "\\logs";

            try
            {
                Directory.CreateDirectory(logPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // redirect console log to a file
            try
            {
                fileStream = new FileStream(logPath + "\\" + DateTime.Now.ToString("MMddyyyy_HHmmss") + ".log", FileMode.OpenOrCreate, FileAccess.Write);
                streamWriter = new StreamWriter(fileStream);
#if DEBUG
#else
                Console.SetOut(streamWriter);
#endif
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot redirect log to a file!!!");
                Console.WriteLine(e.ToString());
            }
        }

        public static void Close()
        {
            if (streamWriter != null)
            {
                streamWriter.Close();
            }

            if (fileStream != null)
            {
                fileStream.Close();
            }
        }

        public static void Write(string message)
        {
            var ts = DateTime.Now.ToString(@"MM/dd HH:mm:ss.fff");
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            var currentMethodName = sf.GetMethod().Name;
            Console.Write(ts + " " + currentMethodName + ": " + message);
        }

        public static void WriteLine(string message)
        {
            var ts = DateTime.Now.ToString(@"MM/dd HH:mm:ss.fff");
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            var currentMethodName = sf.GetMethod().Name;
            Console.WriteLine(ts + " " + currentMethodName + ": " + message);
        }
    }
}
