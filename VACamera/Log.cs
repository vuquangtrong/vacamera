using System;
using System.Diagnostics;
using System.IO;

namespace VACamera
{
    static class Log
    {
        static StreamWriter streamWriter = null;

        public static void Init()
        {
            string logPath = "C:\\logs";

            try
            {
                Directory.CreateDirectory(logPath);
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }

            try
            {
                streamWriter = new StreamWriter(logPath + "\\" + DateTime.Now.ToString("MMddyyyy_HHmmss") + ".log");
            }
            catch (Exception ex)
            {
                WriteLine("Cannot redirect log to a file!!!");
                WriteLine(ex.ToString());
            }

            WriteLine("VERSION: 191220 2230");
        }

        public static void Close()
        {
            if (streamWriter != null)
            {
                streamWriter.Close();
            }
        }

        public static void Write(string message)
        {
            var ts = DateTime.Now.ToString(@"MM/dd HH:mm:ss.fff");
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            var currentMethodName = sf.GetMethod().Name;
#if DEBUG
            Console.Write(ts + " " + currentMethodName + ": " + message);
#else
            if (streamWriter != null)
            {
                streamWriter.Write(ts + " " + currentMethodName + ": " + message);
                streamWriter.Flush();
            }
#endif
        }

        public static void WriteLine(string message)
        {
            var ts = DateTime.Now.ToString(@"MM/dd HH:mm:ss.fff");
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            var currentMethodName = sf.GetMethod().Name;
#if DEBUG
            Console.WriteLine(ts + " " + currentMethodName + ": " + message);
#else
            if (streamWriter != null)
            {
                streamWriter.WriteLine(ts + " " + currentMethodName + ": " + message);
                streamWriter.Flush();
            }
#endif
        }
    }
}
