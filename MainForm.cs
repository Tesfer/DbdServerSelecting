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
            Themes.Dark(this, btnCreateRules, btnRemoveRules, btnExportRules, btnSelectFile, FilePath, ServersBox);
            GetPing();
            Data.GetFirewallRuleStatus(Status);
            FilePath.Select(0, 0);
        }
        // Получение задержки до разных серверов Amazon GameLift Servers
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
        // Асинхронный метод для пинга выбранного сервера
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
        // Кнопка выбора пути к файлу DeadByDaylight-Win64-Shipping.exe
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
        // Кнопка создания правил
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
            await Data.CreateFirewallRule(selectedRegion, FilePath.Text, Status);
            GetPing();
        }
        // Кнопка удаления правил
        private void BtnRemoveRules_Click(object sender, EventArgs e)
        {
            if (ServersBox.SelectedItem == null)
            {
                MessageBox.Show("Please select the region to be deleted");
                return;
            }
            string selectedRegion = ServersBox.SelectedItem.ToString()!;
            Data.DeleteFirewallRule(selectedRegion, Status);
            GetPing();
        }
        // Кнопка экспорта правил
        private async void BtnExportRules_Click(object sender, EventArgs e)
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
            string filePath = FilePath.Text;
            await Data.ExportFirewallRule(selectedRegion, filePath);
        }
    }
}
