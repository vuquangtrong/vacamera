using System.Runtime.InteropServices;
using System.Text;

namespace VACamera
{
    public class IniFile
    {
        public string Path { get; set; }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
          string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
          string key, string def, StringBuilder retVal,
          int size, string filePath);

        public IniFile(string filename)
        {
            Path = filename;
        }

        public void WriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.Path);
        }

        public string ReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            _ = GetPrivateProfileString(Section, Key, "", temp, 255, this.Path);
            return temp.ToString();
        }
    }
}
