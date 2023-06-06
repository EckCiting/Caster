using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caster.Modules
{
    public class ServerInfo
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Target { get; set; }
        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
        public override bool Equals(object obj)
        {
            if (obj is ServerInfo other)
            {
                return other.Host == Host && other.Port == Port && other.Target == Target;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Host, Port, Target);
        }
    }

}
