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
        public string Name => "ArchBench Throttling Demo";
        public string Description => "Este plugin demonstra o funcionamento do throttling usando o SemaphoreSlim(/throttlecount)";
        public string Author => "Duarte Santos";
        public string Version => "1.0";
        //Semaphore funciona no princípio de FIFO (Firt is First Out), sem nenhuma ordem em particular
        //o initial count determina o número de theeads que podem trabalhar em simultâneo. quando maior for o número, mais rápido o trabalhpo se realiza
        private static SemaphoreSlim gate = new SemaphoreSlim(initialCount: 2);
        //amacia
        private static int padding;


        public bool Enabled { get; set; } = false;

        

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
            if (aRequest.Uri.AbsolutePath.StartsWith("/throttlecount", StringComparison.InvariantCultureIgnoreCase))
            {
                //Host.Logger.WriteLine("{0} tasks can enter the semaphore before the operation.", gate.CurrentCount);
                // Criar e fazer 10 trabalhos
                Task[] tasks = new Task[10];
                aSession["Count"] = (int?)aSession["Count"];//inicializar o aSession[Count]
                for (int i = 0; i <= 9; i++)
                {
                    tasks[i] = Task.Run(async () =>
                    {
                        // Cada trabalho começa com um semáforo
                        Console.WriteLine("Task {0} begins and waits for the semaphore.", Task.CurrentId);
                        int semaphoreCount;
                        await gate.WaitAsync();
                        try
                        {
                            Interlocked.Add(ref padding, 100);
                            Console.WriteLine("Task {0} enters the semaphore.", Task.CurrentId);
                            aSession["Count"] = (int?)aSession["Count"] + 1 ?? 1;
                           

                            var writer = new StreamWriter(aResponse.Body);
                            writer.WriteLine($"Number: "+ aSession["Count"]+".");
                            writer.Flush();

                            Host.Logger.WriteLine($"Number: " + aSession["Count"] + ".");
                            //Sleep for 1+ seconds
                            Thread.Sleep(1000 + padding);
                        }
                        finally
                        {
                            semaphoreCount = gate.Release();
                        }
                        //Host.Logger.WriteLine("Task {0} releases the semaphore; previous count: {1}.", Task.CurrentId, semaphoreCount);
                    });
                }
               
                //Espera por metade do tempo para os trablhos poderem começar e bloquear
                Thread.Sleep(500);
                // Restore the semaphore count to its maximum value.
                //Host.Logger.Write("Main thread calls Release(3) --> ");
                //gate.Release(3);
                Host.Logger.WriteLine("{0} tasks can enter the semaphore after the operation.", gate.CurrentCount);
                // Thread principal espera para os trabalhos terminarem.
                Task.WaitAll(tasks);
                Host.Logger.WriteLine("Main thread exits.");

            }
            else
            {
                return false;
            }
            return true;
        }
        


    }

    



}
