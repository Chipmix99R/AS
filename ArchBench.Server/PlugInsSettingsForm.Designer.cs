namespace ArchBench.Server
{
    partial class PlugInsSettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlugInsSettingsForm));
            this.mSettingsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // mSettingsPropertyGrid
            // 
            this.mSettingsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mSettingsPropertyGrid.HelpVisible = false;
            this.mSettingsPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.mSettingsPropertyGrid.Name = "mSettingsPropertyGrid";
            this.mSettingsPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.mSettingsPropertyGrid.Size = new System.Drawing.Size(776, 514);
            this.mSettingsPropertyGrid.TabIndex = 0;
            this.mSettingsPropertyGrid.ToolbarVisible = false;
            // 
            // PlugInsSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 514);
            this.Controls.Add(this.mSettingsPropertyGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PlugInsSettingsForm";
            this.Text = "Plug-In Settings";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid mSettingsPropertyGrid;
    }
}