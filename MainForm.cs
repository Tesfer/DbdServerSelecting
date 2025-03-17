using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace SelectRegionForDbd
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Themes.Dark(this, btnCreateRules, btnRemoveRules, btnSelectFile, FilePath, ServersBox);
            GetPing();
            CheckFirewallRule(Status);
            FilePath.Select(0, 0);
        }

        private static async void Ping(string host, Label label)
        {
            try
            {
                using Ping ping = new();
                PingReply reply = await ping.SendPingAsync(host);
                if (reply.Status == IPStatus.Success)
                {
                    label.Text = $"{reply.RoundtripTime} ms";
                    label.ForeColor = reply.RoundtripTime < 100 ? Color.Green :
                                      reply.RoundtripTime < 200 ? Color.Orange : Color.Red;
                }
                else
                {
                    label.Text = "-1";
                    label.ForeColor = Color.Red;
                }
            }
            catch
            {
                label.Text = "Error";
                label.ForeColor = Color.Red;
            }
        }

        private void GetPing()
        {
            Ping("gamelift.eu-central-1.amazonaws.com", FrankfurtPing);
            Ping("gamelift.eu-west-1.amazonaws.com", IrelandPing);
            Ping("gamelift.eu-west-2.amazonaws.com", LondonPing);
            Ping("gamelift.ca-central-1.amazonaws.com", CentralPing);
            Ping("gamelift.ap-south-1.amazonaws.com", MumbaiPing);
            Ping("gamelift.ap-northeast-2.amazonaws.com", SeoulPing);
            Ping("gamelift.ap-southeast-1.amazonaws.com", SingaporePing);
            Ping("gamelift.ap-southeast-2.amazonaws.com", SydneyPing);
            Ping("gamelift.ap-northeast-1.amazonaws.com", TokyoPing);
            Ping("gamelift.us-east-2.amazonaws.com", OhioPing);
            Ping("gamelift.us-east-1.amazonaws.com", VirginiaPing);
            Ping("gamelift.us-west-1.amazonaws.com", CaliforniaPing);
            Ping("gamelift.us-west-2.amazonaws.com", OregonPing);
            Ping("gamelift.sa-east-1.amazonaws.com", PauloPing);
        }
        private static void CheckFirewallRule(Label label)
        {
            try
            {
                string powerShellCommandInBound = "Get-NetFirewallRule | Where-Object { $_.DisplayName -eq 'DbdBlockRule_IN' } | Select-Object -First 1";
                string powerShellCommandOutBound = "Get-NetFirewallRule | Where-Object { $_.DisplayName -eq 'DbdBlockRule_OUT' } | Select-Object -First 1";
                bool resultInBound = RunPowerShellCommand(powerShellCommandInBound);
                bool resultOutBound = RunPowerShellCommand(powerShellCommandOutBound);
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
                MessageBox.Show($"Ошибка при проверке правила: {ex.Message}");
            }
        }

        private static bool RunPowerShellCommand(string command)
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

        private void DeleteRules()
        {
            string powerShellCommandInBound = "Remove-NetFirewallRule -DisplayName 'DbdBlockRule_IN'";
            string powerShellCommandOutBound = "Remove-NetFirewallRule -DisplayName 'DbdBlockRule_OUT'";
            RunPowerShellCommand(powerShellCommandInBound);
            RunPowerShellCommand(powerShellCommandOutBound);
            bool isInBoundRuleRemoved = !IsRulePresent("DbdBlockRule_IN");
            bool isOutBoundRuleRemoved = !IsRulePresent("DbdBlockRule_OUT");
            if (isInBoundRuleRemoved && isOutBoundRuleRemoved)
            {
                MessageBox.Show("Rules have been successfully removed");
                FlushDnsCache();
                GetPing();
                CheckFirewallRule(Status);
            }
            else
            {
                MessageBox.Show("Rules not found or could not be removed");
            }
        }

        private async Task CreateRules(string excludedRegion)
        {
            string url = "https://ip-ranges.amazonaws.com/ip-ranges.json";
            // string filePathIn = "IP_ranges_in.txt";
            // string filePathOut = "IP_ranges_out.txt"; 
            string scriptPathIn = "firewall_in.ps1";
            string scriptPathOut = "firewall_out.ps1";
            // Список разрешенных регионов
            HashSet<string> allowedRegions =
            [
                "us-east-2", "us-west-1", "us-west-2",
                "ap-south-1", "ap-northeast-2", "ap-southeast-1", "ap-southeast-2",
                "ap-northeast-1", "ca-central-1", "eu-central-1", "eu-west-1",
                "eu-west-2", "sa-east-1"
            ];

            try
            {
                using HttpClient client = new();
                string json = await client.GetStringAsync(url);  // Получаем JSON-строку
                using JsonDocument doc = JsonDocument.Parse(json);  // Парсим JSON
                // Получаем массив IP-диапазонов
                var prefixes = doc.RootElement.GetProperty("prefixes");
                List<string> allIps = [];
                foreach (var entry in prefixes.EnumerateArray())
                {
                    string region = entry.GetProperty("region").GetString()!;

                    // Если регион в списке разрешенных и не исключен
                    if (allowedRegions.Contains(region) && region != excludedRegion)
                    {
                        // Проверяем, что есть поле ip_prefix и оно соответствует IPv4
                        if (entry.TryGetProperty("ip_prefix", out var ipPrefix) && ipPrefix.GetString()?.Contains('.') == true)
                        {
                            allIps.Add(ipPrefix.GetString()!);
                        }
                    }
                }
                // Собираем все IP-адреса в строку
                string resultIn = string.Join(",", allIps);
                string resultOut = string.Join(",", allIps);
                // Для правила входящего трафика
                string ruleIn = $"New-NetFirewallRule -Name \"DbdBlockRule_IN\" -DisplayName \"DbdBlockRule_IN\" -Direction Inbound -Action Block -Program \"{FilePath.Text}\" -RemoteAddress ";
                string ruleOut = $"New-NetFirewallRule -Name \"DbdBlockRule_OUT\" -DisplayName \"DbdBlockRule_OUT\" -Direction Out -Action Block -Program \"{FilePath.Text}\" -RemoteAddress ";
                // Соединяем правило с IP-адресами
                resultIn = ruleIn + resultIn;
                resultOut = ruleOut + resultOut;
                // Сохраняем в файл
                // await File.WriteAllTextAsync(filePathIn, resultIn);
                // await File.WriteAllTextAsync(filePathOut, resultOut);
                // Сохраняем команды в скрипты
                await File.WriteAllTextAsync(scriptPathIn, resultIn);
                await File.WriteAllTextAsync(scriptPathOut, resultOut);

                bool resultInCommand = RunPowerShellScript(scriptPathIn);
                bool resultOutCommand = RunPowerShellScript(scriptPathOut);

                if (resultInCommand && resultOutCommand)
                {
                    MessageBox.Show("Rules have been successfully added");
                    FlushDnsCache();
                    GetPing();
                    CheckFirewallRule(Status);
                    // Удаляем файлы после выполнения
                    // if (File.Exists(filePathIn)) File.Delete(filePathIn);
                    // if (File.Exists(filePathOut)) File.Delete(filePathOut);
                    if (File.Exists(scriptPathIn)) File.Delete(scriptPathIn);
                    if (File.Exists(scriptPathOut)) File.Delete(scriptPathOut);
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

        private static bool RunPowerShellScript(string scriptPath)
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
                MessageBox.Show($"Error: {ex.Message}");
            }
            return false;
        }


        private static void FlushDnsCache()
        {
            try
            {
                ProcessStartInfo processStartInfo = new()
                {
                    FileName = "cmd.exe", 
                    Arguments = "/C ipconfig /flushdns", 
                    CreateNoWindow = true, 
                    UseShellExecute = false, 
                    RedirectStandardOutput = true 
                };

                using Process process = Process.Start(processStartInfo)!;
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private static bool IsRulePresent(string ruleName)
        {
            string command = $"Get-NetFirewallRule | Where-Object {{ $_.DisplayName -eq '{ruleName}' }}";
            return RunPowerShellCommand(command);
        }

        private void BtnSelectFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Select file DeadByDaylight-Win64-Shipping.exe";
            openFileDialog.Filter = "Executable files (DeadByDaylight-Win64-Shipping.exe)|DeadByDaylight-Win64-Shipping.exe";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                FilePath.Text = $"{filePath}";
            }
        }

        private async void BtnCreateRules_Click(object sender, EventArgs e)
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
            await CreateRules(selectedRegion);
        }

        private void BtnRemoveRules_Click(object sender, EventArgs e)
        {
            DeleteRules();
        }
    }
}
