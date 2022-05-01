using System.Collections.Generic;
using HttpServer;
using HttpServer.Sessions;

namespace ArchBench.PlugIns.Dispatcher
{
    public class PlugInDispatcher : IArchBenchHttpPlugIn
    {
        public string Name => "ArchBench Dispatcher PlugIn";
        public string Description => "Dispatcher...";
        public string Author => "Leonel Nóbrega";
        public string Version => "1.0";

        public bool Enabled { get; set; }
        public IArchBenchPlugInHost Host { get; set; }

        public IArchBenchSettings Settings { get; } = new ArchBenchSettings();

        public void Dispose()
        {
        }

        public void Initialize()
        {
        }

        public bool Process(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession)
        {
            if (aRequest.Uri.AbsolutePath.StartsWith("/register"))
            {
                Register(aRequest, aResponse, aSession);
            }
            else if (aRequest.Uri.AbsolutePath.StartsWith("/unregister"))
            {
                Unregister(aRequest, aResponse, aSession);
            }
            else
            { 
                Dispatch(aRequest, aResponse, aSession);
            }
            return false;
        }

        private IList<string> Servers { get; } = new List<string>();

        private void Register(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession)
        {
            var addr = aRequest.Headers["remote_addr"];
            var port = aRequest.QueryString["port"].Value;

            var url = $"http://{addr}:{port}";

            if ( ! Servers.Contains(url) ) Servers.Add(url);

            Host.Logger.WriteLine( $"Server at '{url}' added." );

            aResponse.Status = System.Net.HttpStatusCode.Accepted;
        }

        private void Unregister(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession)
        {
            var addr = aRequest.Headers["remote_addr"];
            var port = aRequest.QueryString["port"].Value;

            var url = $"http://{addr}:{port}";

            if ( Servers.Contains(url) ) Servers.Remove(url);

            Host.Logger.WriteLine($"Server at '{url}' removed.");

            aResponse.Status = System.Net.HttpStatusCode.Accepted;
        }

        private int Current { get; set; } = -1;

        private void Dispatch(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession)
        {
            if (Servers.Count == 0) return;

            Current = (Current + 1) % Servers.Count;

            var url = $"{Servers[Current]}{aRequest.Uri.AbsolutePath}";
            aResponse.Redirect(url);
        }
    }
}
