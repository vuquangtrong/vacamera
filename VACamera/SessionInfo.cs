using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VACamera
{
    public class SessionInfo
    {
        public string Name1 = "";
        public string Name2 = "";
        public string Name3 = "";
        public string Name4 = "";
        public string Name5 = "";
        public string DateTime = "";
        public int MaxTime = 60 * 60; // 1 hour
        public string License = "";

        override
        public string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Environment.NewLine + "[" + Environment.NewLine);
            stringBuilder.Append("Name1 = " + Name1 + Environment.NewLine);
            stringBuilder.Append("Name2 = " + Name2 + Environment.NewLine);
            stringBuilder.Append("Name3 = " + Name3 + Environment.NewLine);
            stringBuilder.Append("Name4 = " + Name4 + Environment.NewLine);
            stringBuilder.Append("Name5 = " + Name5 + Environment.NewLine);
            stringBuilder.Append("DateTime = " + DateTime + Environment.NewLine);
            stringBuilder.Append("MaxTime = " + MaxTime + Environment.NewLine);
            stringBuilder.Append("]" + Environment.NewLine);

            return stringBuilder.ToString();
        }
    }
}
