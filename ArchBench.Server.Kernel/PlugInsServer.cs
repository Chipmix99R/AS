using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using HttpServer;
using HttpServer.HttpModules;
using HttpServer.Sessions;
using ArchBench.PlugIns;

namespace ArchBench.Server.Kernel
{
    public class PlugInsServer : HttpModule, IArchBenchPlugInHost
    {
        #region IArchBenchPlugInHost implementation
        public Uri Uri => new Uri($"http://{Address}:{Port}");
        public IArchBenchLogger Logger  { get; }
        #endregion

        public PlugInsServer( IArchBenchLogger aLogger = null )
        {
            Logger  = aLogger ?? new ConsoleLogger();
            Manager = new PlugInsManager( this );
        }

        public string Address
        {
            get
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork) return ip.ToString();
                }
                return "0.0.0.0";

            }
        }

        public int Port { get; set; } = 8081;

        private HttpServer.HttpServer HttpServer { get; set; }

        public IPlugInsManager Manager { get; }

        public override bool Process( IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession )
        {
            foreach (var plugin in Manager.PlugIns)
            {
                if (!plugin.Enabled) continue;
                if (!(plugin is IArchBenchHttpPlugIn httpPlugIn)) continue;

                if (httpPlugIn.Process(aRequest, aResponse, aSession)) return true;
            }
            return false;
        }

        public void Start( int aPort )
        {
            HttpServer = new HttpServer.HttpServer();
            HttpServer.Add( this );
            HttpServer.Start( IPAddress.Any, aPort );
        }

        public void Stop()
        {
            HttpServer?.Stop();
            HttpServer = null;
        }

        public IEnumerable<IArchBenchPlugIn> Install( string aFileName )
        {
            return Manager.Add( aFileName );
        }

        public void Enable( IArchBenchPlugIn aPlugIn, bool aEnabled )
        {
            if ( aPlugIn != null ) aPlugIn.Enabled = aEnabled;
        }
    }
}
