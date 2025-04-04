using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SelectRegionForDbd
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            PlatformBox.SelectedIndex = 0;
            Themes.Dark(this, btnCreateRules, btnRemoveRules, btnExportRules, btnSelectFile, FilePath, ServersBox, PlatformBox);
            GetPing();
            Data.GetFirewallRuleStatus(Status);
            FilePath.Select(0, 0);
        }
        // Получение задержки до разных серверов Amazon GameLift Servers
        private void GetPing()
        {
            Data.GetPing("gamelift.eu-central-1.amazonaws.com", FrankfurtPing);
            Data.GetPing("gamelift.eu-west-1.amazonaws.com", IrelandPing);
            Data.GetPing("gamelift.eu-west-2.amazonaws.com", LondonPing);
            Data.GetPing("gamelift.ca-central-1.amazonaws.com", CentralPing);
            Data.GetPing("gamelift.ap-south-1.amazonaws.com", MumbaiPing);
            Data.GetPing("gamelift.ap-northeast-2.amazonaws.com", SeoulPing);
            Data.GetPing("gamelift.ap-southeast-1.amazonaws.com", SingaporePing);
            Data.GetPing("gamelift.ap-southeast-2.amazonaws.com", SydneyPing);
            Data.GetPing("gamelift.ap-northeast-1.amazonaws.com", TokyoPing);
            Data.GetPing("gamelift.us-east-2.amazonaws.com", OhioPing);
            Data.GetPing("gamelift.us-east-1.amazonaws.com", VirginiaPing);
            Data.GetPing("gamelift.us-west-1.amazonaws.com", CaliforniaPing);
            Data.GetPing("gamelift.us-west-2.amazonaws.com", OregonPing);
            Data.GetPing("gamelift.sa-east-1.amazonaws.com", PauloPing);
        }
        // Обработочик для PlatformBox
        private void PlatformBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ServersBox.SelectedIndex = -1;
            FilePath.Text = string.Empty;
            string selected = PlatformBox.SelectedItem?.ToString()!;
            if (selected == "STEAM")
            {
                PathLabel.Text = "Path to DeadByDaylight-Win64-Shipping.exe";
                PathLabel.Location = new Point(500 - (PathLabel.Width / 2), 400);
            }
            else if (selected == "EGS")
            {
                PathLabel.Text = "Path to DeadByDaylight-EGS-Shipping.exe";
                PathLabel.Location = new Point(500 - (PathLabel.Width / 2), 400);
            }
            else
            {
                PathLabel.Text = "Path to DeadByDaylight-WinGDK-Shipping.exe";
                PathLabel.Location = new Point(500 - (PathLabel.Width / 2), 400);
            }
        }
        // Кнопка выбора пути к исполняемому файлу
        private void BtnSelectFile_Click(object sender, EventArgs e)
        {
            string selected = PlatformBox.SelectedItem?.ToString()!;
            if (selected == "STEAM")
            {
                openFileDialog.Title = "Select file DeadByDaylight-Win64-Shipping.exe";
                openFileDialog.Filter = "Executable files (DeadByDaylight-Win64-Shipping.exe)|DeadByDaylight-Win64-Shipping.exe";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    FilePath.Text = $"{filePath}";
                }
            }
            else if (selected == "EGS")
            {
                openFileDialog.Title = "Select file DeadByDaylight-EGS-Shipping.exe";
                openFileDialog.Filter = "Executable files (DeadByDaylight-EGS-Shipping.exe)|DeadByDaylight-EGS-Shipping.exe";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    FilePath.Text = $"{filePath}";
                }
            }
            else
            {
                openFileDialog.Title = "Select file DeadByDaylight-WinGDK-Shipping.exe";
                openFileDialog.Filter = "Executable files (DeadByDaylight-WinGDK-Shipping.exe)|DeadByDaylight-WinGDK-Shipping.exe";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    FilePath.Text = $"{filePath}";
                }
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
