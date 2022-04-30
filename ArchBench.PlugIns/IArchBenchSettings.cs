using System.Collections.Generic;

namespace ArchBench.PlugIns
{
    public interface IArchBenchSettings : IEnumerable<string>
    {
        string this[ string key] { get; set; }
    }
}
