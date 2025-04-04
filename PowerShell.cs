using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectRegionForDbd
{
    public static class PowerShell
    {
        public static bool Command(string command)
        {
            try
            {
                ProcessStartInfo pro = new()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };
                using Process process = Process.Start(pro)!;
                if (process == null) return false;
                using System.IO.StreamReader reader = process.StandardOutput;
                string result = reader.ReadToEnd();
                process.WaitForExit();
                return !string.IsNullOrEmpty(result);
            }
            catch (Exception ex)
            {
                // Путь к логам
                string logPath = "PowerShellLog.txt";
                File.WriteAllTextAsync(logPath, ex.Message);
                return false;
            }
        }

        public static bool Script(string scriptPath)
        {
            ProcessStartInfo psi = new()
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Verb = "runas"
            };
            try
            {
                using Process process = Process.Start(psi)!;
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Путь к логам
                string logPath = "PowerShellLog.txt";
                File.WriteAllTextAsync(logPath, ex.Message);
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
            return false;
        }
    }
}
