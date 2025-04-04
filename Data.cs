using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SelectRegionForDbd
{
    public static class Data
    {
        // Поиск правил брандмауэра и отображения результата на форме
        public static void GetFirewallRuleStatus(Label label)
        {
            try
            {
                string powerShellCommandInBound = "Get-NetFirewallRule | Where-Object { $_.DisplayName -match 'DbdBlockRule' } | Select-Object -First 1";
                string powerShellCommandOutBound = "Get-NetFirewallRule | Where-Object { $_.DisplayName -match 'DbdBlockRule' } | Select-Object -First 1";
                bool resultInBound = PowerShell.Command(powerShellCommandInBound);
                bool resultOutBound = PowerShell.Command(powerShellCommandOutBound);
                if (resultInBound && resultOutBound)
                {
                    label.Text = "Rules are found";
                    label.Location = new Point(500 - (label.Width / 2), 50);
                    label.ForeColor = Color.Red;
                }
                else
                {
                    label.Text = "No rules found";
                    label.Location = new Point(500 - (label.Width / 2), 50);
                    label.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when validating a rule: {ex.Message}");
            }
        }
        // Экспорт правил брандмауэра в файлы
        public static async Task ExportFirewallRule(string ExcludedRegion, string FullPath, string Platfrom)
        {
            string url = "https://ip-ranges.amazonaws.com/ip-ranges.json";
            string filePathIn = $"IP_ranges_for_{Platfrom}_in.txt";
            string filePathOut = $"IP_ranges_for_{Platfrom}_out.txt";
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
                string json = await client.GetStringAsync(url);
                using JsonDocument doc = JsonDocument.Parse(json);
                var prefixes = doc.RootElement.GetProperty("prefixes");
                List<string> allIps = [];
                foreach (var entry in prefixes.EnumerateArray())
                {
                    string region = entry.GetProperty("region").GetString()!;
                    if (allowedRegions.Contains(region) && region != ExcludedRegion)
                    {
                        if (entry.TryGetProperty("ip_prefix", out var ipPrefix) && ipPrefix.GetString()?.Contains('.') == true)
                        {
                            allIps.Add(ipPrefix.GetString()!);
                        }
                    }
                }
                string resultIn = string.Join(",", allIps);
                string resultOut = string.Join(",", allIps);
                string ruleIn = $"New-NetFirewallRule -Name \"DbdBlockRule{Platfrom}_IN\" -DisplayName \"DbdBlockRule{Platfrom}_IN\" -Direction Inbound -Action Block -Program \"{FullPath}\" -RemoteAddress ";
                string ruleOut = $"New-NetFirewallRule -Name \"DbdBlockRule{Platfrom}_OUT\" -DisplayName \"DbdBlockRule{Platfrom}_OUT\" -Direction Out -Action Block -Program \"{FullPath}\" -RemoteAddress ";
                resultIn = ruleIn + resultIn;
                resultOut = ruleOut + resultOut;
                await File.WriteAllTextAsync(filePathIn, resultIn);
                await File.WriteAllTextAsync(filePathOut, resultOut);
                MessageBox.Show($"The rules have been successfully exported");
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
        // Добавление правил брандмауэра
        public static async Task CreateFirewallRule(string ExcludedRegion, string FullPath, Label Status, string Platform)
        {
            bool hosts = Hosts.Write(ExcludedRegion);
            if (hosts)
            {
                string url = "https://ip-ranges.amazonaws.com/ip-ranges.json";
                string scriptPathIn = "firewall_in.ps1";
                string scriptPathOut = "firewall_out.ps1";
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
                    string json = await client.GetStringAsync(url);
                    using JsonDocument doc = JsonDocument.Parse(json);
                    var prefixes = doc.RootElement.GetProperty("prefixes");
                    List<string> allIps = [];
                    foreach (var entry in prefixes.EnumerateArray())
                    {
                        string region = entry.GetProperty("region").GetString()!;
                        if (allowedRegions.Contains(region) && region != ExcludedRegion)
                        {
                            if (entry.TryGetProperty("ip_prefix", out var ipPrefix) && ipPrefix.GetString()?.Contains('.') == true)
                            {
                                allIps.Add(ipPrefix.GetString()!);
                            }
                        }
                    }
                    string resultIn = string.Join(",", allIps);
                    string resultOut = string.Join(",", allIps);
                    string ruleIn = $"New-NetFirewallRule -Name \"DbdBlockRule{Platform}_IN\" -DisplayName \"DbdBlockRule{Platform}_IN\" -Direction Inbound -Action Block -Program \"{FullPath}\" -RemoteAddress ";
                    string ruleOut = $"New-NetFirewallRule -Name \"DbdBlockRule{Platform}_OUT\" -DisplayName \"DbdBlockRule{Platform}_OUT\" -Direction Out -Action Block -Program \"{FullPath}\" -RemoteAddress ";
                    resultIn = ruleIn + resultIn;
                    resultOut = ruleOut + resultOut;
                    await File.WriteAllTextAsync(scriptPathIn, resultIn);
                    await File.WriteAllTextAsync(scriptPathOut, resultOut);
                    bool resultInCommand = PowerShell.Script(scriptPathIn);
                    bool resultOutCommand = PowerShell.Script(scriptPathOut);
                    if (resultInCommand && resultOutCommand)
                    {
                        MessageBox.Show("Rules have been successfully added");
                        Data.FlushDnsCache();
                        Data.GetFirewallRuleStatus(Status);
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
        }
        // Удаление правил брандмауэра
        public static void DeleteFirewallRule(string region, Label Status, string Platform)
        {
            bool hosts = Hosts.Remove(region);
            if (hosts)
            {
                string powerShellCommandInBound = $"Remove-NetFirewallRule -DisplayName 'DbdBlockRule{Platform}_IN'";
                string powerShellCommandOutBound = $"Remove-NetFirewallRule -DisplayName 'DbdBlockRule{Platform}_OUT'";
                PowerShell.Command(powerShellCommandInBound);
                PowerShell.Command(powerShellCommandOutBound);
                bool isInBoundRuleRemoved = !CheckingForTheExistenceOfRules($"DbdBlockRule{Platform}_IN");
                bool isOutBoundRuleRemoved = !CheckingForTheExistenceOfRules($"DbdBlockRule{Platform}_OUT");
                if (isInBoundRuleRemoved && isOutBoundRuleRemoved)
                {
                    MessageBox.Show("Rules have been successfully removed");
                    FlushDnsCache();
                    Data.GetFirewallRuleStatus(Status);
                }
                else
                {
                    MessageBox.Show("Rules not found or could not be removed");
                }
            }
        }
        // Получение задержки до разных серверов Amazon GameLift Servers
        public static async void GetPing(string host, Label label)
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
        // Очистка DNS кэша
        public static void FlushDnsCache()
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
        // Проверка на существование правила
        private static bool CheckingForTheExistenceOfRules(string ruleName)
        {
            string command = $"Get-NetFirewallRule | Where-Object {{ $_.DisplayName -eq '{ruleName}' }}";
            return PowerShell.Command(command);
        }
    }
}
