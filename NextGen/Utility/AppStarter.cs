using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Generator.Utility
{
    class AppStarter
    {
        public static void BrowseTo(string url)
        {
            BrowseTo(url, -1); 
        }
        public static void BrowseTo(string url, int timeout)
        {
            if (OpenFile(url) < 16)
            {
                SpawnCommand("explorer.exe " + url, timeout);
            }
        }
        public static void SpawnCommand(string command, int timeout)
        {
            ExecuteCommand(command, timeout);
        }

        public static int OpenFile(string filename)
        {
            int result = Win32.ShellExecute(TemplateMain.Instance().Handle.ToInt32(), "Open", filename, "", @"C:\", 1);
            return result;
        }
        public static int ExecuteCommand(string command, int timeout)
        {
            int ExitCode = 0;
            ProcessStartInfo ProcessInfo;
            Process Process;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/C " + command);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = false;
            Process = Process.Start(ProcessInfo);
            if (timeout > 0)
            {
                Process.WaitForExit(timeout);
                if (Process.HasExited)
                    ExitCode = Process.ExitCode;
                Process.Close();
            }

            return ExitCode;
        }

 
    }
}
