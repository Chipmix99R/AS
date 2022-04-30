using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchBench.Server.Configurations
{
    public class PlugInConfig
    {
        public string FullName { get; set; }
        public string FileName { get; set; }

        public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    }
}
