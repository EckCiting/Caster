using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using static Caster.Utils.GlobalVariables;

namespace Caster.Modules
{
    internal class DNSAuthInfo
    {
        public string Domain { get; set; }
        public string Email { get; set; }
        public string CF_token { get; set; }
        public string ZoneId { get; set; }
        public DNSAuthInfo(string domain, string email, string cf_token, string zoneId)
        {
            Domain = domain;
            ZoneId = zoneId;
            Email = email;
            CF_token = cf_token;
        }
        
        public void SaveToFile()
        {
            DNSAuthInfo newDNSAuth = new DNSAuthInfo(this.Domain,
                Encrypt(this.ZoneId),
                Encrypt(this.Email), 
                Encrypt(this.CF_token)
                );
            // 读取现有的数据
            List<DNSAuthInfo> cretentials;
            string path = Path.Combine(PROJ_DIR, "Data", "dnsauth.json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                cretentials = JsonConvert.DeserializeObject<List<DNSAuthInfo>>(json);
            }
            else
            {
                cretentials = new List<DNSAuthInfo>();
            }

            // 添加新的ServerInfo对象到列表中
            cretentials.Add(newDNSAuth);
            // 将列表序列化为JSON格式
            string newJson = JsonConvert.SerializeObject(newDNSAuth, Newtonsoft.Json.Formatting.Indented);

            // 写入到文件中
            File.WriteAllText(path, newJson);

        }

        public static DNSAuthInfo ReadFromFile(string domain)
        {
            string path = Path.Combine(PROJ_DIR, "Data", "dnsauth.json");
            DNSAuthInfo result = null;
            if (File.Exists(path))
            {
                // 读取文件
                string json = File.ReadAllText(path);
                // 反序列化为 DNSAuthInfo 列表
                List<DNSAuthInfo> cretentials = JsonConvert.DeserializeObject<List<DNSAuthInfo>>(json);

                // 基于 this.Domain 查找对应的记录
                var foundAuthInfo = cretentials.FirstOrDefault(a => a.Domain == domain);
                if (foundAuthInfo != null)
                {
                    // 如果找到了对应的记录，对数据进行解密
                    result = new DNSAuthInfo(
                        domain,
                        Decrypt(foundAuthInfo.Email),
                        Decrypt(foundAuthInfo.ZoneId),
                        Decrypt(foundAuthInfo.CF_token)
                        );

                }
                else
                {
                    Debug.WriteLine("No record found for the domain " + domain);
                }
            }
            else
            {
                Debug.WriteLine("File does not exist: " + path);
            }
            return result;
        }


        private string Encrypt(string data, string optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var entropyBytes = string.IsNullOrEmpty(optionalEntropy) ? null : Encoding.UTF8.GetBytes(optionalEntropy);
            var encryptedBytes = ProtectedData.Protect(dataBytes, entropyBytes, scope);
            return Convert.ToBase64String(encryptedBytes);
        }

        private static string Decrypt(string data, string optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            var dataBytes = Convert.FromBase64String(data);
            var entropyBytes = string.IsNullOrEmpty(optionalEntropy) ? null : Encoding.UTF8.GetBytes(optionalEntropy);
            var decryptedBytes = ProtectedData.Unprotect(dataBytes, entropyBytes, scope);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
