using HttpServer;
using HttpServer.Sessions;

namespace ArchBench.PlugIns.Logger
{
    public class PlugInLogger : IArchBenchHttpPlugIn
    {
        public string Name        => "Logger";
        public string Description => "Log all requests";
        public string Author      => "Leonel Nóbrega";
        public string Version     => "1.0";

        public bool Enabled { get; set; }
        public IArchBenchPlugInHost Host { get; set; }

        public IArchBenchSettings Settings { get; } = new ArchBenchSettings();

        public void Dispose()
        {
        }

        public void Initialize()
        {
            Settings[ "Headers" ] = false.ToString();
        }

        public bool Process(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession)
        {
            Host.Logger.WriteLine("{0,8} {1} : {2}", aRequest.Method, aRequest.HttpVersion, aRequest.Uri.AbsoluteUri );

            if ( ! bool.TryParse( Settings[ "Headers" ], out var headers ) ) return false;
            if ( ! headers ) return false;

            foreach (var key in aRequest.Headers.AllKeys)
            {
                Host.Logger.WriteLine("{0}: {1}", key, aRequest.Headers[key]);
            }

            return false;
        }
    }
}
