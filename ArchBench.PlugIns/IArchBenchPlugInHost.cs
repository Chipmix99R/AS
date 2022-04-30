
using System;

namespace ArchBench.PlugIns
{
    public interface IArchBenchPlugInHost
    {
        Uri              Uri    { get; }
        IArchBenchLogger Logger { get; }
    }
}
