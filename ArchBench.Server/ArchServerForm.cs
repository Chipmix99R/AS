using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using System.Net;
using ArchBench.PlugIns;
using ArchBench.Server.Kernel;
using ArchBench.Server.Configurations;


namespace ArchBench.Server
{
    public partial class ArchServerForm : Form
    {

        public ArchServerForm( string[] aArgs )
        {
            InitializeComponent();
            Logger = new TextBoxLogger( mOutput );
            Server = new PlugInsServer( Logger );

            mPort.Text = Server.Port.ToString();

            mPlugInsListView.SelectedIndexChanged +=
                (sender, args) => mSettingsToolStripButton.Enabled = mPlugInsListView.SelectedItems.Count > 0;
            mPlugInsListView.SelectedIndexChanged +=
                (sender, args) => mRemoveToolStripButton.Enabled = mPlugInsListView.SelectedItems.Count > 0;

            mNameColumnHeader.ImageIndex = 3;
        }

        private IArchBenchLogger Logger { get; }
        public  PlugInsServer    Server { get; }

        #region Toolbar Double Click problem

        private bool HandleFirstClick { get; set; } = false;

        protected override void OnActivated( EventArgs e )
        {
            base.OnActivated( e );
            if (HandleFirstClick)
            {
                var position = Cursor.Position;
                var point = this.PointToClient(position);
                var child = this.GetChildAtPoint(point);
                while ( HandleFirstClick && child != null )
                {
                    if (child is ToolStrip toolStrip)
                    {
                        HandleFirstClick = false;
                        point = toolStrip.PointToClient(position);
                        foreach (var item in toolStrip.Items)
                        {
                            if (item is ToolStripItem toolStripItem && toolStripItem.Bounds.Contains(point))
                            {
                                if (item is ToolStripMenuItem tsMenuItem)
                                {
                                    tsMenuItem.ShowDropDown();
                                }
                                else
                                {
                                    toolStripItem.PerformClick();
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        child = child.GetChildAtPoint(point);
                    }
                }
                HandleFirstClick = false;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_ACTIVATE = 0x0006;
            const int WA_CLICKACTIVE = 0x0002;
            if (m.Msg == WM_ACTIVATE && Low16(m.WParam) == WA_CLICKACTIVE)
            {
                HandleFirstClick = true;
            }
            base.WndProc( ref m );
        }

        private static int GetIntUnchecked(IntPtr value)
        {
            return IntPtr.Size == 8 ? unchecked((int)value.ToInt64()) : value.ToInt32();
        }

        private static int Low16(IntPtr value)
        {
            return unchecked((short)GetIntUnchecked(value));
        }

        private static int High16(IntPtr value)
        {
            return unchecked((short)(((uint)GetIntUnchecked(value)) >> 16));
        }

        #endregion
        
        private void OnExit(object sender, EventArgs e)
        {
            Server?.Stop();
            Application.Exit();
        }

        private void OnClearConsole(object sender, EventArgs e)
        {
            mOutput.Text = string.Empty;
        }

        private void OnConnect(object sender, EventArgs e)
        {
            mConnectTool.Checked = ! mConnectTool.Checked;
            if ( mConnectTool.Checked )
            {
                if ( int.TryParse( mPort.Text, out int port ) )
                {
                    Server.Start( port );
                    Logger.WriteLine("Server online on port {0}", port );
                    mConnectTool.Image = Properties.Resources.connect;

                    mPort.Enabled = false;
                    mPort.BackColor = Color.PaleGreen;
                }
                else
                {
                    Logger.WriteLine("Invalid port '{0}' specification", mPort.Text );
                }
            }
            else
            {
                Server.Stop();
                mConnectTool.Image = Properties.Resources.disconnect;

                mPort.Enabled = true;
                mPort.BackColor = Color.Empty;
            }
        }

        private int GetPort()
        {
            return int.TryParse( mPort.Text, out var port ) ? port : Server?.Port ?? 8081;
        }

        private void OnNew(object sender, EventArgs e)
        {
            Server.Stop();

            mPlugInsListView.Items.Clear();
            Server.Manager.Clear();

            mPort.Text = Server.Port.ToString();
            mPort.Enabled = true;
            mPort.BackColor = Color.Empty;

            mNameColumnHeader.ImageIndex = 3;
        }

        private void OnOpen(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog {
                Filter = @"Configuration Files (*.config)|*.config|All Files (*.*)|*.*"
            };
            if ( dialog.ShowDialog() != DialogResult.OK ) return;
            
            Open( dialog.FileName );
        }

        private async void Open( string aFileName )
        {
            mPlugInsListView.BeginUpdate();
            using (var stream = File.OpenRead( aFileName))
            {
                var config = await JsonSerializer.DeserializeAsync<ServerConfig>(stream);
                if (config == null) return;

                mPort.Text = config.Port.ToString();
                Server.Manager.Clear();
                foreach (var plugin in config.PlugIns)
                {
                    try
                    {
                        var instance = Server.Manager.Add(plugin.FileName, plugin.FullName);
                        if (instance == null) continue;

                        foreach (var key in plugin.Settings.Keys)
                        {
                            instance.Settings[key] = plugin.Settings[key];
                        }

                        Append(instance);
                    }
                    catch ( FileNotFoundException e )
                    {
                        Logger.WriteLine($"Cannot load plugin: { e.FileName }");
                    }
                }
            }
            mPlugInsListView.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            mPlugInsListView.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.HeaderSize);
            mPlugInsListView.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);
            mPlugInsListView.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.ColumnContent);
            mPlugInsListView.EndUpdate();
        }

        private async void OnSave(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog() {
                Filter = @"Configuration Files (*.config)|*.config"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            var config = new ServerConfig { Port = GetPort() };
            foreach ( var plugin in Server.Manager.PlugIns )
            {
                var filename = Server.Manager.GetFileName( plugin );
                if ( config.PlugIns.Any( c => c.FileName.Equals( filename ) ) ) continue;

                var elem = new PlugInConfig {
                    FullName = plugin.GetType().FullName,
                    FileName = filename
                };

                foreach ( var setting in plugin.Settings )
                {
                    elem.Settings.Add(setting, plugin.Settings[setting]);
                }
                config.PlugIns.Add( elem  );
            }

            using ( var stream = File.Create( dialog.FileName ) )
            {
                await JsonSerializer.SerializeAsync( stream, config );
            }
        }

        private void Append( IArchBenchPlugIn aPlugIn )
        {
            var item = new ListViewItem
            {
                Text = aPlugIn.Name,
                Checked = aPlugIn.Enabled,
                ImageIndex = 0,
                Tag = aPlugIn
            };

            item.SubItems.Add(aPlugIn.Version);
            item.SubItems.Add(aPlugIn.Author);
            item.SubItems.Add(aPlugIn.Description);

            mPlugInsListView.Items.Add(item);
        }

        private void OnPlugInAppend(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog() { Multiselect = true };
            dialog.Filter = @"Arch.Bench PlugIn File (*.dll)|*.dll";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var name in dialog.FileNames)
                {
                    var plugins = Server.Manager.Add( name );
                    foreach (var plugin in plugins)
                    {
                        Append(plugin);
                    }
                }
            }
        }

