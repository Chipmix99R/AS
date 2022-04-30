namespace ArchBench.PlugIns
{
    public interface IArchBenchLogger
    {
        int IndentLevel { get; set; }

        void Write( string aMessage );
        void Write( string aFormat, params object[] aArgs );

        void WriteLine( string aMessage = @"" );
        void WriteLine( string aFormat, params object[] aArgs );
    }
}
