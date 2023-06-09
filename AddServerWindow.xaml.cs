﻿using Caster.Modules;
using static Caster.Utils.GlobalVariables;

using System.Collections.Generic;
using System.Windows;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using System;

namespace Caster
{
    /// <summary>
    /// AddServerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddServerWindow : Window
    {
        public AddServerWindow()
        {
            InitializeComponent();
        }
        // ConfirmButton_Click
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取用户输入的数据
            string username = UsernameBox.Text;
            string host = HostBox.Text;
            int port = int.Parse(PortBox.Text); // 假设用户总是输入一个有效的整数
            string keyname = PrivateKeyFileBox.Text;


            // 创建一个新的ServerInfo对象
            ServerInfo newServerInfo = new ServerInfo(
                username,
                host,
                port,
                keyname
            );

            // 读取现有的数据
            List<ServerInfo> servers;
            string path = Path.Combine(PROJ_DIR, "Data", "server.json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                servers = JsonConvert.DeserializeObject<List<ServerInfo>>(json);
            }
            else
            {
                servers = new List<ServerInfo>();
            }

            // 添加新的ServerInfo对象到列表中
            servers.Add(newServerInfo);

            // 将列表序列化为JSON格式
            string newJson = JsonConvert.SerializeObject(servers, Newtonsoft.Json.Formatting.Indented);

            // 写入到文件中
            File.WriteAllText(path, newJson);

            // 提示保存成功
            MessageBox.Show("Success");

            // 关闭窗口
            this.Close();
        }
        // CancelButton_Click
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
