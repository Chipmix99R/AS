using System;

namespace ArchBench.PlugIns.Proxy
{
    public class PluginProxy : IArchBenchHttpPlugIn
    {
        public string Name        => "PlugIn Proxy ArchBench";
        public string Description => "Implement a HTTP proxy";
        public string Author      => "Leonel Nóbrega";
        public string Version     => "1.0";

        public bool Enabled { get; set; } = false;

        public IArchBenchPlugInHost Host     { get; set; }
        public IArchBenchSettings   Settings { get; } = new ArchBenchSettings();

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
            try { 
                var client = new WebClient();
                ForwardCookies(client, aRequest);
                byte[] bytes = null;

                if(aRequest.Method == Method.Post)
                {
                    var inputs = ForwardInputs(aRequest);
                    bytes = client.UploadValues(url, inputs);
                }
                else
                    var byte = client.DownloadData(url);

                BackwardCookies(client, aResponse);
                aResponse.Body.Write(byte, 0, byte.length);
            }
            catch(Exception ex)
            {
                Host.Logger.WriteLine($"Error: {ex.Message} on getting '{url}'");
                return false;
            }
            return true;
        }

        private void ForwardCookies (WebClient client, IHttpRequest request)
        {
            if(request.Headers["Cookie"] != null)
            {
                client.Headers.Add("Cookie", request.Headers["Cookie"]);
            }
        }

        private void BackwardCookies (WebClient client, IHttpResponse response)
        {
            if(client.ResponseHeaders["Set-Cookie"] != null)
            {
                response.AddHeader("Set-Cookie", client.ResponseHeaders["Set-Cookie"]);
            }
        }

        private NameValueCollection ForwardInputs(IHttpRequest request)
        {
            var inputs = new NameValueCollection();
            foreach(HttpInputItem item in request.Form)
            {
                inputs.Add(item.Name, item.Value);
            }
            return inputs;
        }
    }
}
