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

        private const string COLUMN_DWARF_GENDER_IMAGE = "dwarf_gender_image";
        private const string COLUMN_DWARF_NAME = "dwarf_name";
        private const string COLUMN_DWARF_PROFESSION = "dwarf_profession";
        private const string COLUMN_DWARF_PROFESSION_IMAGE = "dwarf_profession_image";
        private const string COLUMN_DWARF_SKILLS = "dwarf_skills";

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
                DataPropertyName = "Profession",
                HeaderText = "",
                Name = COLUMN_DWARF_PROFESSION_IMAGE,
                Width = DwarfGraphics.GetDefaultImage().Width + 2,
            });
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
            grid_dwarves.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
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
            grid_dwarves.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DataPropertyName = "SkillsList",
                HeaderText = "Skills",
                Name = COLUMN_DWARF_SKILLS,
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
            string name;
            int? id;

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
                case COLUMN_DWARF_NAME:
                    var name_info = e.Value as NameInfo;
                    if (name_info == null) return;

                    e.Value = FormatDwarfName(name_info);

                    break;
                case COLUMN_DWARF_PROFESSION:
                    id = e.Value as int?;
                    if (id == null) return;

                    e.Value = FormatDwarfProfession(id.Value);

                    if ("Unknown".Equals(e.Value))
                        e.CellStyle.ForeColor = Color.LightGray;

                    break;
                case COLUMN_DWARF_PROFESSION_IMAGE:
                    id = e.Value as int?;
                    if (id == null) return;

                    if (m_names_profession.TryGetValue(id.Value, out name))
                        e.Value = DwarfGraphics.GetImage(name) ?? DwarfGraphics.GetDefaultImage();
                    else
                        e.Value = null;

                    break;
                case COLUMN_DWARF_SKILLS:
                    var skills = e.Value as IList<SkillInfo>;
                    if (skills == null || skills.Count < 1) return;

                    var value = new StringBuilder();
                    foreach (var skill in skills)
                        value.AppendFormat("{0} {1}, ", FormatDwarfSkill(skill.Id), skill.Level);
                    if (value.Length > 2)
                        value.Remove(value.Length - 2, 2);

                    e.Value = value.ToString();

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

            grid_dwarves.DataSource = new ObjectBindingList<BasicUnitInfo>(reply.Data.ValueList.Where(u => u.Race == m_world.Data.RaceId));
        }

        private string FormatDwarfName(NameInfo name)
        {
            return string.Format("{0} {1}",
                FormatName(name.FirstName),
                FormatName(name.LastName));
        }

        private string FormatDwarfProfession(int id)
        {
            string name;

            if (m_names_profession.TryGetValue(id, out name))
                return FormatName(name);
            else
                return "Unknown";
        }

        private string FormatDwarfSkill(int id)
        {
            string name;

            if (m_names_skill.TryGetValue(id, out name))
                return name;
            else
                return "UNKNOWN";
        }

        private string FormatName(string name)
        {
            name = name.Replace('_', ' ');
            name = name.ToLowerInvariant();
            name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name);

            return name;
        }
    }
}
