using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace SilentPowershell
{
    class Program
    {
        private static System.IO.StreamWriter file;
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

        static void Main(string[] args)
        {
            var powershellPath = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            // System.Console.WriteLine(commandLineWithoutCommand());
            var processStartInfo = new ProcessStartInfo(powershellPath, commandLineWithoutCommand());
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            var process = Process.Start(processStartInfo);

            var logFile = Path.Combine(Path.GetTempPath(), "SilentPowerShell.log");
            using (file = new System.IO.StreamWriter(logFile, true))
            {
                process.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                process.ErrorDataReceived  += new DataReceivedEventHandler(ErrorDataHandler);

                process.StandardInput.Close();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit();
                file.WriteLine("{0} EXIT: {1}",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), process.ExitCode);
            }
            Environment.Exit(process.ExitCode);
        }

        private static void OutputDataHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                file.WriteLine("{0} OUT: {1}",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), outLine.Data);
            }
        }

        private static void ErrorDataHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                file.WriteLine("{0} ERR: {1}",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), outLine.Data);
            }
        }
    }
}
