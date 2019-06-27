using System;
using System.Diagnostics;
using System.IO;

namespace RemoteCam.Pi
{
    public static class Extensions
    {
        public static void RunAsTerminalCommand(this String command)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "fswebcam", Arguments = command, };
            Process camProcess = new Process() { StartInfo = startInfo, };
            camProcess.Start();
            camProcess.WaitForExit();
        }
        public static string ToBase64(this FileInfo fileInfo)
        {
            return Convert.ToBase64String(File.ReadAllBytes(fileInfo.FullName));
        }
    }   
}
