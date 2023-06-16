using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caster.Utils
{
    internal class GlobalVariables
    {
        public static string PROJ_DIR = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        public static string SSH_DIR = Path.Combine(@"C:\Users", Environment.UserName, ".ssh");
    }
}
