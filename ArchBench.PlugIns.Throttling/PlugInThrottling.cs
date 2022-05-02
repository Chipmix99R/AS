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
        public string Name => "ArchBench Throttle Count Demo";
        public string Description => "Este plugin demonstra o funcionamento do throttling usando o SemaphoreSlim(/throttlecount) e Thread.Sleep";
        public string Author => "Duarte Santos";
        public string Version => "1.0";
        //Semaphore funciona no princípio de FIFO (Firt is First Out), sem nenhuma ordem em particular
        //a variável countNumber determina o número de theeads que podem trabalhar em simultâneo. quando maior for o número, mais rápido o trabalhpo se realiza
        //a variável taskNumber indica quantos números devem ser gerados numa seção
        //Embore que o programa rode com sucesso com o task number menor que o countNumber, o task number deve ser superior, de preferência por uma diferenca significativa, para que
        //throttling tenha efeito
        
        private static int countNumber = 3;
        private static int taskNumber = 10;
        private static int generalResponseTime = 500;


        private static SemaphoreSlim gate = new SemaphoreSlim(countNumber);
        //variável para tempo de resposta mínimo para todas as threads, para limitar o uso de recursos
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
                Task[] tasks = new Task[taskNumber];
                for (int i = 0; i < taskNumber; i++)
                {
                    tasks[i] = Task.Run(() =>
                    {
                        // Cada trabalho começa com um semáforo
                        int semaphoreCount;
                        Console.WriteLine("Task {0} begins and waits for the semaphore.", Task.CurrentId);
                        gate.Wait();
                        try
                        {
                            Interlocked.Add(ref padding, generalResponseTime);
                            Thread.Sleep(10 * i + padding); //o tempo de espera de cada thread varia com o i, para os números saiem em ordem crescente (de lembrar que o semaphore funciona no príncípio FIFO)
                            Console.WriteLine("Task {0} enters the semaphore.", Task.CurrentId);
                            aSession["Count"] = (int?)aSession["Count"] + 1 ?? 1;
                            var num = aSession["Count"];
                            var writer = new StreamWriter(aResponse.Body);
                            writer.WriteLine($"Number: " + aSession["Count"] + ".");
                            writer.Flush();

                            Host.Logger.WriteLine($"Number: " + aSession["Count"] + ".");
                            //Sleep for 1+ seconds
                        }
                        finally
                        {
                            semaphoreCount = gate.Release();
                        }
                        Host.Logger.WriteLine("Task {0} releases the semaphore; previous count: {1}.", Task.CurrentId, semaphoreCount);
                    });
                }
               
                //Espera por metade do tempo para os trablhos poderem começar e bloquear
                Thread.Sleep(500);
                // Restore the semaphore count to its maximum value.
                //Host.Logger.Write("Main thread calls Release(3) --> ");
                //gate.Release(3);
                //Host.Logger.WriteLine("{0} tasks can enter the semaphore after the operation.", gate.CurrentCount);
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
