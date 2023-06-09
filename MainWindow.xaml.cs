using Caster.Modules;
using static Caster.Utils.SSHUtil;
using static Caster.Utils.GlobalVariables;
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
using System.Runtime.CompilerServices;
using System.Windows.Shapes;
using Path = System.IO.Path;

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

            string path = Path.Combine(PROJ_DIR, "Data", "server.json");

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
            string path = Path.Combine(PROJ_DIR, "Data", "server.json");

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
            string path = Path.Combine(PROJ_DIR, "Data", "server.json");
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

        // AddCredentialButton_Click
        private void AddCredentialButton_Click(object sender, RoutedEventArgs e)
        {
            AddCredentialWindow addCredentialWindow = new AddCredentialWindow();
            addCredentialWindow.ShowDialog(); // ShowDialog will block the current thread until the window is closed
        }

        // DeployButotn_Click
        private void DeployButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取选中的项
            ServerInfo selectedServer = (ServerInfo)ServerList.SelectedItem;
        }

       

    }
}
