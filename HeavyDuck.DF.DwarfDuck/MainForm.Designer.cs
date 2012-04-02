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
            this.grid_dwarves = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.grid_dwarves)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(613, 25);
            this.toolStrip.TabIndex = 0;
            // 
            // grid_dwarves
            // 
            this.grid_dwarves.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid_dwarves.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_dwarves.Location = new System.Drawing.Point(0, 25);
            this.grid_dwarves.Name = "grid_dwarves";
            this.grid_dwarves.Size = new System.Drawing.Size(613, 623);
            this.grid_dwarves.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 648);
            this.Controls.Add(this.grid_dwarves);
            this.Controls.Add(this.toolStrip);
            this.Name = "MainForm";
            this.Text = "Dwarf Duck";
            ((System.ComponentModel.ISupportInitialize)(this.grid_dwarves)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.DataGridView grid_dwarves;
    }
}

