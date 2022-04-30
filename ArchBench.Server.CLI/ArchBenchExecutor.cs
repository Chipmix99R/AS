using System;
using System.Linq;
using Antlr4.Runtime.Tree;
using ArchBench.PlugIns;
using ArchBench.Server.Kernel;

namespace ArchBench.Server.CLI
{
    class ArchBenchExecutor : ArchBenchBaseVisitor<bool>
    {
        private PlugInsServer Server { get; }

        public ArchBenchExecutor( PlugInsServer aServer )
        {
            Server = aServer;
        }

        public override bool VisitErrorNode( IErrorNode node )
        {
            return true;
        }



        public override bool VisitCommand(ArchBenchParser.CommandContext aContext)
        {
            if (aContext.ChildCount < 1) return false;

            switch (aContext.GetChild(0).GetText())
            {
                case "help":
                    Help();
                    break;
                case "start":
                    Start( aContext.GetChild( 1 ) );
                    break;
                case "stop":
                    Stop();
                    break;
                case "install":
                    Install( aContext.GetChild( 1 ) );
                    break;
                case "with":
                    Set( aContext );
                    break;
                case "show":
                    Show( aContext );
                    break;
                case "enable":
                    Enable( aContext, true);
                    break;
                case "disable":
                    Enable( aContext, false);
                    break;
                case "exit":
                    return Exit();
                default:
                    //Console.WriteLine($"Unknown Command: '{ command }'");
                    return true;
            }

            return true;
        }

        public static void Help()
        {
            Console.WriteLine("The ArchBench Server CLI is a simple command line utility that allows ");
            Console.WriteLine("running and instance of ArchBench Server.");
            Console.WriteLine();
            Console.WriteLine("Available Commands:");
            Console.WriteLine();
            Console.WriteLine("  start <port>                                 Start the server on the specified port (<port>).");
            Console.WriteLine("  stop                                         Stops the server.");
            Console.WriteLine("  install <path>                               Install a plugin.");
            Console.WriteLine("  show                                         Show all plugins installed");
            Console.WriteLine("  show <id>                                    Show details of plugin with the given id");
            Console.WriteLine("  enable  ( <id> | <name> )                    Enables the plugin.");
            Console.WriteLine("  disable ( <id> | <name> )                    Disables the plugin.");
            Console.WriteLine("  with ( <id> |<name> ) set <property>=<value> Define the plugin's settings");
            Console.WriteLine("  help                                         Help about this application.");
            Console.WriteLine("  exit                                         Exits from server.");
            Console.WriteLine();
        }

        private bool Exit()
        {
            return false;
        }

        private void Start( IParseTree aNode )
        {
            if ( aNode is IErrorNode )
            {
                Console.WriteLine("The <port> is required. Example: ");
                Console.WriteLine("start 8081");
            }
            else if ( int.TryParse( aNode.GetText(), out int port))
            {
                Server.Start(port);
                Console.WriteLine($"Server started on port '{ port }'");
            }
        }

        private void Stop()
        {
            Server.Stop();
        }

        private void Install( IParseTree aNode )
        {
            if ( aNode == null )
            {
            }
            else if ( aNode is IErrorNode )
            {
                Console.WriteLine( aNode.GetText() );
            }
            else
            {
                var plugins = Server.Install( aNode.GetText() );
                Console.WriteLine();
                Console.WriteLine("Installed plugins:");
                Console.WriteLine();
                foreach (var plugin in plugins)
                {
                    Show( plugin );
                }
                Console.WriteLine();
            }
        }

        private void Show( ArchBenchParser.CommandContext aContext )
        {
            //VisitChildren( aContext );

            var context = aContext.GetChild( 1 ) as ArchBenchParser.IdentifierOptContext;
            if ( context == null ) return;

            if ( context.ChildCount == 0 )
            {
                foreach (var plugin in Server.Manager.PlugIns)
                {
                    var index = Server.Manager.IndexOf(plugin);
                    Console.WriteLine($"[{ index }] :\t{ plugin.Name } ({ GetStatus(plugin)})");
                }
            }
            else
            {
                if ( context.GetChild(0)?.GetChild(0) is ITerminalNode child )
                {
                    var symbol = child?.Symbol.Text;
                    if ( int.TryParse( symbol, out var id ) )
                    {
                        Show(Server.Manager.Get(id - 1 ) );
                    }
                    else
                    {
                        symbol = symbol.Trim( '\'' );
                        Show( Server.Manager.Find( symbol )  );
                    }
                }
            }
        }

        private void Show( IArchBenchPlugIn aPlugIn )
        {
            if (aPlugIn == null) return;

            var index = Server.Manager.IndexOf(aPlugIn);
            Console.Write($"[{index + 1}]");
            Console.WriteLine($" :\t{ aPlugIn.Name } ({ GetStatus(aPlugIn) })");
            Console.WriteLine($"\tAuthor: { aPlugIn.Author }");
            Console.WriteLine($"\tVersion: { aPlugIn.Version }");
            Console.WriteLine($"\tDescription: { aPlugIn.Description }");
            if (aPlugIn.Settings.Any())
            {
                Console.WriteLine("\tSettings:");
                foreach (var key in aPlugIn.Settings)
                {
                    Console.WriteLine($"\t\t{key} = { aPlugIn.Settings[key] }");
                }
            }
        }

        private static string GetStatus(IArchBenchPlugIn aPlugIn)
        {
            return aPlugIn.Enabled ? "enabled" : "disabled";
        }

        private void Enable( ArchBenchParser.CommandContext aContext, bool aEnabled )
        {
            if ( aContext.ChildCount < 2 )
            {
                Console.WriteLine( "usage: enable ( <id> | <name> )");
            }
            else
            {
                var child = aContext.GetChild( 1 ) as ITerminalNode;
                if ( int.TryParse( child?.Symbol.Text, out var id ) )
                {
                    Server.Enable(Server.Manager.Get(id - 1), aEnabled);
                }
                else
                {
                    var name = child?.Symbol.Text.Trim( '\'' );
                    Server.Enable(Server.Manager.Find( name ), aEnabled );
                }

            }
        }

        private void Set( ArchBenchParser.CommandContext aContext )
        {
            if ( aContext.ChildCount < 6 )
            {
                Console.WriteLine( "with ( <id> |<name> ) set <property>=<value>" );
            }

            var ident    = aContext.GetChild( 1 ).GetChild( 0 ) as ITerminalNode;
            var property = aContext.GetChild( 3 ) as ITerminalNode;
            var value    = aContext.GetChild( 5 ).GetChild( 0 ) as ITerminalNode;

            IArchBenchPlugIn plugin;
            if ( int.TryParse( ident?.Symbol.Text, out var id ) )
            {
                plugin = Server.Manager.Get( id - 1 );
            }
            else
            {
                var name = ident?.Symbol.Text.Trim( '\'' );
                plugin = Server.Manager.Find( name );
            }

            if ( plugin.Settings.Contains( property?.Symbol.Text ) )
            {
                plugin.Settings[property?.Symbol.Text] = value?.Symbol.Text;
            }
        }
    }
}
