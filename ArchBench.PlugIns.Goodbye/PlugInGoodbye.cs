using HttpServer;
using HttpServer.Sessions;
using System;

namespace ArchBench.PlugIns.Goodbye
{
    public class Goodbye : IArchBenchHttpPlugIn
    {
        public string Name        => "Goodbye PlugIn";
        public string Description => "Bye...";
        public string Author      => "Leonel Nóbrega";
        public string Version     => "1.0";

        public bool Enabled {get;set;}
        
        public IArchBenchPlugInHost Host     { get; set; }
        public IArchBenchSettings   Settings { get; } = new ArchBenchSettings();

        public bool Process(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession)
        {
             if ( aRequest.Uri.AbsolutePath.StartsWith( "/goodbye", StringComparison.InvariantCultureIgnoreCase ) )
            {
                aSession[ "Count" ] = (int?) aSession[ "Count" ] + 1 ?? 1;
                var ordinal = AddOrdinal( (int) aSession[ "Count" ] );

                var writer = new StreamWriter(aResponse.Body);
                writer.WriteLine( $"Hi! for the { ordinal } time.");
                writer.Flush();

                Host.Logger.WriteLine( $"Saying Hello for the { ordinal } times.");
                
                return true;
            }
            return false;
        }
    }


}