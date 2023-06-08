using Caster.Modules;
using static Caster.Utils.GlobalVariables;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caster.Modules;

namespace Caster.Utils
{
    internal class SSHUtil
    {
        private static ConnectionInfo GetConnectionInfo(ServerInfo serverInfo)
        {
            string keyPath = Path.Combine(SSH_DIR, serverInfo.PrivateKeyFileName);
            // Create a new instance of the PrivateKeyFile class with the path to the private key file
            var keyFile = new PrivateKeyFile(keyPath);
            var keyFiles = new[] { keyFile };

            // Create a new instance of the ConnectionInfo class with the connection details
            var connectionInfo = new ConnectionInfo(
                serverInfo.Host,
                serverInfo.Username,
                new PrivateKeyAuthenticationMethod(serverInfo.Username, keyFiles));

            return connectionInfo;
    }
        public static void Test_SSH_Connection(ServerInfo serverInfo)
        {

            ConnectionInfo connectionInfo = GetConnectionInfo(serverInfo);
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
        public static void SCP_UPLOAD(ServerInfo serverInfo, string filepath,string targetFilename) 
        {
            ConnectionInfo connectionInfo = GetConnectionInfo(serverInfo);
            using (var client = new ScpClient(connectionInfo))
            {
                try
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        client.Upload(new FileInfo(filepath), "/home/" + serverInfo.Username + "/" + targetFilename);
                    }
                    else
                    {
                        MessageBox.Show("Connection failed!");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Connection failed: {e.Message}");
                }
                finally
                {
                    if (client.IsConnected)
                    {
                        client.Disconnect();
                    }
                }
            }
         }
    }
}
