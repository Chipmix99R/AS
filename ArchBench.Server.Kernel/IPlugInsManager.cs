using System.Collections.Generic;
using ArchBench.PlugIns;

namespace ArchBench.Server.Kernel
{
    public interface IPlugInsManager
    {
        /// <summary>
        /// A Collection of all plug-ins
        /// </summary>
        IEnumerable<IArchBenchPlugIn> PlugIns { get; }

        /// <summary>
        /// <summary>Loads all plug-ins contained in the specified file.</summary>
        /// </summary>
        /// <param name="aFileName">The assembly's path.</param>
        /// <returns>An enumeration of all new added plug-ins</returns>
        IEnumerable<IArchBenchPlugIn> Add( string aFileName );

        /// <summary>
        /// <summary>Loads all plug-ins contained in the specified file.</summary>
        /// </summary>
        /// <param name="aFileName">The assembly's path.</param>
        /// <param name="aFullName">The plugin's full name.</param>
        /// <returns>The new added plug-ins</returns>
        IArchBenchPlugIn Add( string aFileName, string aFullName );

        /// <summary>
        /// Removes the specified plug-in from the manager's collection. 
        /// </summary>
        /// <param name="aPlugIn">a reference to the plug-in</param>
        void Remove( IArchBenchPlugIn aPlugIn );

        /// <summary>
        /// Search for a plug-in
        /// </summary>
        /// <param name="aName">The name of the plug-in</param>
        /// <returns></returns>
        IArchBenchPlugIn Find( string aName );

        ///// <summary>
        ///// Returns the plugin at a given index
        ///// </summary>
        ///// <param name="aIndex">The index of the plug-in</param>
        ///// <returns></returns>
        //IArchBenchPlugIn Get( int aIndex );

        /// <summary>
        /// Returns the index of a given plugin
        /// </summary>
        /// <param name="aPlugIn">The plug-in</param>
        /// <returns></returns>
        int IndexOf( IArchBenchPlugIn aPlugIn );

        /// <summary>
        /// Returns the plugin at a given index
        /// </summary>
        /// <param name="aIndex">The index of the plug-in</param>
        /// <returns></returns>
        IArchBenchPlugIn Get(int aIndex);

        /// <summary>
        /// Unloads and Closes all plug-ins
        /// </summary>
        void Clear();

        string GetFileName( IArchBenchPlugIn aPlugIn );
    }
}