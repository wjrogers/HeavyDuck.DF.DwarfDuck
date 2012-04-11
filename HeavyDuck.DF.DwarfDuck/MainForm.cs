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

        private const string COLUMN_LABORS_ASSIGNED = "labors_assigned";
        private const string COLUMN_LABORS_CATEGORY = "labors_category";
        private const string COLUMN_LABORS_NAME = "labors_name";
        private const string COLUMN_LABORS_POTENTIAL = "labors_skilled";

        private const string COLUMN_DWARF_GENDER_IMAGE = "dwarf_gender_image";
        private const string COLUMN_DWARF_LABORS = "dwarf_labors";
        private const string COLUMN_DWARF_UNSKILLED_COUNT = "dwarf_unskilled_count";
        private const string COLUMN_DWARF_NAME = "dwarf_name";
        private const string COLUMN_DWARF_PROFESSION = "dwarf_profession";
        private const string COLUMN_DWARF_IMAGE = "dwarf_image";
        private const string COLUMN_DWARF_SKILLS = "dwarf_skills";

        private readonly Font m_font_header;
        private readonly DwarfTip m_dwarftip;

        private Func<Dwarf, bool> m_filter_dwarf;
        private string m_filter_dwarf_caption;
        private Func<DwarfLabor, bool> m_filter_labor;
        private string m_filter_labor_caption;

        public MainForm()
        {
            InitializeComponent();

            ToolStripManager.RenderMode = ToolStripManagerRenderMode.Professional;

            m_font_header = new Font("Verdana", 9f, FontStyle.Bold);
            m_dwarftip = new DwarfTip();

            this.Load += new EventHandler(MainForm_Load);
            this.Shown += new EventHandler(MainForm_Shown);
            button_filter_clear_dwarf.Click += new EventHandler(button_filter_clear_dwarf_Click);
            button_filter_clear_labor.Click += new EventHandler(button_filter_clear_labor_Click);
        }

        #region Event Handlers

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
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                HeaderText = "",
                Name = COLUMN_LABORS_CATEGORY,
                Width = 10,
            });
            grid_labors.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "Caption",
                HeaderText = "Name",
                Name = COLUMN_LABORS_NAME,
            });
            grid_labors.Columns.Add(new DwarfListColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "UnitsAssigned",
                HeaderText = "Assigned",
                Name = COLUMN_LABORS_ASSIGNED,
                Width = 100,
            });
            grid_labors.Columns.Add(new DwarfListColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "UnitsPotential",
                HeaderText = "Others Skilled",
                Name = COLUMN_LABORS_POTENTIAL,
                Width = 100,
            });
            grid_labors.Columns.Add(new DataGridViewTextBoxColumn() { AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            grid_labors.CellDoubleClick += new DataGridViewCellEventHandler(grid_labors_CellDoubleClick);
            grid_labors.CellFormatting += new DataGridViewCellFormattingEventHandler(grid_labors_CellFormatting);
            grid_labors.CellMouseLeave += new DataGridViewCellEventHandler(grid_CellMouseLeave);
            grid_labors.CellMouseMove += new DataGridViewCellMouseEventHandler(grid_CellMouseMove);
            grid_labors.RowsAdded += new DataGridViewRowsAddedEventHandler(grid_labors_RowsAdded);

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
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "LaborsView",
                HeaderText = "Skilled Labors",
                Name = COLUMN_DWARF_LABORS,
                Width = 100,
            });
            grid_dwarves.Columns.Add(new DwarfListColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "LaborsPotentialView",
                HeaderText = "Other Skills",
                Name = COLUMN_DWARF_SKILLS,
                Width = 100,
            });
            grid_dwarves.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "LaborsUnskilledCount",
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                },
                HeaderText = "Unskilled",
                Name = COLUMN_DWARF_UNSKILLED_COUNT,
            });
            grid_dwarves.Columns.Add(new DataGridViewTextBoxColumn() { AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            grid_dwarves.CellDoubleClick += new DataGridViewCellEventHandler(grid_dwarves_CellDoubleClick);
            grid_dwarves.CellFormatting += new DataGridViewCellFormattingEventHandler(grid_dwarves_CellFormatting);
            grid_dwarves.CellMouseLeave += new DataGridViewCellEventHandler(grid_CellMouseLeave);
            grid_dwarves.CellMouseMove += new DataGridViewCellMouseEventHandler(grid_CellMouseMove);

            // initialize filter UI state
            ApplyFilters();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            GameData.Initialize();
        }

        private void button_filter_clear_dwarf_Click(object sender, EventArgs e)
        {
            ClearFilterDwarf();
            ApplyFilters();
        }

        private void button_filter_clear_labor_Click(object sender, EventArgs e)
        {
            ClearFilterLabor();
            ApplyFilters();
        }

        private void grid_labors_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var labor = grid_labors.Rows[e.RowIndex].DataBoundItem as DwarfLabor;
            if (labor == null) return;

            if (labor.IsHeader)
            {
                m_filter_dwarf = d => d.Labors.Any(l => l.Category == labor.Category);
                m_filter_dwarf_caption = string.Format("Matching category {0}", labor.Category);
                ClearFilterLabor();
            }
            else
            {
                m_filter_dwarf = d => d.Labors.Contains(labor);
                m_filter_dwarf_caption = string.Format("Matching labor {0}", labor.Caption);
                ClearFilterLabor();
            }

            ApplyFilters();
        }

        private void grid_labors_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Tuple<Color, Color> colors;
            IList<DwarfListItem> dwarves;
            var labor = grid_labors.Rows[e.RowIndex].DataBoundItem as DwarfLabor;
            var column = grid_labors.Columns[e.ColumnIndex];
            if (labor == null) return;

            if (labor.IsHeader)
            {
                colors = GameData.GetCategoryColors(labor.Category);
                e.CellStyle.Font = m_font_header;
                e.CellStyle.ForeColor = colors.Item1;
                e.CellStyle.BackColor = colors.Item2;
            }
            else if (column.Name == COLUMN_LABORS_CATEGORY)
            {
                colors = GameData.GetCategoryColors(labor.Category);
                e.CellStyle.BackColor = colors.Item2;
            }
            else if (column.Name == COLUMN_LABORS_ASSIGNED && !labor.HasSkill)
            {
                dwarves = e.Value as IList<DwarfListItem>;

                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                e.FormattingApplied = true;

                if (dwarves != null && dwarves.Count > 0)
                    e.Value = string.Format("{0} dwarves", dwarves.Count);
                else
                    e.Value = "";
            }
            else if (column.Name == COLUMN_LABORS_POTENTIAL && !labor.HasSkill)
            {
                e.FormattingApplied = true;
                e.Value = "";
            }
        }

        private void grid_labors_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int index = e.RowIndex; index - e.RowIndex < e.RowCount; index++)
            {
                var row = grid_labors.Rows[index];
                var labor = row.DataBoundItem as DwarfLabor;
                if (labor == null) return;

                // don't use the dwarf list for unskilled labors that may have many workers
                if (!labor.IsHeader && !labor.HasSkill)
                {
                    row.Cells[COLUMN_LABORS_ASSIGNED] = new DataGridViewTextBoxCell();
                    row.Cells[COLUMN_LABORS_POTENTIAL] = new DataGridViewTextBoxCell();
                }
            }
        }

        private void grid_dwarves_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var dwarf = grid_dwarves.Rows[e.RowIndex].DataBoundItem as Dwarf;
            if (dwarf == null) return;

            ClearFilterDwarf();
            m_filter_labor = l => (l.IsHeader && dwarf.Labors.Any(dl => dl.Category == l.Category)) || dwarf.Labors.Contains(l);
            m_filter_labor_caption = string.Format("Matching {0}", dwarf.Name);
            ApplyFilters();
        }

        private void grid_dwarves_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dwarf = grid_dwarves.Rows[e.RowIndex].DataBoundItem as Dwarf;
            if (dwarf == null) return;

            // general stuff
            if (dwarf.IsMigrant)
                e.CellStyle.BackColor = Color.FromArgb(200, 200, 255);

            // column-specific stuff
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
                case COLUMN_DWARF_UNSKILLED_COUNT:
                    int count = (int)e.Value;

                    if (count > 0)
                    {
                        e.FormattingApplied = true;
                        e.Value = string.Format("{0} labors", count);
                    }
                    else
                    {
                        e.FormattingApplied = true;
                        e.Value = "";
                    }
                    break;
            }
        }

        private void grid_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (m_dwarftip.Visible)
                m_dwarftip.Hide();
        }

        private void grid_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            Rectangle rect;
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;

            // get the grid, cell, and dwarf the mouse is over
            var grid = sender as DataGridView;
            var cell = grid[e.ColumnIndex, e.RowIndex] as DwarfListCell;
            if (cell == null || e.RowIndex < 0) return;
            var dwarf = cell.DwarfHitTest(e);

            // no hit?
            if (dwarf == null)
            {
                if (m_dwarftip.Visible)
                    m_dwarftip.Hide();
                return;
            }

            // get the display rectangle of the cell we are hovering
            rect = grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            rect = grid.RectangleToScreen(rect);

            // prepare the tip
            m_dwarftip.SetData(dwarf, grid == grid_labors ? DwarfListMode.Labor : DwarfListMode.Dwarf);
            m_dwarftip.AutoPosition(this, rect);

            // show or redraw it
            if (!m_dwarftip.Visible)
                m_dwarftip.Show(this);
            else
                m_dwarftip.Invalidate();
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
            ApplyFilters();
        }

        #endregion

        private void ApplyFilters()
        {
            // filter dwarf list
            var dwarves = grid_dwarves.DataSource as ObjectBindingList<Dwarf>;
            if (dwarves != null)
            {
                dwarves.ApplyFilter(m_filter_dwarf);
                label_filter_dwarf.Text = m_filter_dwarf_caption;
                panel_dwarves.Visible = m_filter_dwarf != null;
            }
            else
            {
                panel_dwarves.Visible = false;
            }

            // filter labor list
            var labors = grid_labors.DataSource as ObjectBindingList<DwarfLabor>;
            if (labors != null)
            {
                labors.ApplyFilter(m_filter_labor);
                label_filter_labor.Text = m_filter_labor_caption;
                panel_labors.Visible = m_filter_labor != null;
            }
            else
            {
                panel_labors.Visible = false;
            }

            // clear annoying default selections
            grid_dwarves.ClearSelection();
            grid_labors.ClearSelection();
        }

        private void ClearFilterDwarf()
        {
            m_filter_dwarf = null;
            m_filter_dwarf_caption = null;
        }

        private void ClearFilterLabor()
        {
            m_filter_labor = null;
            m_filter_labor_caption = null;
        }
    }
}
