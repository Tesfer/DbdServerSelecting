using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;

namespace SelectRegionForDbd
{
    public partial class MainForm : Form
    {
        // ������� � ��������� � �� �������
        private readonly Dictionary<string, string> regions = new Dictionary<string, string>
        {
            { "US East (Ohio)", "ec2.us-east-2.amazonaws.com" },
            { "US East (N. Virginia)", "ec2.us-east-1.amazonaws.com" },
            { "US West (N. California)", "ec2.us-west-1.amazonaws.com" },
            { "US West (Oregon)", "ec2.us-west-2.amazonaws.com" },
            { "Asia Pacific (Mumbai)", "ec2.ap-south-1.amazonaws.com" },
            { "Asia Pacific (Seoul)", "ec2.ap-northeast-2.amazonaws.com" },
            { "Asia Pacific (Singapore)", "ec2.ap-southeast-1.amazonaws.com" },
            { "Asia Pacific (Sydney)", "ec2.ap-southeast-2.amazonaws.com" },
            { "Asia Pacific (Tokyo)", "ec2.ap-northeast-1.amazonaws.com" },
            { "Canada (Central)", "ec2.ca-central-1.amazonaws.com" },
            { "Europe (Frankfurt)", "ec2.eu-central-1.amazonaws.com" },
            { "Europe (Ireland)", "ec2.eu-west-1.amazonaws.com" },
            { "Europe (London)", "ec2.eu-west-2.amazonaws.com" },
            { "South America (Sao Paulo)", "ec2.sa-east-1.amazonaws.com" }
        };

        public MainForm()
        {
            InitializeComponent();
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
    }
}
