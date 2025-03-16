using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SelectRegionForDbd
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            GetData();
        }

        private void GetData()
        {
            GetPing("gamelift.eu-central-1.amazonaws.com", FrankfurtPing);
            GetPing("gamelift.eu-west-1.amazonaws.com", IrelandPing);
            GetPing("gamelift.eu-west-2.amazonaws.com", LondonPing);
            GetPing("gamelift.ap-south-1.amazonaws.com", MumbaiPing);
            GetPing("gamelift.ap-northeast-2.amazonaws.com", SeoulPing);
            GetPing("gamelift.ap-southeast-1.amazonaws.com", SingaporePing);
            GetPing("gamelift.ap-southeast-2.amazonaws.com", SydneyPing);
            GetPing("gamelift.ap-northeast-1.amazonaws.com", TokyoPing);

        }

        private async void GetPing(string host, Label label)
        {
            int port = 443;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    stopwatch.Start();
                    await client.ConnectAsync(host, port);
                    stopwatch.Stop();

                    label.Text = $"{stopwatch.ElapsedMilliseconds} ms";
                    label.ForeColor = stopwatch.ElapsedMilliseconds < 100 ? Color.Green :
                                      stopwatch.ElapsedMilliseconds < 200 ? Color.Orange : Color.Red;
                }
            }
            catch (Exception)
            {
                label.Text = "Error";
                label.ForeColor = Color.Red;
            }
        }
    }
}
