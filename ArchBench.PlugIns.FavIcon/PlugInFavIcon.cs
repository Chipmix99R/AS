using HttpServer;
using HttpServer.Sessions;
using HttpServer.Helpers;
using System;
using System.Reflection;
using System.Net;

namespace ArchBench.PlugIns.FavIcon
{
    public class PlugInFavIcon : IArchBenchHttpPlugIn
    {
        private ResourceManager Manager { get; } = new ResourceManager();

        public string Name        => "PlugIn FavIcon";
        public string Description => "Returns tha favicon.ico";
        public string Author      => "Leonel Nóbrega";
        public string Version     => "1.0";

        public bool Enabled { get; set; } = false;

        public IArchBenchPlugInHost Host { get; set; }
        public IArchBenchSettings Settings { get; } = new ArchBenchSettings();

        public PlugInFavIcon()
        {
            Manager.LoadResources("/", Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly().GetName().Name);
        }

        public void Dispose()
        {
        }

        public void Initialize()
        {
        }

        public bool Process( IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession )
        {
            if ( aRequest.Uri.AbsolutePath.StartsWith( "/favicon.ico", StringComparison.InvariantCultureIgnoreCase))
            {
                var stream = Manager.GetResourceStream( aRequest.UriPath );
                if (stream == null) return false;

                aResponse.ContentType = "image/x-icon";

                // Force the load of the resource
                aResponse.Status = HttpStatusCode.OK;
                aResponse.AddHeader("Last-modified", DateTime.Now.ToString("r"));

                aResponse.ContentLength = stream.Length;
                aResponse.SendHeaders();

                if (aRequest.Method != "Headers" && aResponse.Status != HttpStatusCode.NotModified)
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead = stream.Read(buffer, 0, 8192);
                    while (bytesRead > 0)
                    {
                        aResponse.SendBody(buffer, 0, bytesRead);
                        bytesRead = stream.Read(buffer, 0, 8192);
                    }
                }

                return true;
            }
            return false;
        }
    }
}
