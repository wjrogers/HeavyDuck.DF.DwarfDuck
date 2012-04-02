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

        private DFHackReply<GetWorldInfoOut> m_world;
        private DFHackReply<ListEnumsOut> m_enums;

        private Dictionary<int, string> m_names_labor;
        private Dictionary<int, string> m_names_profession;
        private Dictionary<int, string> m_names_skill;

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

            // configure grid
            GridHelper.Initialize(grid_dwarves, true);
            grid_dwarves.MultiSelect = true;
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
                Name = "Gender",
                Width = 20,
            });
            grid_dwarves.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DataPropertyName = "Name",
                HeaderText = "Name",
                Name = "Name",
            });
            grid_dwarves.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "Race",
                HeaderText = "Race",
                Name = "Race",
            });
            grid_dwarves.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "Profession",
                HeaderText = "Profession",
                Name = "Profession",
            });
            grid_dwarves.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DataPropertyName = "SkillsList",
                HeaderText = "Skills",
                Name = "SkillsList",
            });
            grid_dwarves.Columns.Add(new DataGridViewImageColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "Profession",
                HeaderText = "Profession",
                Name = "Profession",
            });
            grid_dwarves.CellFormatting += new DataGridViewCellFormattingEventHandler(grid_dwarves_CellFormatting);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            using (var client = new DFHackClient())
            {
                client.Open();

                m_world = client.GetWorldInfo();
                m_enums = client.ListEnums();
            }

            m_names_labor = m_enums.Data.UnitLaborList.ToDictionary(o => o.Value, o => o.Name);
            m_names_profession = m_enums.Data.ProfessionList.ToDictionary(o => o.Value, o => o.Name);
            m_names_skill = m_enums.Data.JobSkillList.ToDictionary(o => o.Value, o => o.Name);
        }

        private void grid_dwarves_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
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
            }
            else if (e.ColumnIndex == 1)
            {
                var name = e.Value as NameInfo;
                if (name == null) return;

                e.Value = string.Format("{0} {1}",
                    CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name.FirstName),
                    CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name.LastName));
            }
            else if (e.ColumnIndex == 2)
            {
                var id = e.Value as int?;
                if (id == null) return;

                if (id.Value == m_world.Data.RaceId)
                    e.Value = "Dwarf";
                else
                    e.Value = "?";
            }
            else if (e.ColumnIndex == 3)
            {
                string name;
                var id = e.Value as int?;
                if (id == null) return;

                if (m_names_profession.TryGetValue(id.Value, out name))
                {
                    e.Value = name;
                }
                else
                {
                    e.Value = "Unknown";
                    e.CellStyle.ForeColor = Color.LightGray;
                }
            }
            else if (e.ColumnIndex == 4)
            {
                string name;
                var skills = e.Value as IList<SkillInfo>;
                if (skills == null || skills.Count < 1) return;

                var value = new StringBuilder();
                foreach (var skill in skills)
                    value.AppendFormat("{0} {1}, ", m_names_skill.TryGetValue(skill.Id, out name) ? name : "Unknown", skill.Level);
                if (value.Length > 2)
                    value.Remove(value.Length - 2, 2);

                e.Value = value.ToString();
            }
            else if (e.ColumnIndex == 5)
            {
                string name;
                var id = e.Value as int?;
                if (id == null) return;

                if (m_names_profession.TryGetValue(id.Value, out name))
                    e.Value = DwarfGraphics.GetImage(name) ?? DwarfGraphics.GetDefaultImage();
                else
                    e.Value = null;
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

            grid_dwarves.DataSource = new ObjectBindingList<BasicUnitInfo>(reply.Data.ValueList);
        }
    }
}
