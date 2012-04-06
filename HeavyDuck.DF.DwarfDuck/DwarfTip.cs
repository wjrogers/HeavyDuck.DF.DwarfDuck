using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HeavyDuck.DF.DwarfDuck
{
    internal partial class DwarfTip : Form
    {
        private const int LAYOUT_PADDING = 6;
        private const int SKILL_BAR_BORDER = 1;
        private const int SKILL_BAR_SEGMENT = 4;
        private const int SKILL_BAR_PADDING = 1;
        private const int SKILL_BAR_WIDTH =
            (int)GameData.SKILL_MAX * (SKILL_BAR_SEGMENT + SKILL_BAR_PADDING)
            + SKILL_BAR_PADDING
            + SKILL_BAR_BORDER * 2;
        private const int SKILL_BAR_HEIGHT = 18;
        private const int SKILL_BAR_HEIGHT_INNER = SKILL_BAR_SEGMENT + SKILL_BAR_PADDING * 2 + SKILL_BAR_BORDER * 2;
        private const string MESSAGE_NO_DATA = "No data.";
        private const string MESSAGE_NONE = "None";
        private const string MESSAGE_DWARF_LABORS_SKILLED = "Skilled Labors";
        private const string MESSAGE_DWARF_LABORS_UNSKILLED = "Unskilled Labors";
        private const string MESSAGE_DWARF_SKILLS_UNUSED = "Other Skills";
        private const string MESSAGE_UNITS_ASSIGNED = "Others Assigned";
        private const string MESSAGE_UNITS_POTENTIAL = "Others Skilled";

        private readonly Font m_font_bold;
        private readonly Font m_font_bold_small;
        private readonly Font m_font_italic;
        private readonly StringFormat m_format_left_center;

        private DwarfListItem m_data;

        public DwarfTip()
        {
            InitializeComponent();

            m_font_bold = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            m_font_bold_small = new Font(this.Font.FontFamily, 8, FontStyle.Bold);
            m_font_italic = new Font(this.Font, FontStyle.Italic);
            m_format_left_center = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
            };

            this.Load += new EventHandler(DwarfTip_Load);
        }

        private void DwarfTip_Load(object sender, EventArgs e)
        {
            using (var g = this.CreateGraphics())
                this.ClientSize = DrawTip(g, false).ToSize();
        }

        public DwarfListItem Data
        {
            get { return m_data; }
            set
            {
                if (value == m_data) return;

                // store it
                m_data = value;

                // measure it
                using (var g = this.CreateGraphics())
                    this.ClientSize = DrawTip(g, false).ToSize();
            }
        }

        public void AutoPosition(Form owner, Rectangle rect)
        {
            Rectangle screen;
            Rectangle layout;

            // our default position is below and slightly to the right of the control's rect
            layout = new Rectangle(
                rect.X + LAYOUT_PADDING,
                rect.Bottom + LAYOUT_PADDING,
                this.Width,
                this.Height);
            screen = Screen.GetWorkingArea(owner);

            // do we go off the screen?
            if (layout.Bottom > screen.Bottom)
                layout.Offset(0, (rect.Top - layout.Top) - layout.Height - LAYOUT_PADDING);
            if (layout.Right > screen.Right)
                layout.Offset(-layout.Width - LAYOUT_PADDING * 2, 0);

            // assign it
            this.Location = layout.Location;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // let the normal stuff happen
            base.OnPaint(e);

            // draw our fancy tooltip
            DrawTip(e.Graphics, true);
        }

        private SizeF DrawTip(Graphics g, bool draw)
        {
            Rectangle client;
            SizeF measured;
            float width = LAYOUT_PADDING * 2;
            int y = LAYOUT_PADDING;

            // draw a nice border
            if (draw)
            {
                client = this.ClientRectangle;
                client.Width -= 1;
                client.Height -= 1;
                g.DrawRectangle(Pens.Gray, client);
            }

            // no data?
            if (m_data == null)
            {
                measured = g.MeasureString(MESSAGE_NO_DATA, m_font_italic);
                if (draw)
                    g.DrawString(MESSAGE_NO_DATA, m_font_italic, Brushes.Black, LAYOUT_PADDING, LAYOUT_PADDING);
                return SizeF.Add(measured, new SizeF(LAYOUT_PADDING * 2, LAYOUT_PADDING * 2));
            }

            // draw the dwarf name
            measured = g.MeasureString(m_data.Dwarf.Name, m_font_bold);
            if (draw)
                g.DrawString(m_data.Dwarf.Name, m_font_bold, Brushes.Black, LAYOUT_PADDING, y);
            y += (int)Math.Ceiling(measured.Height);
            width = Math.Max(width, measured.Width + LAYOUT_PADDING * 2);

            // draw this labor skill
            DrawSkillBar(g, m_data, draw, ref y, ref width, m_data.Labor.Caption);

            // depending on mode...
            switch (m_data.Mode)
            {
                case DwarfListMode.Labor:
                    // draw skilled labors
                    var list_skilled = m_data.Dwarf.Labors
                        .Where(l => l.HasSkill && l != m_data.Labor)
                        .Select(l => new DwarfListItem(l.Skill.Profession.Image, m_data.Dwarf, l, m_data.Mode));
                    DrawDwarfList(g, MESSAGE_DWARF_LABORS_SKILLED, list_skilled, draw, ref y, ref width);

                    // draw skills unused
                    var list_unused = m_data.Dwarf.Skills
                        .Where(p => !m_data.Dwarf.Labors.Contains(p.Key.Labor))
                        .Select(p => new DwarfListItem(p.Key.Profession.Image, m_data.Dwarf, p.Key.Labor, m_data.Mode));
                    DrawDwarfList(g, MESSAGE_DWARF_SKILLS_UNUSED, list_unused, draw, ref y, ref width);

                    // draw unskilled labors
                    var list_unskilled = m_data.Dwarf.Labors
                        .Where(l => !l.HasSkill)
                        .Select(l => new DwarfListItem(l.Skill.Profession.Image, m_data.Dwarf, l, m_data.Mode));
                    DrawDwarfList(g, MESSAGE_DWARF_LABORS_UNSKILLED, list_unskilled, draw, ref y, ref width);

                    break;
                case DwarfListMode.Dwarf:
                    // draw other dwarfs assigned
                    DrawDwarfList(g, MESSAGE_UNITS_ASSIGNED, m_data.Labor.UnitsAssigned.Where(i => i.Dwarf != m_data.Dwarf), draw, ref y, ref width);

                    // draw other dwarfs assigned
                    DrawDwarfList(g, MESSAGE_UNITS_POTENTIAL, m_data.Labor.UnitsPotential.Where(i => i.Dwarf != m_data.Dwarf), draw, ref y, ref width);

                    break;
            }

            // return the total size we used
            return new SizeF(width, y + LAYOUT_PADDING);
        }

        private void DrawDwarfList(Graphics g, string header, IEnumerable<DwarfListItem> dwarves, bool draw, ref int y, ref float width)
        {
            SizeF measured;
            var list = dwarves.OrderBy(d => Tuple.Create(-d.SkillInfo.Level, d.Caption)).ToList();

            // header
            y += LAYOUT_PADDING;
            measured = g.MeasureString(header, m_font_bold_small);
            if (draw)
                g.DrawString(header, m_font_bold_small, Brushes.Black, LAYOUT_PADDING, y);
            y += (int)Math.Ceiling(measured.Height);

            // no data?
            if (list.Count < 1)
            {
                measured = g.MeasureString(MESSAGE_NONE, m_font_italic);
                if (draw)
                    g.DrawString(MESSAGE_NONE, m_font_italic, Brushes.Black, LAYOUT_PADDING * 2, y);
                y += (int)Math.Ceiling(measured.Height);
            }

            // draw the list
            foreach (var item in list)
                DrawSkillBar(g, item, draw, ref y, ref width);
        }

        private void DrawSkillBar(Graphics g, DwarfListItem item, bool draw, ref int y, ref float width, string caption = null)
        {
            var brush = item.SkillInfo.Level >= GameData.SKILL_MAX ? Brushes.Gold : SystemBrushes.ControlDarkDark;
            var y_inner = y + (SKILL_BAR_HEIGHT - SKILL_BAR_HEIGHT_INNER) / 2;

            // use caption override?
            if (caption == null)
                caption = item.Caption;

            if (draw && item.Labor.HasSkill)
            {
                // draw background
                g.FillRectangle(SystemBrushes.ControlLightLight, LAYOUT_PADDING * 2, y_inner, SKILL_BAR_WIDTH, SKILL_BAR_HEIGHT_INNER);
                g.DrawRectangle(SystemPens.ControlDark, LAYOUT_PADDING * 2, y_inner, SKILL_BAR_WIDTH, SKILL_BAR_HEIGHT_INNER + 1);

                // draw the level bars
                for (int i = 1; i <= item.SkillInfo.Level && i <= GameData.SKILL_MAX; ++i)
                {
                    g.FillRectangle(
                        brush,
                        LAYOUT_PADDING * 2 + SKILL_BAR_WIDTH - SKILL_BAR_BORDER - (SKILL_BAR_SEGMENT + SKILL_BAR_PADDING) * i,
                        y_inner + SKILL_BAR_BORDER + SKILL_BAR_PADDING + 1,
                        SKILL_BAR_SEGMENT,
                        SKILL_BAR_SEGMENT);
                }
            }

            // draw the image
            if (draw)
            {
                var pos = item.Labor.HasSkill
                    ? new Point(LAYOUT_PADDING * 3 + SKILL_BAR_WIDTH, y)
                    : new Point(LAYOUT_PADDING * 2, y);
                g.DrawImageUnscaled(item.Image, pos);
            }

            // draw the caption
            var measured = g.MeasureString(caption, this.Font);
            var rect_caption = item.Labor.HasSkill
                ? new Rectangle(LAYOUT_PADDING * 4 + SKILL_BAR_WIDTH + item.Image.Width, (int)y, (int)Math.Ceiling(measured.Width), SKILL_BAR_HEIGHT)
                : new Rectangle(LAYOUT_PADDING * 3 + item.Image.Width, (int)y, (int)Math.Ceiling(measured.Width), SKILL_BAR_HEIGHT);
            if (draw)
                g.DrawString(caption, this.Font, Brushes.Black, rect_caption, m_format_left_center);
            width = Math.Max(width, rect_caption.Right + LAYOUT_PADDING);

            // increment measurements
            y += SKILL_BAR_HEIGHT + LAYOUT_PADDING / 3;
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 132;
            const int HTTRANSPARENT = -1;

            // ignore all mouse interactions
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    m.Result = (IntPtr)HTTRANSPARENT;
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
