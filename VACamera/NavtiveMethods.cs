using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace VACamera
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

        public static bool IsWin64Emulator(Process process)
        {
            if ((Environment.OSVersion.Version.Major > 5)
                || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1)))
            {
                bool retVal;

                return IsWow64Process(process.Handle, out retVal) && retVal;
            }

            return false; // not on 64-bit Windows Emulator
        }
    }
}
