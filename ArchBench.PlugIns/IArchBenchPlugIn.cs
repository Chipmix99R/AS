using System.Collections.Generic;

namespace ArchBench.PlugIns
{
    public interface IArchBenchPlugIn
    {
        string Name        { get; }
        string Description { get; }
        string Author      { get; }
        string Version     { get; }
        
        bool Enabled { get; set; }

        IArchBenchPlugInHost Host { get; set; }

        IArchBenchSettings Settings { get; }

        void Initialize();
        void Dispose();
    }
}
