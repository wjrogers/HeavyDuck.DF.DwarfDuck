namespace HeavyDuck.DF.DwarfDuck
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.grid_dwarves = new HeavyDuck.Utilities.Forms.BufferedDataGridView();
            this.split_main = new System.Windows.Forms.SplitContainer();
            this.grid_labors = new HeavyDuck.Utilities.Forms.BufferedDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.grid_dwarves)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.split_main)).BeginInit();
            this.split_main.Panel1.SuspendLayout();
            this.split_main.Panel2.SuspendLayout();
            this.split_main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_labors)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(884, 25);
            this.toolStrip.TabIndex = 0;
            // 
            // grid_dwarves
            // 
            this.grid_dwarves.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid_dwarves.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_dwarves.Location = new System.Drawing.Point(0, 0);
            this.grid_dwarves.Name = "grid_dwarves";
            this.grid_dwarves.Size = new System.Drawing.Size(586, 623);
            this.grid_dwarves.TabIndex = 0;
            // 
            // split_main
            // 
            this.split_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split_main.Location = new System.Drawing.Point(0, 25);
            this.split_main.Name = "split_main";
            // 
            // split_main.Panel1
            // 
            this.split_main.Panel1.Controls.Add(this.grid_labors);
            this.split_main.Panel1MinSize = 200;
            // 
            // split_main.Panel2
            // 
            this.split_main.Panel2.Controls.Add(this.grid_dwarves);
            this.split_main.Panel2MinSize = 200;
            this.split_main.Size = new System.Drawing.Size(884, 623);
            this.split_main.SplitterDistance = 294;
            this.split_main.TabIndex = 2;
            // 
            // grid_labors
            // 
            this.grid_labors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid_labors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_labors.Location = new System.Drawing.Point(0, 0);
            this.grid_labors.Name = "grid_labors";
            this.grid_labors.Size = new System.Drawing.Size(294, 623);
            this.grid_labors.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 648);
            this.Controls.Add(this.split_main);
            this.Controls.Add(this.toolStrip);
            this.Name = "MainForm";
            this.Text = "Dwarf Duck";
            ((System.ComponentModel.ISupportInitialize)(this.grid_dwarves)).EndInit();
            this.split_main.Panel1.ResumeLayout(false);
            this.split_main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split_main)).EndInit();
            this.split_main.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid_labors)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private HeavyDuck.Utilities.Forms.BufferedDataGridView grid_dwarves;
        private System.Windows.Forms.SplitContainer split_main;
        private HeavyDuck.Utilities.Forms.BufferedDataGridView grid_labors;
    }
}