        private void OnPlugInRemove(object sender, EventArgs e)
        {
            foreach (ListViewItem item in mPlugInsListView.SelectedItems)
            {
                var plugin = (IArchBenchPlugIn)item.Tag;
                if (plugin == null) continue;

                Server.Manager.Remove(plugin);
                item.Remove();
            }

        }

        private void OnPlugInSettings(object sender, EventArgs e)
        {
            if (mPlugInsListView.SelectedItems.Count == 0) return;

            var dialog = new PlugInsSettingsForm(mPlugInsListView.SelectedItems[0].Tag as IArchBenchPlugIn);
            dialog.ShowDialog();
        }

        private void OnPlugInItemChecked(object sender, ItemCheckedEventArgs e)
        {
            e.Item.ImageIndex = e.Item.Checked ? 0 : 1;
            e.Item.ForeColor = e.Item.Checked ? Color.Empty : Color.Gray;

            var plugin = (IArchBenchPlugIn) e.Item.Tag;
            if (plugin != null) plugin.Enabled = e.Item.Checked;
        }

        private void OnColumnClick( object sender, ColumnClickEventArgs aArgs )
        {
            if ( aArgs.Column != 0 ) return;

            var check = mNameColumnHeader.ImageIndex == 3;
            foreach ( ListViewItem item in mPlugInsListView.Items )
            {
                var plugin = (IArchBenchPlugIn)item.Tag;
                if (plugin == null) continue;

                plugin.Enabled = check;
                item.Checked = check;
            }

            mNameColumnHeader.ImageIndex = mNameColumnHeader.ImageIndex == 3 ? 2 : 3;
        }

        private void OnPortChanged( object sender, EventArgs e )
        {
            if ( int.TryParse( mPort.Text, out var port ) )
            {
                Server.Port = port;
            }
        }

    }
}
