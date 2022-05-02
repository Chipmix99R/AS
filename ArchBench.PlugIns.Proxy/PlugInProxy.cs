using HttpServer;
using HttpServer.Sessions;
using System;
using System.Collections.Specialized;
using System.Net;

namespace ArchBench.PlugIns.Proxy
{
    public class PlugInProxy : IArchBenchHttpPlugIn
    {
        public string Name => @"ArchBench Proxy PlugIn";

        public string Description => @"Implements a HTTP Proxy";

        public string Author => @"Leonel Nobrega";

        public string Version => @"1.0";

        public bool Enabled { get; set; }
        public IArchBenchPlugInHost Host { get; set; }

        public IArchBenchSettings Settings { get; } = new ArchBenchSettings();

        public void Dispose()
        {
        }

        public void Initialize()
        {
            Settings["Remote"] = "http://localhost:8082";
        }

        public bool Process(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession)
        {
            var url = $"{Settings["Remote"]}{aRequest.Uri.AbsolutePath}";

            try
            {
                var client = new WebClient();
                
                ForwardCookies( client, aRequest );

                byte[] bytes = null;

                if (aRequest.Method == Method.Post)
                {
                    var inputs = ForwardInputs(aRequest);
                    bytes = client.UploadValues(url, inputs);
                }
                else
                {
                    bytes = client.DownloadData(url);
                }

                BackwardCookies(client, aResponse);

                aResponse.Body.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Host.Logger.WriteLine($"Error: {e.Message} on getting '{url}'");
                return false;
            }
            return true;
        }
        private void ForwardCookies(WebClient client, IHttpRequest request)
        {
            if (request.Headers["Cookie"] == null) return;
            
            client.Headers.Add("Cookie", request.Headers["Cookie"]);
        }
        private void BackwardCookies( WebClient client, IHttpResponse response )
        {
            if (client.ResponseHeaders["Set-Cookie"] == null) return;

            response.AddHeader("Set-Cookie", client.ResponseHeaders["Set-Cookie"]);
        }

        private NameValueCollection ForwardInputs(IHttpRequest request)
        {
            var inputs = new NameValueCollection();
            foreach (HttpInputItem item in request.Form)
            {
                inputs.Add(item.Name, item.Value);
            }
            return inputs;
        }
    }
}
