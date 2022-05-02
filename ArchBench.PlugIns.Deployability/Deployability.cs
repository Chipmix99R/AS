using HttpServer;
using HttpServer.Sessions;
using System;
using System.IO;
using ArchBench.PlugIns;
using ArchBench.Server.Kernel;
using ArchBench;

namespace ArchBench.PlugIns.Deployability
{
    public class Deployability : IArchBenchHttpPlugIn
    {
        public string Name => "Deployability PlugIn";
        public string Description => "Verify the version and tells the administrator if there is a newer one";
        public string Author => "Rúben Silva";
        public string Version => "1.0";

        public bool Enabled { get; set; }
        public PlugInsServer Server { get; }
        public IArchBenchPlugInHost Host { get; set; }
        public IArchBenchSettings Settings { get; } = new ArchBenchSettings();

        public void Initialize() {
            var plug2 = Host;
            var plug3 = (PlugInsServer)plug2;
            var plugs = plug3.Manager.PlugIns;
            foreach(var plug in plugs)
            {
                VerifyPlugIn(plug);
            }
        }


        public void Dispose() { }

        public bool Process(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession)
        {
            var plug2 = Host;
            var plug3 = (PlugInsServer)plug2;
            var plugs = plug3.Manager.PlugIns;
            foreach (var plug in plugs)
            {
                VerifyPlugIn(plug);
            }
            return false;
        }

        private void VerifyPlugIn(IArchBenchPlugIn plugs)
        {
            if(File.Exists("Version Verifier.txt"))
            {
                var token = false;
                var newText = "";
                var i = 0;
                foreach (string text in File.ReadLines("Version Verifier.txt"))
                {
                    if (text.Contains(plugs.Name))
                    {
                        newText = text.Substring(plugs.Name.Length + 3);
                        var newnew = newText.Replace('.', ',');
                        var newS = float.Parse(newnew);
                        var actVer = plugs.Version;
                        var actact = actVer.Replace('.', ',');
                        var actV = float.Parse(actact);

                        if (actV < newS)
                        {
                            Host.Logger.WriteLine($"You are using an older version of '{plugs.Name}'");
                        }
                        else if (actV > newS)
                        {
                            token = true;
                            break;
                        }
                    }
                    i++;                
                }
                if (token)
                {
                    string newTextN = File.ReadAllText("Version Verifier.txt");
                    string textToReplace = $"{plugs.Name} ->{newText}";
                    string replacedText = $"{plugs.Name} ->{plugs.Version}";
                    newTextN = newTextN.Replace(textToReplace, replacedText);
                    File.WriteAllText("Version Verifier.txt", newTextN);
                }
                return;
            }
            else
            {
                String text = $"{plugs.Name} ->{plugs.Version}";
                File.WriteAllText("Version Verifier.txt", text);
            }
        }

    }


}