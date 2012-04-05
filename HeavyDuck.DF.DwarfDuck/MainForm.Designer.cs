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
            this.panel_labors = new System.Windows.Forms.Panel();
            this.label_filter_labor = new System.Windows.Forms.Label();
            this.button_filter_clear_labor = new System.Windows.Forms.Button();
            this.panel_dwarves = new System.Windows.Forms.Panel();
            this.label_filter_dwarf = new System.Windows.Forms.Label();
            this.button_filter_clear_dwarf = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grid_dwarves)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.split_main)).BeginInit();
            this.split_main.Panel1.SuspendLayout();
            this.split_main.Panel2.SuspendLayout();
            this.split_main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_labors)).BeginInit();
            this.panel_labors.SuspendLayout();
            this.panel_dwarves.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1184, 25);
            this.toolStrip.TabIndex = 0;
            // 
            // grid_dwarves
            // 
            this.grid_dwarves.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid_dwarves.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_dwarves.Location = new System.Drawing.Point(0, 35);
            this.grid_dwarves.Name = "grid_dwarves";
            this.grid_dwarves.Size = new System.Drawing.Size(787, 702);
            this.grid_dwarves.TabIndex = 1;
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
            this.split_main.Panel1.Controls.Add(this.panel_labors);
            this.split_main.Panel1MinSize = 200;
            // 
            // split_main.Panel2
            // 
            this.split_main.Panel2.Controls.Add(this.grid_dwarves);
            this.split_main.Panel2.Controls.Add(this.panel_dwarves);
            this.split_main.Panel2MinSize = 200;
            this.split_main.Size = new System.Drawing.Size(1184, 737);
            this.split_main.SplitterDistance = 393;
            this.split_main.TabIndex = 2;
            // 
            // grid_labors
            // 
            this.grid_labors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid_labors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_labors.Location = new System.Drawing.Point(0, 35);
            this.grid_labors.Name = "grid_labors";
            this.grid_labors.Size = new System.Drawing.Size(393, 702);
            this.grid_labors.TabIndex = 1;
            // 
            // panel_labors
            // 
            this.panel_labors.Controls.Add(this.button_filter_clear_labor);
            this.panel_labors.Controls.Add(this.label_filter_labor);
            this.panel_labors.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_labors.Location = new System.Drawing.Point(0, 0);
            this.panel_labors.Name = "panel_labors";
            this.panel_labors.Size = new System.Drawing.Size(393, 35);
            this.panel_labors.TabIndex = 0;
            // 
            // label_filter_labor
            // 
            this.label_filter_labor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_filter_labor.AutoEllipsis = true;
            this.label_filter_labor.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_filter_labor.Location = new System.Drawing.Point(3, 6);
            this.label_filter_labor.Name = "label_filter_labor";
            this.label_filter_labor.Size = new System.Drawing.Size(306, 23);
            this.label_filter_labor.TabIndex = 0;
            this.label_filter_labor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button_filter_clear_labor
            // 
            this.button_filter_clear_labor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_filter_clear_labor.Location = new System.Drawing.Point(315, 6);
            this.button_filter_clear_labor.Name = "button_filter_clear_labor";
            this.button_filter_clear_labor.Size = new System.Drawing.Size(75, 23);
            this.button_filter_clear_labor.TabIndex = 1;
            this.button_filter_clear_labor.Text = "Clear";
            this.button_filter_clear_labor.UseVisualStyleBackColor = true;
            // 
            // panel_dwarves
            // 
            this.panel_dwarves.Controls.Add(this.button_filter_clear_dwarf);
            this.panel_dwarves.Controls.Add(this.label_filter_dwarf);
            this.panel_dwarves.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_dwarves.Location = new System.Drawing.Point(0, 0);
            this.panel_dwarves.Name = "panel_dwarves";
            this.panel_dwarves.Size = new System.Drawing.Size(787, 35);
            this.panel_dwarves.TabIndex = 0;
            // 
            // label_filter_dwarf
            // 
            this.label_filter_dwarf.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_filter_dwarf.AutoEllipsis = true;
            this.label_filter_dwarf.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_filter_dwarf.Location = new System.Drawing.Point(3, 6);
            this.label_filter_dwarf.Name = "label_filter_dwarf";
            this.label_filter_dwarf.Size = new System.Drawing.Size(700, 23);
            this.label_filter_dwarf.TabIndex = 0;
            this.label_filter_dwarf.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button_filter_clear_dwarf
            // 
            this.button_filter_clear_dwarf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_filter_clear_dwarf.Location = new System.Drawing.Point(709, 6);
            this.button_filter_clear_dwarf.Name = "button_filter_clear_dwarf";
            this.button_filter_clear_dwarf.Size = new System.Drawing.Size(75, 23);
            this.button_filter_clear_dwarf.TabIndex = 1;
            this.button_filter_clear_dwarf.Text = "Clear";
            this.button_filter_clear_dwarf.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 762);
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
            this.panel_labors.ResumeLayout(false);
            this.panel_dwarves.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private HeavyDuck.Utilities.Forms.BufferedDataGridView grid_dwarves;
        private System.Windows.Forms.SplitContainer split_main;
        private HeavyDuck.Utilities.Forms.BufferedDataGridView grid_labors;
        private System.Windows.Forms.Panel panel_labors;
        private System.Windows.Forms.Panel panel_dwarves;
        private System.Windows.Forms.Label label_filter_labor;
        private System.Windows.Forms.Button button_filter_clear_labor;
        private System.Windows.Forms.Label label_filter_dwarf;
        private System.Windows.Forms.Button button_filter_clear_dwarf;
    }
}

