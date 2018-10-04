using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SilentPowershell
{
    class Program
    {
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
            string powershellPath = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            // System.Console.WriteLine(commandLineWithoutCommand());
            ProcessStartInfo processStartInfo = new ProcessStartInfo(powershellPath, commandLineWithoutCommand());
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            Process process = Process.Start(processStartInfo);
            process.StandardInput.Close();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
            Environment.Exit(process.ExitCode);
        }
    }
}
