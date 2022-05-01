using HttpServer;
using HttpServer.Sessions;
using System;
using System.IO;

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

        public void Initialize() { }
        public void Dispose() { }

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

        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }


    }


}