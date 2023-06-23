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
using Certes.Acme.Resource;
using Certes.Acme;
using Caster.Utils;
using Certes;

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

            string serverPath = Path.Combine(PROJ_DIR, "Data", "server.json");

            if (File.Exists(serverPath))
            {
                string json = File.ReadAllText(serverPath);
                var servers = JsonConvert.DeserializeObject<List<ServerInfo>>(json);
                ServerList.ItemsSource = servers;
            }

            string credentialPath = Path.Combine(PROJ_DIR, "Data", "credential.json");
            if (File.Exists(credentialPath)) {
                string json = File.ReadAllText(credentialPath);
                var credentials = JsonConvert.DeserializeObject<List<CredentialInfo>>(json);
                CredentialList.ItemsSource = credentials;
            }
        }

        private void ServerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CredentialList_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

            // Reload data from credential.json and update ListBox
            string path = Path.Combine(PROJ_DIR, "Data", "credential.json");

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var credentials = JsonConvert.DeserializeObject<List<CredentialInfo>>(json);
                CredentialList.ItemsSource = credentials;
            }

        }
        // RemoveCredentialButton_Click
        private void RemoveCredentialButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取选中的项
            CredentialInfo selectedCredential = (CredentialInfo)CredentialList.SelectedItem;

            if (selectedCredential == null)
            {
                // 如果没有选中任何项，则直接返回
                return;
            }

            // 读取现有的数据
            string path = Path.Combine(PROJ_DIR, "Data", "credential.json");
            List<CredentialInfo> credentials = new List<CredentialInfo>();
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var deserialized = JsonConvert.DeserializeObject<List<CredentialInfo>>(json);
                credentials = deserialized ?? credentials;
            }
            // 移除选中的项
            credentials.Remove(selectedCredential);

            // 将列表序列化为JSON格式
            string newJson = JsonConvert.SerializeObject(credentials, Newtonsoft.Json.Formatting.Indented);

            // 写入到文件中
            File.WriteAllText(path, newJson);

            // 更新ListBox的数据源
            CredentialList.ItemsSource = null;
            CredentialList.ItemsSource = credentials;
        } 

        // DeployButotn_Click
        private async void DeployButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取选中的项
            ServerInfo selectedServer = (ServerInfo)ServerList.SelectedItem;
            CredentialInfo selectedCredential = (CredentialInfo)CredentialList.SelectedItem;

            // Generate server
            if (selectedServer == null)
            {
                MessageBox.Show("Please select a server");
                return;
            }
            else if (selectedCredential == null)
            {
                MessageBox.Show("Please select a credential");
                return;
            }
            else
            {
                await LetsEncV2.RegisterCertificate(selectedCredential);
                // the generated fullchain.pem and privkey.pem will be in Data folder
                SSHUtil.SCP_UPLOAD(selectedServer, Path.Combine(PROJ_DIR, "Data", "ssl" , "fullchain.pem"), "ssl/fullchain.pem");
                SSHUtil.SCP_UPLOAD(selectedServer, Path.Combine(PROJ_DIR, "Data", "ssl", "privkey.pem"), "ssl/privkey.pem");

            }
        }

       

    }
}
