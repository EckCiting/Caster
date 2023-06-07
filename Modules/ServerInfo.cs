using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Caster.Modules
{
    public class ServerInfo
    {
        public string Username { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string PrivateKeyFileName { get; set; } = "id_rsa";

        public override string ToString()
        {
            return $"{Username}@{Host}:{Port}";
        }

        public override bool Equals(object obj)
        {
            if (obj is ServerInfo other)
            {
                return Username == other.Username && Host == other.Host && Port == other.Port && PrivateKeyFileName == other.PrivateKeyFileName;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Username, Host, Port, PrivateKeyFileName);
        }
    }
}
