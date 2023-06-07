using Caster.Modules;
using Newtonsoft.Json;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Caster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string path = Path.Combine(projectDirectory, "Data", "server.json");

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var servers = JsonConvert.DeserializeObject<List<ServerInfo>>(json);
                ServerList.ItemsSource = servers;
            }
        }

        private void ServerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // AddServerButton_Click
        private void AddServerButton_Click(object sender, RoutedEventArgs e)
        {
            AddServerWindow addServerWindow = new AddServerWindow();
            addServerWindow.ShowDialog(); // ShowDialog will block the current thread until the window is closed

            // Reload data from server.json and update ListBox
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string path = Path.Combine(projectDirectory, "Data", "server.json");

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var servers = JsonConvert.DeserializeObject<List<ServerInfo>>(json);
                ServerList.ItemsSource = servers;
            }
        }

        // RemoveServerButton_Click
        private void RemoveServerButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取选中的项
            ServerInfo selectedServer = (ServerInfo)ServerList.SelectedItem;

            if (selectedServer == null)
            {
                // 如果没有选中任何项，则直接返回
                return;
            }

            // 读取现有的数据
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string path = Path.Combine(projectDirectory, "Data", "server.json");
            List<ServerInfo> servers = new List<ServerInfo>();
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var deserialized = JsonConvert.DeserializeObject<List<ServerInfo>>(json);
                servers = deserialized ?? servers;
            }
            // 移除选中的项
            servers.Remove(selectedServer);

            // 将列表序列化为JSON格式
            string newJson = JsonConvert.SerializeObject(servers, Newtonsoft.Json.Formatting.Indented);

            // 写入到文件中
            File.WriteAllText(path, newJson);

            // 更新ListBox的数据源
            ServerList.ItemsSource = null;
            ServerList.ItemsSource = servers;
        }

        // DeployButotn_Click
        private void DeployButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取选中的项
            ServerInfo selectedServer = (ServerInfo)ServerList.SelectedItem;
            Test_SSH_Connection(selectedServer);
        }

        private void Test_SSH_Connection(ServerInfo serverInfo)
        {
            string keyPath = Path.Combine(@"C:\Users\" + Environment.UserName + @"\.ssh\", serverInfo.PrivateKeyFileName);

            // Create a new instance of the PrivateKeyFile class with the path to the private key file
            var keyFile = new PrivateKeyFile(keyPath);
            var keyFiles = new[] { keyFile };

            // Create a new instance of the ConnectionInfo class with the connection details
            var connectionInfo = new ConnectionInfo(
                serverInfo.Host,
                serverInfo.Username,
                new PrivateKeyAuthenticationMethod(serverInfo.Username, keyFiles));

            // Create a new instance of the SshClient class with the connection info
            using (var client = new SshClient(connectionInfo))
            {
                try
                {
                    // Connect to the SSH server
                    client.Connect();

                    // Check if the client is connected
                    if (client.IsConnected)
                    {
                        MessageBox.Show("Connection successful!");
                    }
                    else
                    {
                        MessageBox.Show("Connection failed!");
                    }
                }
                catch (Exception e)
                {
                    // Handle any exceptions that occur during the connection attempt
                    MessageBox.Show($"Connection failed: {e.Message}");
                }
                finally
                {
                    // Disconnect from the SSH server
                    if (client.IsConnected)
                    {
                        client.Disconnect();
                    }
                }
            }
        }


    }
}
