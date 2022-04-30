using System.Collections;
using System.Collections.Generic;

namespace ArchBench.PlugIns
{
    public class ArchBenchSettings : IArchBenchSettings
    {
        private IDictionary<string, string> Settings { get; } = new Dictionary<string, string>();

        public string this[ string key ]
        {
            get => Settings.ContainsKey( key ) ? Settings[ key ] : string.Empty;
            set { if (Settings.ContainsKey(key)) Settings[ key ] = value; else Settings.Add( key, value ); }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Settings.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
