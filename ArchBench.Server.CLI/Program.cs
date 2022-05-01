using System;
using Antlr4.Runtime;
using ArchBench.Server.Kernel;

namespace ArchBench.Server.CLI
{
    class Program
    {
        private static PlugInsServer Server { get; } = new PlugInsServer();

        static void Main()
        {
            Console.WriteLine("ArchBench Server CLI (Command Line Interface) - version 1.0");
            Console.WriteLine();
            Console.WriteLine( "Use 'help' command for usage information.");
            Console.WriteLine();
            Serve();
        }

        private static void Serve()
        {
            while (true)
            {
                Console.Write( "? ");
                var command = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(command)) continue;

                var lexer  = new ArchBenchLexer(new CodePointCharStream( command ));
                var tokens = new CommonTokenStream( lexer );
                var parser = new ArchBenchParser( tokens );

                parser.RemoveErrorListeners();
                parser.AddErrorListener( new ArchBenchErrorListener() );

                var visitor = new ArchBenchExecutor( Server );
                if ( ! visitor.VisitCommand( parser.command() ) ) break;
            }

            Server.Stop();
        }
    }
}
