using System;
using System.Windows.Forms;

namespace ArchBench.Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main( string[] args )
        {
            Properties.Settings.Default.Reload();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ArchServerForm( args ));
            Properties.Settings.Default.Save();
        }
    }
}
