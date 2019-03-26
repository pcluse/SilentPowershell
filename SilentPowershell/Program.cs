using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SilentPowershell
{
    class Program
    {
        private static bool logging = false;
        private static string output = "";

        static string commandLineWithoutCommand()
        {
            string commandLine = Environment.CommandLine;
            int startIndex = 0;
            // If command line starts with " start searching for first
            // space after the second "
            if (commandLine[0] == '"')
            {
                startIndex = commandLine.IndexOf('"', 1) + 1;
                if (startIndex >= commandLine.Length)
                {
                    return "";
                }
            }
            
            int nextSpaceIndex = commandLine.IndexOf(' ', startIndex);
            if (nextSpaceIndex == -1)
            {
                return "";
            }
            return commandLine.Substring(nextSpaceIndex + 1);
        }

        static private bool IsLogging()
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey("Software\\PLS\\SilentPowershell");
                if (key != null)
                {
                    return key.GetValue("Logging").ToString() == "1";
                }
            }
            catch { }
            return false;
        }

        static void Main(string[] args)
        {
            logging = IsLogging();
            
            if (logging)
            {
                output += String.Format("{0} CALL: {1}\r\n",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Environment.CommandLine);
                output += String.Format("{0} USER: {1}\r\n",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            }
            var powershellPath = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            // System.Console.WriteLine(commandLineWithoutCommand());
            var processStartInfo = new ProcessStartInfo(powershellPath, commandLineWithoutCommand());
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            var process = Process.Start(processStartInfo);
           
            if (logging) {
                process.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                process.ErrorDataReceived  += new DataReceivedEventHandler(ErrorDataHandler);
            }
            process.StandardInput.Close();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();

            if (logging)
            {
                var logFile = Path.Combine(Path.GetTempPath(), "SilentPowerShell.log");
                output += String.Format("{0} EXIT: {1}\r\n---------------------------------\r\n",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), process.ExitCode);

                using (var mutex = new Mutex(false, "silentpowershell.pc.lu.se"))
                {
                    mutex.WaitOne();
                    File.AppendAllText(logFile, output);
                    mutex.ReleaseMutex();
                }
            }
            Environment.Exit(process.ExitCode);
        }

        private static void OutputDataHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                output += String.Format("{0} OUT: {1}\r\n",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), outLine.Data);
            }
        }

        private static void ErrorDataHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                output += String.Format("{0} ERR: {1}\r\n",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), outLine.Data);
            }
        }
    }
}
