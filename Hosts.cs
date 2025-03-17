using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SelectRegionForDbd
{
    public static class Hosts
    {
        static readonly string hostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";

        public static bool Write(string region)
        {
            string linesToAdd;
            switch (region)
            {
                case "eu-central-1":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
#0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "eu-west-1":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
#0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "eu-west-2":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
#0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "us-east-1":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "us-east-2":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
#0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "us-west-1":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
#0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "us-west-2":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
#0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "ca-central-1":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
#0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "ap-south-1":
                    linesToAdd = @"
#asia
#0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "ap-northeast-1":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
#0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "ap-northeast-2":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
#0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "ap-southeast-1":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
#0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "ap-southeast-2":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
#0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                case "sa-east-1":
                    linesToAdd = @"
#asia
0.0.0.0 gamelift.ap-south-1.amazonaws.com # Asia Pacific (Mumbai) ap-south-1
0.0.0.0 gamelift.ap-east-1.amazonaws.com # Asia Pacific (Hong Kong)    ap-east-1
0.0.0.0 gamelift.ap-northeast-1.amazonaws.com # Asia Pacific (Tokyo)    ap-northeast-1
0.0.0.0 gamelift.ap-northeast-2.amazonaws.com # Asia Pacific (Seoul)    ap-northeast-2
0.0.0.0 gamelift.ap-southeast-1.amazonaws.com # Asia Pacific (Singapore)    ap-southeast-1
0.0.0.0 gamelift.ap-southeast-2.amazonaws.com # Asia Pacific (Sydney)    ap-southeast-2
#america
0.0.0.0 gamelift.ca-central-1.amazonaws.com # Canada (Central)    ca-central-1
#0.0.0.0 gamelift.us-east-1.amazonaws.com # US East (N. Virginia)    us-east-1
0.0.0.0 gamelift.us-east-2.amazonaws.com # US East (Ohio)    us-east-2
0.0.0.0 gamelift.us-west-1.amazonaws.com # US West (N. California)    us-west-1
0.0.0.0 gamelift.us-west-2.amazonaws.com # US West (Oregon)    us-west-2
#0.0.0.0 gamelift.sa-east-1.amazonaws.com # South America (Sao Paulo)    sa-east-1
#europe
0.0.0.0 gamelift.eu-central-1.amazonaws.com # Europe (Frankfurt)    eu-central-1
0.0.0.0 gamelift.eu-west-1.amazonaws.com # Europe (Ireland)    eu-west-1
0.0.0.0 gamelift.eu-west-2.amazonaws.com # Europe (London)    eu-west-2";
                    try
                    {
                        using StreamWriter sw = new(hostsFilePath, true);
                        sw.WriteLine(linesToAdd);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to modify the hosts file. Run the application as administrator");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        return false;
                    }
                default:
                    return false;
            }
        }
    }
}
