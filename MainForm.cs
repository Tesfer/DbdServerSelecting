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
            FilePath.Select(0, 0);
        }

        private async Task<string?> GetSingleIpForRegion(string region)
        {
            string url = "https://ip-ranges.amazonaws.com/ip-ranges.json";

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);
                using JsonDocument doc = JsonDocument.Parse(json);

                // Получаем массив IP-диапазонов
                var prefixes = doc.RootElement.GetProperty("prefixes");

                // Фильтруем только для конкретного региона и только по полю ip_prefix (IPv4)
                var filteredIp = prefixes.EnumerateArray()
                    .Where(entry =>
                        entry.GetProperty("region").GetString() == region &&  // Фильтруем только по региону
                        entry.TryGetProperty("ip_prefix", out var ipPrefix))  // Проверяем наличие поля ip_prefix (IPv4)
                    .Select(entry => entry.GetProperty("ip_prefix").GetString())   // Получаем ip_prefix
                    .FirstOrDefault(); // Получаем первый из найденных

                return filteredIp;  // Если IP найден, возвращаем его, иначе null
            }
            catch (Exception)
            {
                return null;  // Если произошла ошибка, возвращаем null
            }
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Выберите файл DeadByDaylight-Win64-Shipping.exe";
            openFileDialog.Filter = "Исполнимые файлы (DeadByDaylight-Win64-Shipping.exe)|DeadByDaylight-Win64-Shipping.exe";

            // Если файл выбран, сохраняем его путь в строку
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                FilePath.Text = $"{filePath}";
            }
        }
    }
}
