﻿using System;
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
        private DwarfListMode m_mode = DwarfListMode.Dwarf;

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

        public void SetData(DwarfListItem data, DwarfListMode mode)
        {
            // store it
            m_data = data;
            m_mode = mode;

            // measure it
            using (var g = this.CreateGraphics())
                this.ClientSize = DrawTip(g, false).ToSize();
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
            DrawSkillBar(g, m_data, Brushes.Black, m_data.Labor.Caption, draw, ref y, ref width);

            // depending on mode...
            switch (m_mode)
            {
                case DwarfListMode.Labor:
                    // draw skilled labors
                    var list_skilled = m_data.Dwarf.Labors
                        .Where(l => l.HasSkill && l != m_data.Labor)
                        .Select(l => new DwarfListItem(l.Skill.Profession.Image, m_data.Dwarf, l));
                    DrawDwarfList(g, MESSAGE_DWARF_LABORS_SKILLED, list_skilled, DwarfListMode.Labor, Brushes.Navy, draw, ref y, ref width);

                    // draw unskilled labors
                    var list_unskilled = m_data.Dwarf.Labors
                        .Where(l => !l.HasSkill)
                        .Select(l => new DwarfListItem(l.Skill.Profession.Image, m_data.Dwarf, l));
                    DrawDwarfList(g, MESSAGE_DWARF_LABORS_UNSKILLED, list_unskilled, DwarfListMode.Labor, Brushes.Firebrick, draw, ref y, ref width);

                    // draw skills unused
                    var list_unused = m_data.Dwarf.Skills
                        .Where(p => p.Key.HasLabor && !m_data.Dwarf.Labors.Contains(p.Key.Labor))
                        .Select(p => new DwarfListItem(p.Key.Profession.Image, m_data.Dwarf, p.Key.Labor));
                    DrawDwarfList(g, MESSAGE_DWARF_SKILLS_UNUSED, list_unused, DwarfListMode.Labor, Brushes.DarkSlateGray, draw, ref y, ref width);

                    break;
                case DwarfListMode.Dwarf:
                    // draw other dwarfs assigned
                    var list_assigned = m_data.Labor.UnitsAssigned.Where(i => i.Dwarf != m_data.Dwarf);
                    DrawDwarfList(g, MESSAGE_UNITS_ASSIGNED, list_assigned, DwarfListMode.Dwarf, Brushes.Navy, draw, ref y, ref width);

                    // draw other dwarfs assigned
                    var list_potential = m_data.Labor.UnitsPotential.Where(i => i.Dwarf != m_data.Dwarf);
                    DrawDwarfList(g, MESSAGE_UNITS_POTENTIAL, list_potential, DwarfListMode.Dwarf, Brushes.DarkSlateGray, draw, ref y, ref width);

                    break;
            }

            // return the total size we used
            return new SizeF(width, y + LAYOUT_PADDING);
        }

        private void DrawDwarfList(Graphics g, string header, IEnumerable<DwarfListItem> dwarves, DwarfListMode mode, Brush brush, bool draw, ref int y, ref float width)
        {
            SizeF measured;
            var list = dwarves.OrderBy(d => Tuple.Create(-d.SkillInfo.Level, d.GetCaption(mode))).ToList();

            // header
            y += LAYOUT_PADDING;
            measured = g.MeasureString(header, m_font_bold_small);
            if (draw)
                g.DrawString(header, m_font_bold_small, brush, LAYOUT_PADDING, y);
            y += (int)Math.Ceiling(measured.Height);

            // no data?
            if (list.Count < 1)
            {
                measured = g.MeasureString(MESSAGE_NONE, m_font_italic);
                if (draw)
                    g.DrawString(MESSAGE_NONE, m_font_italic, brush, LAYOUT_PADDING * 2, y);
                y += (int)Math.Ceiling(measured.Height);
            }

            // draw the list
            foreach (var item in list)
                DrawSkillBar(g, item, brush, item.GetCaption(mode), draw, ref y, ref width);
        }

        private void DrawSkillBar(Graphics g, DwarfListItem item, Brush brush, string caption, bool draw, ref int y, ref float width)
        {
            // draw the image
            if (draw)
                g.DrawImageUnscaled(item.Image, LAYOUT_PADDING * 2, y);

            // skill bar
            if (draw && item.Labor.HasSkill)
            {
                var brush_pip = item.SkillInfo.Level >= GameData.SKILL_MAX ? Brushes.Gold : brush;
                var rect_background = new Rectangle(
                    LAYOUT_PADDING * 3 + item.Image.Width,
                    y + (SKILL_BAR_HEIGHT - SKILL_BAR_HEIGHT_INNER) / 2,
                    SKILL_BAR_WIDTH,
                    SKILL_BAR_HEIGHT_INNER);

                // draw background
                g.FillRectangle(SystemBrushes.ControlLightLight, rect_background);
                g.DrawRectangle(SystemPens.ControlDarkDark, rect_background.Left, rect_background.Top, rect_background.Width + 1, rect_background.Height + 1);

                // draw the level pips
                for (int i = 1; i <= item.SkillInfo.Level && i <= GameData.SKILL_MAX; ++i)
                {
                    g.FillRectangle(
                        brush_pip,
                        rect_background.Left + SKILL_BAR_WIDTH - SKILL_BAR_BORDER - (SKILL_BAR_SEGMENT + SKILL_BAR_PADDING) * i + SKILL_BAR_PADDING,
                        rect_background.Top + SKILL_BAR_BORDER + SKILL_BAR_PADDING + 1,
                        SKILL_BAR_SEGMENT,
                        SKILL_BAR_SEGMENT);
                }
            }

            // draw the caption
            var measured = g.MeasureString(caption, this.Font);
            var rect_caption = item.Labor.HasSkill
                ? new Rectangle(LAYOUT_PADDING * 4 + SKILL_BAR_WIDTH + item.Image.Width, (int)y, (int)Math.Ceiling(measured.Width), SKILL_BAR_HEIGHT)
                : new Rectangle(LAYOUT_PADDING * 3 + item.Image.Width, (int)y, (int)Math.Ceiling(measured.Width), SKILL_BAR_HEIGHT);
            if (draw)
                g.DrawString(caption, this.Font, brush, rect_caption, m_format_left_center);
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
