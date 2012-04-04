using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using dfproto;
using HeavyDuck.DF.DFHack;
using HeavyDuck.Utilities.Collections;
using HeavyDuck.Utilities.Forms;

namespace HeavyDuck.DF.DwarfDuck
{
    public partial class MainForm : Form
    {
        private const string TOOL_MAIN_REFRESH = "MAIN_REFRESH";

        private const string COLUMN_LABORS_NAME = "labors_name";
        private const string COLUMN_LABORS_ASSIGNED = "labors_assigned";
        private const string COLUMN_LABORS_POTENTIAL = "labors_skilled";

        private const string COLUMN_DWARF_GENDER_IMAGE = "dwarf_gender_image";
        private const string COLUMN_DWARF_LABORS = "dwarf_labors";
        private const string COLUMN_DWARF_MENIAL_COUNT = "dwarf_menial_count";
        private const string COLUMN_DWARF_NAME = "dwarf_name";
        private const string COLUMN_DWARF_PROFESSION = "dwarf_profession";
        private const string COLUMN_DWARF_IMAGE = "dwarf_image";
        private const string COLUMN_DWARF_SKILLS = "dwarf_skills";

        public MainForm()
        {
            InitializeComponent();

            ToolStripManager.RenderMode = ToolStripManagerRenderMode.Professional;

            this.Load += new EventHandler(MainForm_Load);
            this.Shown += new EventHandler(MainForm_Shown);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // configure toolbar
            toolStrip.AutoSize = false;
            toolStrip.Font = new Font("Verdana", 10, FontStyle.Bold);
            toolStrip.Height = 32;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Padding = new Padding(6, 2, 6, 0);
            toolStrip.Items.Add(new ToolStripButton("Refresh", Properties.Resources.arrow_circle_double, toolStrip_Refresh, TOOL_MAIN_REFRESH));

            // configure grid - labors
            GridHelper.Initialize(grid_labors, true);
            grid_labors.MultiSelect = true;
            grid_labors.RowTemplate.Height = 20;
            grid_labors.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid_labors.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "Caption",
                HeaderText = "Name",
                Name = COLUMN_LABORS_NAME,
            });
            grid_labors.Columns.Add(new DwarfListColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DataPropertyName = "UnitsAssigned",
                HeaderText = "Assigned",
                Name = COLUMN_LABORS_ASSIGNED,
                Width = 100,
            });
            grid_labors.Columns.Add(new DwarfListColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DataPropertyName = "UnitsPotential",
                HeaderText = "Others Skilled",
                Name = COLUMN_LABORS_POTENTIAL,
                Width = 100,
            });
            grid_labors.CellFormatting += new DataGridViewCellFormattingEventHandler(grid_labors_CellFormatting);

            // configure grid - dwarves
            GridHelper.Initialize(grid_dwarves, true);
            grid_dwarves.MultiSelect = true;
            grid_dwarves.RowTemplate.Height = 20;
            grid_dwarves.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid_dwarves.Columns.Add(new DataGridViewImageColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DataPropertyName = "Gender",
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                },
                HeaderText = "",
                Name = COLUMN_DWARF_GENDER_IMAGE,
                Width = 20,
            });
            grid_dwarves.Columns.Add(new DataGridViewImageColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DataPropertyName = "Image",
                HeaderText = "",
                Name = COLUMN_DWARF_IMAGE,
                Width = DwarfGraphics.GetDefaultImage().Width + 2,
            });
            grid_dwarves.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "Name",
                HeaderText = "Name",
                Name = COLUMN_DWARF_NAME,
            });
            grid_dwarves.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "Profession",
                HeaderText = "Profession",
                Name = COLUMN_DWARF_PROFESSION,
            });
            grid_dwarves.Columns.Add(new DwarfListColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DataPropertyName = "LaborsView",
                HeaderText = "Labors",
                Name = COLUMN_DWARF_LABORS,
                Width = 100,
            });
            grid_dwarves.Columns.Add(new DwarfListColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DataPropertyName = "LaborsPotentialView",
                HeaderText = "Other Skills",
                Name = COLUMN_DWARF_SKILLS,
                Width = 100,
            });
            grid_dwarves.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "MenialLaborCount",
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                },
                HeaderText = "Menials",
                Name = COLUMN_DWARF_MENIAL_COUNT,
            });
            grid_dwarves.CellFormatting += new DataGridViewCellFormattingEventHandler(grid_dwarves_CellFormatting);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            GameData.Initialize();
        }

        private void grid_labors_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        }

        private void grid_dwarves_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            switch (grid_dwarves.Columns[e.ColumnIndex].Name)
            {
                case COLUMN_DWARF_GENDER_IMAGE:
                    var gender = e.Value as int?;
                    if (gender == null) return;

                    switch (gender.Value)
                    {
                        case 0:
                            e.Value = Properties.Resources.gender_female;
                            break;
                        case 1:
                            e.Value = Properties.Resources.gender;
                            break;
                        default:
                            e.Value = null;
                            break;
                    }

                    break;
                case COLUMN_DWARF_MENIAL_COUNT:
                    if ((int)e.Value < 1)
                        e.CellStyle.ForeColor = Color.Gray;
                    break;
            }
        }

        private void toolStrip_Refresh(object sender, EventArgs e)
        {
            DFHackReply<ListUnitsOut> reply;

            using (var client = new DFHackClient())
            {
                client.Open();
                reply = client.ListUnits();
            }

            var dwarves = new ObjectBindingList<Dwarf>(reply.Data.ValueList
                .Where(u => u.Race == GameData.World.RaceId)
                .Select(u => new Dwarf(u)));

            GameData.UpdateLabors(dwarves);
            grid_dwarves.DataSource = dwarves;
            grid_labors.DataSource = new ObjectBindingList<DwarfLabor>(GameData.GetLabors());
        }
    }
}
