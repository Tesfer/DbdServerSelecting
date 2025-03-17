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
                string powerShellCommandInBound = $"Get-NetFirewallRule | Where-Object {{ $_.DisplayName -eq 'DbdBlockRule_IN' }}";
                string powerShellCommandOutBound = $"Get-NetFirewallRule | Where-Object {{ $_.DisplayName -eq 'DbdBlockRule_OUT' }}";

                bool resultInBound = RunPowerShellCommand(powerShellCommandInBound);
                bool resultOutBound = RunPowerShellCommand(powerShellCommandOutBound);

                if (resultInBound && resultOutBound)
                {
                    label.Text = "Rules found";
                    label.ForeColor = Color.Red;
                }
                else
                {
                    label.Text = "There are no rules";
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
            try
            {
                // ��������� PowerShell ������� ����� Process
                ProcessStartInfo pro = new ProcessStartInfo();
                pro.FileName = "powershell.exe";
                pro.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\""; // ��� ���������� ������� � ��������
                pro.RedirectStandardOutput = true;
                pro.UseShellExecute = false;
                pro.CreateNoWindow = true;

                using (Process process = Process.Start(pro)!)
                {
                    // ������ ���������� ���������� PowerShell �������
                    using (System.IO.StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        process.WaitForExit();  // ���������� ���������� �������� PowerShell

                        // ���� ��������� �� ������, ���������� true
                        return !string.IsNullOrEmpty(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ���������� ������� PowerShell: {ex.Message}");
                return false;
            }
        }


        private async Task<string?> GetSingleIpForRegion(string region)
        {
            string url = "https://ip-ranges.amazonaws.com/ip-ranges.json";

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);
                using JsonDocument doc = JsonDocument.Parse(json);

                // �������� ������ IP-����������
                var prefixes = doc.RootElement.GetProperty("prefixes");

                // ��������� ������ ��� ����������� ������� � ������ �� ���� ip_prefix (IPv4)
                var filteredIp = prefixes.EnumerateArray()
                    .Where(entry =>
                        entry.GetProperty("region").GetString() == region &&  // ��������� ������ �� �������
                        entry.TryGetProperty("ip_prefix", out var ipPrefix))  // ��������� ������� ���� ip_prefix (IPv4)
                    .Select(entry => entry.GetProperty("ip_prefix").GetString())   // �������� ip_prefix
                    .FirstOrDefault(); // �������� ������ �� ���������

                return filteredIp;  // ���� IP ������, ���������� ���, ����� null
            }
            catch (Exception)
            {
                return null;  // ���� ��������� ������, ���������� null
            }
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "�������� ���� DeadByDaylight-Win64-Shipping.exe";
            openFileDialog.Filter = "���������� ����� (DeadByDaylight-Win64-Shipping.exe)|DeadByDaylight-Win64-Shipping.exe";

            // ���� ���� ������, ��������� ��� ���� � ������
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                FilePath.Text = $"{filePath}";
            }
        }
    }
}
