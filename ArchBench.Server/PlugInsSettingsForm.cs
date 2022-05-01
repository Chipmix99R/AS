using System.Windows.Forms;
using ArchBench.PlugIns;

namespace ArchBench.Server
{
    public partial class PlugInsSettingsForm : Form
    {
        public PlugInsSettingsForm( IArchBenchPlugIn aPlugIn )
        {
            InitializeComponent();
            mSettingsPropertyGrid.SelectedObject = new UI.SettingsPropertyGridAdapter( aPlugIn.Settings );
        }
    }
}
