using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ArchBench.PlugIns;

namespace ArchBench.Server.Kernel
{
    /// <summary>
    /// Summary description for PlugInsManager.
    /// </summary>
    public class PlugInsManager : IPlugInsManager
    {
        #region Fields
        private readonly IDictionary<IArchBenchPlugIn, string> mPlugIns = new Dictionary<IArchBenchPlugIn, string>();
        #endregion

        public PlugInsManager(IArchBenchPlugInHost aHost)
        {
            Host = aHost;
        }

        public IArchBenchPlugInHost Host { get; }

        public IEnumerable<IArchBenchPlugIn> PlugIns => mPlugIns.Keys;

        private IEnumerable<IArchBenchPlugIn> Load(string aFileName, string aFullName)
        {
            if ( ! File.Exists( aFileName ) )
                throw new FileNotFoundException( "Not Found", aFileName );

            // Create a new assembly from the plugin file we're adding..
            Assembly assembly = Assembly.LoadFrom(aFileName);

            var instances = new List<IArchBenchPlugIn>();

            // Next we'll loop through all the Types found in the assembly
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsPublic) continue;
                if (type.IsAbstract) continue;

                // Gets a type object of the interface we need the plugins to match
                Type typeInterface = type.GetInterface($"ArchBench.PlugIns.{ nameof(IArchBenchPlugIn) }", true);

                // Make sure the interface we want to use actually exists
                if (typeInterface == null) continue;
                if (aFullName?.Equals(type.FullName) ?? true)
                {
                    // Create a new instance and store the instance in the collection for later use
                    var instance = (IArchBenchPlugIn)Activator.CreateInstance(assembly.GetType(type.ToString()));

                    // Set the Plug-in's host to this class which inherited IPluginHost
                    instance.Host = Host;

                    // Call the initialization sub of the plugin
                    instance.Initialize();

                    //Add the new plugin to our collection here
                    mPlugIns.Add(instance, aFileName);

                    instances.Add(instance);
                }
            }

            return instances;
        }

        public IEnumerable<IArchBenchPlugIn> Add( string aFileName )
        {
            return Load(aFileName, null);
        }

        public IArchBenchPlugIn Add( string aFileName, string aFullName )
        {
            return Load(aFileName, aFullName)?.FirstOrDefault();
        }

        public void Remove(IArchBenchPlugIn aPlugIn)
        {
            if (mPlugIns.ContainsKey(aPlugIn)) mPlugIns.Remove(aPlugIn);
        }

        public IArchBenchPlugIn Find(string aName)
        {
            return mPlugIns.Keys.FirstOrDefault(p => p.Name == aName);
        }

        public IArchBenchPlugIn Get(int aIndex)
        {
            if ( aIndex < 0 ) return null;
            if ( aIndex > mPlugIns.Count - 1 ) return null;

            return mPlugIns.Keys.ToList()[ aIndex ];
        }

        public int IndexOf( IArchBenchPlugIn aPlugIn )
        {
            return mPlugIns.Keys.ToList().IndexOf( aPlugIn );
        }

        public void Clear()
        {
            foreach (var plugin in mPlugIns.Keys)
            {
                // Close all plugin instances
                plugin.Dispose();
            }

            //Finally, clear our collection of available plugins
            mPlugIns.Clear();
        }

        public string GetFileName(IArchBenchPlugIn aPlugIn)
        {
            if ( mPlugIns.ContainsKey( aPlugIn ) ) return mPlugIns[ aPlugIn ];
            return string.Empty;
        }

    }
}