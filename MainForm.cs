using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text.Json;

namespace SelectRegionForDbd
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            CheckFirewallRule(Status);
            FilePath.Select(0, 0);
        }

        private void CheckFirewallRule(Label label)
        {
            try
            {
                // ������� ��� �������� ������� ������ �� ������
                string powerShellCommandInBound = "Get-NetFirewallRule | Where-Object { $_.DisplayName -eq 'DbdBlockRule_IN' } | Select-Object -First 1";
                string powerShellCommandOutBound = "Get-NetFirewallRule | Where-Object { $_.DisplayName -eq 'DbdBlockRule_OUT' } | Select-Object -First 1";

                // �������� ������� ������
                bool resultInBound = RunPowerShellCommand(powerShellCommandInBound);
                bool resultOutBound = RunPowerShellCommand(powerShellCommandOutBound);

                // ��������� ����� Label � ����������� �� �����������
                if (resultInBound && resultOutBound)
                {
                    label.Text = "Rules are found";
                    label.ForeColor = Color.Red;
                }
                else
                {
                    label.Text = "No rules found";
                    label.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� �������� �������: {ex.Message}");
            }
        }

        private bool RunPowerShellCommand(string command)
        {
            // ���� � PowerShell
            string powerShellPath = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            try
            {
                ProcessStartInfo pro = new ProcessStartInfo
                {
                    FileName = powerShellPath,
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"  // ������ � ������� ��������������
                };

                using (Process process = Process.Start(pro)!)
                {
                    // ������ ������
                    string error = process.StandardError.ReadToEnd();

                    if (process == null) return false;

                    // ������ ���������� ���������� PowerShell �������
                    using (System.IO.StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();  // �������� ���� �����
                        process.WaitForExit();  // ���������� ���������� ��������

                        if (!string.IsNullOrEmpty(error))
                        {
                            string log = "PowerShellLog.txt";
                            File.WriteAllTextAsync(log, error);
                        }

                        // ���� ��������� ������, ��� ������, ��� ������� �� �������
                        return !string.IsNullOrEmpty(result);
                    }
                }
            }
            catch (Exception ex)
            {
                // ���� � �����
                string logPath = "PowerShellMainLog.txt";

                File.WriteAllTextAsync(logPath, ex.Message);
                return false;
            }
        }

        private async Task SaveIps(string excludedRegion)
        {
            string url = "https://ip-ranges.amazonaws.com/ip-ranges.json";
            string filePathIn = "IP_ranges_in.txt";  // ���� � �����, ��� ����� ��������� IP-������
            string filePathOut = "IP_ranges_out.txt";  // ���� � �����, ��� ����� ��������� IP-������
            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);  // �������� JSON-������
                using JsonDocument doc = JsonDocument.Parse(json);  // ������ JSON

                // �������� ������ IP-����������
                var prefixes = doc.RootElement.GetProperty("prefixes");

                // ������ ��� �������� ���� IP-�������
                List<string> allIps = new List<string>();

                // �������� �� ���� ��������� � ��������� �� ��������
                foreach (var entry in prefixes.EnumerateArray())
                {
                    string region = entry.GetProperty("region").GetString()!;
                    if (region != excludedRegion)  // ��������� ��������� ������
                    {
                        // ���������, ��� ���� ���� ip_prefix � ��� ������������� IPv4
                        if (entry.TryGetProperty("ip_prefix", out var ipPrefix) && ipPrefix.GetString()?.Contains(".") == true)
                        {
                            allIps.Add(ipPrefix.GetString()!);
                        }
                    }
                }

                // �������� ��� IP-������ � ������
                string resultIn = string.Join(",", allIps);
                string resultOut = string.Join(",", allIps);
                // ��� ������� ��������� �������
                string ruleIn = $"New-NetFirewallRule -Name \"DbdBlockRule_IN\" -DisplayName \"DbdBlockRule_IN\" -Direction Inbound -Action Block -Program \"{FilePath.Text}\" -RemoteAddress ";
                string ruleOut = $"New-NetFirewallRule -Name \"DbdBlockRule_OUT\" -DisplayName \"DbdBlockRule_OUT\" -Direction Out -Action Block -Program \"{FilePath.Text}\" -RemoteAddress ";
                // ��������� ������� � IP-��������
                resultIn = ruleIn + resultIn;
                resultOut = ruleOut + resultOut;
                // ��������� � ����
                await File.WriteAllTextAsync(filePathIn, resultIn);
                await File.WriteAllTextAsync(filePathOut, resultOut);
                // ������ ������� �� ������
                string commandIn = File.ReadAllText(filePathIn);
                string commandOut = File.ReadAllText(filePathOut);
                // ���������� ������
                bool resultInCommand = RunPowerShellCommand(commandIn);
                bool resultOutCommand = RunPowerShellCommand(commandOut);

                if (resultInCommand && resultOutCommand)
                {
                    MessageBox.Show("Rules have been successfully added");
                    FlushDnsCache();
                    CheckFirewallRule(Status);
                    // ������� ����� ����� ����������
                    if (File.Exists(filePathIn)) File.Delete(filePathIn);
                    if (File.Exists(filePathOut)) File.Delete(filePathOut);
                }
                else
                {
                    MessageBox.Show("Failed to add rules");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error while requesting data: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void FlushDnsCache()
        {
            try
            {
                // �������� ������� ��� ������� cmd
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe", // ������ ��������� ������
                    Arguments = "/C ipconfig /flushdns", // �������� /C ��������� ������� � ��������� cmd
                    CreateNoWindow = true, // �������� ���� ��������� ������
                    UseShellExecute = false, // �� ������������ �������� ��� ����������
                    RedirectStandardOutput = true // �������������� �����
                };

                // ������ ��������
                using (Process process = Process.Start(processStartInfo)!)
                {
                    process.WaitForExit(); // ������� ���������� �������
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        // ����� ��� ��������, ���������� �� �������
        private bool IsRulePresent(string ruleName)
        {
            string command = $"Get-NetFirewallRule | Where-Object {{ $_.DisplayName -eq '{ruleName}' }}";
            return RunPowerShellCommand(command);
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Select file DeadByDaylight-Win64-Shipping.exe";
            openFileDialog.Filter = "Executable files (DeadByDaylight-Win64-Shipping.exe)|DeadByDaylight-Win64-Shipping.exe";

            // ���� ���� ������, ��������� ��� ���� � ������
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                FilePath.Text = $"{filePath}";
            }
        }

        private async void btnCreateRules_Click(object sender, EventArgs e)
        {
            if (ServersBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a region");
                return;
            }
            if (FilePath.Text == string.Empty)
            {
                MessageBox.Show("Please specify the path to the executable file");
                return;
            }

            string selectedRegion = ServersBox.SelectedItem.ToString()!;
            await SaveIps(selectedRegion);
        }

        private void btnRemoveRules_Click(object sender, EventArgs e)
        {
            // ������� ��� �������� ������
            string powerShellCommandInBound = "Remove-NetFirewallRule -DisplayName 'DbdBlockRule_IN'";
            string powerShellCommandOutBound = "Remove-NetFirewallRule -DisplayName 'DbdBlockRule_OUT'";

            // ���������� ������ ��� �������� ������
            bool resultInBound = RunPowerShellCommand(powerShellCommandInBound);
            bool resultOutBound = RunPowerShellCommand(powerShellCommandOutBound);

            // ������������� ���������, ��� ������� ������������� �������
            bool isInBoundRuleRemoved = !IsRulePresent("DbdBlockRule_IN");
            bool isOutBoundRuleRemoved = !IsRulePresent("DbdBlockRule_OUT");

            // ���������, ���� �� ������� �������
            if (isInBoundRuleRemoved && isOutBoundRuleRemoved)
            {
                MessageBox.Show("Rules have been successfully removed");
                FlushDnsCache();
                CheckFirewallRule(Status);
            }
            else
            {
                MessageBox.Show("Rules not found or could not be removed");
            }
        }
    }
}
