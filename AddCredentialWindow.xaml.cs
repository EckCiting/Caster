using Caster.Modules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Caster
{
    /// <summary>
    /// AddCredentialWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddCredentialWindow : Window
    {
        public AddCredentialWindow()
        {
            InitializeComponent();
        }
        // CacnelButton_Click
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // ConfirmButton_Click
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string domain = DomainBox.Text;
            string Zoneid = ZoneIdBox.Text;
            string email = EmailBox.Text;
            string CF_key = TokenBox.Text;

            DNSAuthInfo inputDNSAuthInfo = new DNSAuthInfo(
                domain,
                Zoneid,
                email,
                CF_key
            );

            inputDNSAuthInfo.SaveToFile();

            // 提示保存成功
            MessageBox.Show("Success");

            // 关闭窗口
            this.Close();
        }
    }
}
