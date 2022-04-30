using System;
using ArchBench.PlugIns;

namespace ArchBench.PlugIns
{
    public class ConsoleLogger : IArchBenchLogger
    {
        public ConsoleLogger()
        {
            IndentLevel = 0;
            IsNewLine = true;
        }

        public  int  IndentLevel { get; set; }
        private bool IsNewLine   { get; set; }

        public void Write( string aMessage )
        {
            if ( IsNewLine )
            {
                for (var i = 0; i < IndentLevel; i++)
                    Console.Write("    ");
                IsNewLine = false;
            }
            Console.Write( aMessage );
        }

        public void Write(string aFormat, params object[] aArgs)
        {
            Write(string.Format( aFormat, aArgs ) );
        }

        private void WriteLine()
        {
            Console.WriteLine();
            IsNewLine = true;
        }

        public void WriteLine( string aFormat, params object[] aArgs )
        {
            WriteLine(string.Format( aFormat, aArgs ) );
        }

        public void WriteLine( string aMessage = @"" )
        {
            Write( aMessage );
            WriteLine();
        }
    }
}
