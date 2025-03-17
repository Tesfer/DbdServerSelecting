using System.ComponentModel.Design;
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
                // Команды для проверки наличия правил по именам
                string powerShellCommandInBound = "Get-NetFirewallRule | Where-Object { $_.DisplayName -eq 'DbdBlockRule_IN' } | Select-Object -First 1";
                string powerShellCommandOutBound = "Get-NetFirewallRule | Where-Object { $_.DisplayName -eq 'DbdBlockRule_OUT' } | Select-Object -First 1";
                // Проверка наличия правил
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

        private bool RunPowerShellCommand(string command)
        {
            try
            {
                ProcessStartInfo pro = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process process = Process.Start(pro)!)
                {
                    if (process == null) return false;
                    using (System.IO.StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        process.WaitForExit();
                        return !string.IsNullOrEmpty(result);
                    }
                }
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
            // Команды для удаления правил
            string powerShellCommandInBound = "Remove-NetFirewallRule -DisplayName 'DbdBlockRule_IN'";
            string powerShellCommandOutBound = "Remove-NetFirewallRule -DisplayName 'DbdBlockRule_OUT'";
            // Выполнение команд для удаления правил
            bool resultInBound = RunPowerShellCommand(powerShellCommandInBound);
            bool resultOutBound = RunPowerShellCommand(powerShellCommandOutBound);
            // Дополнительно проверяем, что правила действительно удалены
            bool isInBoundRuleRemoved = !IsRulePresent("DbdBlockRule_IN");
            bool isOutBoundRuleRemoved = !IsRulePresent("DbdBlockRule_OUT");
            // Проверяем, были ли удалены правила
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

        private async Task CreateRules(string excludedRegion)
        {
            string url = "https://ip-ranges.amazonaws.com/ip-ranges.json";
            // string filePathIn = "IP_ranges_in.txt";
            // string filePathOut = "IP_ranges_out.txt"; 
            string scriptPathIn = "firewall_in.ps1";
            string scriptPathOut = "firewall_out.ps1";
            // Список разрешенных регионов
            HashSet<string> allowedRegions = new HashSet<string>
            {
                "us-east-2", "us-west-1", "us-west-2",
                "ap-south-1", "ap-northeast-2", "ap-southeast-1", "ap-southeast-2",
                "ap-northeast-1", "ca-central-1", "eu-central-1", "eu-west-1",
                "eu-west-2", "sa-east-1"
            };

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);  // Получаем JSON-строку
                using JsonDocument doc = JsonDocument.Parse(json);  // Парсим JSON
                // Получаем массив IP-диапазонов
                var prefixes = doc.RootElement.GetProperty("prefixes");
                List<string> allIps = new List<string>();
                foreach (var entry in prefixes.EnumerateArray())
                {
                    string region = entry.GetProperty("region").GetString()!;

                    // Если регион в списке разрешенных и не исключен
                    if (allowedRegions.Contains(region) && region != excludedRegion)
                    {
                        // Проверяем, что есть поле ip_prefix и оно соответствует IPv4
                        if (entry.TryGetProperty("ip_prefix", out var ipPrefix) && ipPrefix.GetString()?.Contains(".") == true)
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

        private bool RunPowerShellScript(string scriptPath)
        {
            ProcessStartInfo psi = new ProcessStartInfo
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
                using (Process process = Process.Start(psi)!)
                {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            return false;
        }


        private void FlushDnsCache()
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe", 
                    Arguments = "/C ipconfig /flushdns", 
                    CreateNoWindow = true, 
                    UseShellExecute = false, 
                    RedirectStandardOutput = true 
                };

                using (Process process = Process.Start(processStartInfo)!)
                {
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private bool IsRulePresent(string ruleName)
        {
            string command = $"Get-NetFirewallRule | Where-Object {{ $_.DisplayName -eq '{ruleName}' }}";
            return RunPowerShellCommand(command);
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Select file DeadByDaylight-Win64-Shipping.exe";
            openFileDialog.Filter = "Executable files (DeadByDaylight-Win64-Shipping.exe)|DeadByDaylight-Win64-Shipping.exe";
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
            await CreateRules(selectedRegion);
        }

        private void btnRemoveRules_Click(object sender, EventArgs e)
        {
            DeleteRules();
        }
    }
}
