using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Antlr4.Runtime;

namespace ArchBench.Server.CLI
{
    class ArchBenchErrorListener : BaseErrorListener
    {
        public override void SyntaxError( TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine,
            string msg, RecognitionException e )
        {
        }
    }
}
