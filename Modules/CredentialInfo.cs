using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Printing;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using static Caster.Utils.GlobalVariables;

namespace Caster.Modules
{
    internal class CredentialInfo
    {
        public string Domain { get; set; }
        public string Email { get; set; }
        public string CF_token { get; set; }
        public string ZoneId { get; set; }
        public CredentialInfo(string domain, string email, string cf_token, string zoneId)
        {
            Domain = domain;
            Email = email;
            CF_token = cf_token;
            ZoneId = zoneId;
        }

        // ToString
        public override string ToString()
        {
            return $"{Domain}:{Email}";
        }
        // Equals
        public override bool Equals(object obj)
        {
            if (obj is CredentialInfo other)
            {
                return Domain == other.Domain && Email == other.Email;
            }
            return false;
        }
        // HashCode
        public override int GetHashCode()
        {
            return HashCode.Combine(Domain, Email);
        }

        public CredentialInfo DecryptThisRecord()
        {
            return new CredentialInfo(
                this.Domain,
                this.Email,
                Decrypt(this.CF_token),
                Decrypt(this.ZoneId)
            );
        }
        public void SaveToFile()
        {
            CredentialInfo newDNSAuth = new CredentialInfo(this.Domain,
                this.Email,
                Encrypt(this.CF_token),
                Encrypt(this.ZoneId)
                );
            // 读取现有的数据
            List<CredentialInfo> cretentials;
            string path = Path.Combine(PROJ_DIR, "Data", "credential.json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                cretentials = JsonConvert.DeserializeObject<List<CredentialInfo>>(json);
            }
            else
            {
                cretentials = new List<CredentialInfo>();
            }

            bool isDuplicateDomain = cretentials.Any(c => c.Domain == newDNSAuth.Domain);

            if (!isDuplicateDomain)
            {
                // 添加新的ServerInfo对象到列表中
                cretentials.Add(newDNSAuth);
                // 将列表序列化为JSON格式
                string newJson = JsonConvert.SerializeObject(cretentials, Newtonsoft.Json.Formatting.Indented);

                // 写入到文件中
                File.WriteAllText(path, newJson);
            }
            else
            {
                MessageBox.Show("Duplicate domain name found. Please check your input.");
            }
        }



        public static CredentialInfo FindCredentialByDomain(string domain)
        {
            string path = Path.Combine(PROJ_DIR, "Data", "credential.json");
            CredentialInfo result = null;
            if (File.Exists(path))
            {
                // 读取文件
                string json = File.ReadAllText(path);
                // 反序列化为 DNSAuthInfo 列表
                List<CredentialInfo> cretentials = JsonConvert.DeserializeObject<List<CredentialInfo>>(json);

                // 基于 this.Domain 查找对应的记录
                var foundAuthInfo = cretentials.FirstOrDefault(a => a.Domain == domain);
                if (foundAuthInfo != null)
                {
                    // 如果找到了对应的记录，对数据进行解密
                    result = new CredentialInfo(
                        domain,
                        foundAuthInfo.Email,
                        Decrypt(foundAuthInfo.CF_token),
                        Decrypt(foundAuthInfo.ZoneId)
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
