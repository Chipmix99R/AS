using HttpServer;
using HttpServer.Sessions;

namespace ArchBench.PlugIns
{
    public interface IArchBenchHttpPlugIn : IArchBenchPlugIn
    {
        bool Process( IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession );
    }
}
