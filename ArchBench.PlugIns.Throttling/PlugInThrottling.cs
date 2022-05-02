using HttpServer;
using HttpServer.Sessions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ArchBench.PlugIns.Throttling
{
    public class PlugInThrottling : IArchBenchHttpPlugIn
    {
        public string Name => "ArchBench Hello Throttling";
        public string Description => "Este plugin faz o Throttling do recurso hello usando o SemaphoreSlim";
        public string Author => "Duarte Santos";
        public string Version => "1.0";


        SemaphoreSlim gate = new SemaphoreSlim(1);


        public bool Enabled { get; set; } = false;

        

        public IArchBenchPlugInHost Host { get; set; }
        public IArchBenchSettings Settings { get; } = new ArchBenchSettings();

        public void Dispose()
        {
        }

        public void Initialize()
        {
            
        }


        public  bool Process(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession)
        {
            if (aRequest.Uri.AbsolutePath.StartsWith("/hello", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    await gate.WaitAsync();
                    aSession["Count"] = (int?)aSession["Count"] + 1 ?? 1;
                    var ordinal = AddOrdinal((int)aSession["Count"]);

                    var writer = new StreamWriter(aResponse.Body);
                    writer.WriteLine($"Hi! for the { ordinal } time.");
                    writer.Flush();

                    Host.Logger.WriteLine($"Saying Hello for the { ordinal } times.");
                    gate.Release();
                }
                catch(Exception ex)
                {
                    Host.Logger.WriteLine(ex.ToString());
                    return false;
                }
                return true;
            }
            return false;
        }
        //Hello com o throttle SemaphoreSlim
        
            
        

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
