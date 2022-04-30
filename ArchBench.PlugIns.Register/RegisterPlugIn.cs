using System;
using System.Net.Http;

namespace ArchBench.PlugIns.Register
{
    public class RegisterPlugIn : IArchBenchHttpPlugIn
    {
        public string Name        => "Server Register Plugin";
        public string Description => "Register the current Server on the given Registry";
        public string Author      => "Leonel Nóbrega";
        public string Version     => "1.0";

        public bool Enabled
        {
            get => OnService; 
            set => Register( value );
        }
        public IArchBenchPlugInHost Host { get; set; }

        public IArchBenchSettings Settings { get; } = new ArchBenchSettings();

        public bool OnService { get; set; } = false;

        public void Dispose()
        {
        }

        public void Initialize()
        {
            Settings[ "Registry" ] = "http://localhost:8081/";
        }

        public bool Process( HttpServer.IHttpRequest aRequest, HttpServer.IHttpResponse aResponse, HttpServer.Sessions.IHttpSession aSession )
        {
            return false;
        }

        private async void Register( bool aEnabled )
        {
            if ( OnService == aEnabled ) return;

            var url = Settings[ "Registry" ];
            if ( ! url.EndsWith( "/" ) ) url += "/";
            url += aEnabled ? "register" : "unregister";
            url += $"?port={ Host.Uri.Port }";
            
            var http = new HttpClient();
            var response = await http.GetAsync( new Uri( url ) );
            if ( response.IsSuccessStatusCode ) OnService = aEnabled;
        } 
    }
}
