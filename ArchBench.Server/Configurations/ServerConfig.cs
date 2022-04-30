using System.Collections.Generic;

namespace ArchBench.Server.Configurations
{
    public class ServerConfig
    {
        public int Port { get; set; }
        public ICollection<PlugInConfig> PlugIns { get; set; } = new List<PlugInConfig>();
    }
}
