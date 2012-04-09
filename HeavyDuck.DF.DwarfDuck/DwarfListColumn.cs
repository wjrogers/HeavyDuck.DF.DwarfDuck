using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HeavyDuck.DF.DwarfDuck
{
    internal class DwarfListColumn : DataGridViewColumn
    {
        public DwarfListColumn()
            : base(new DwarfListCell())
        {
            this.DefaultCellStyle.Padding = new Padding(3, 0, 3, 0);
        }
    }

    internal class DwarfListCell : DataGridViewCell
    {
        private const int ITEM_WIDTH = 18 + (ITEM_PADDING + PIP_SIZE) * 3 + ITEM_PADDING;
        private const int ITEM_PADDING = 1;
        private const int PIP_SIZE = 2;

        private static readonly ThreadLocal<Dictionary<Color, Brush>> m_brush_cache
            = new ThreadLocal<Dictionary<Color, Brush>>(() => new Dictionary<Color, Brush>());

        private static Brush GetCachedBrush(Color color)
        {
            Brush brush;
            var cache = m_brush_cache.Value;

            if (!cache.TryGetValue(color, out brush))
            {
                brush = new SolidBrush(color);
                cache[color] = brush;
            }

            return brush;
        }

        public override Type FormattedValueType
        {
            get { return typeof(IList<DwarfListItem>); }
        }

        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            // don't calculate anything when we don't belong to a grid
            if (this.DataGridView == null) return new Size(-1, -1);

            // check if we have the kind of value we're expecting
            var value = this.GetValue(rowIndex) as IList<DwarfListItem>;
            if (value == null)
                return new Size(-1, -1);

            // measure how much space we need
            return new Size(cellStyle.Padding.Horizontal + value.Count * (ITEM_WIDTH + ITEM_PADDING), -1);
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            Rectangle rect_borders;
            Rectangle rect_paint;
            int x;

            // draw standard border stuffs
            base.PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);

            // measure where we will paint
            rect_borders = BorderWidths(advancedBorderStyle);
            rect_paint = cellBounds;
            rect_paint.Offset(rect_borders.X, rect_borders.Y);
            rect_paint.Width -= rect_borders.Right;
            rect_paint.Height -= rect_borders.Bottom;
            x = rect_paint.Left + cellStyle.Padding.Left;

            // fill the background
            if ((elementState & DataGridViewElementStates.Selected) != DataGridViewElementStates.None)
                graphics.FillRectangle(GetCachedBrush(cellStyle.SelectionBackColor), rect_paint);
            else
                graphics.FillRectangle(GetCachedBrush(cellStyle.BackColor), rect_paint);

            // is our value a list of units?
            var list = value as IEnumerable<DwarfListItem>;
            if (list == null)
                return;

            // draw our list of dwarfs
            foreach (var item in list)
            {
                if (item.Image == null) continue;

                // center vertically in the cell I guess
                int y = rect_paint.Top + rect_paint.Height / 2 - item.Image.Height / 2;

                // draw the image
                graphics.DrawImageUnscaled(item.Image, x, y);
                x += item.Image.Width + ITEM_PADDING;

                // draw skill pips, column 1
                for (int i = 1; i <= item.SkillInfo.Level && i <= 6; ++i)
                    graphics.FillRectangle(Brushes.Black, x, y + item.Image.Height - i * (PIP_SIZE + ITEM_PADDING) + 1, PIP_SIZE, PIP_SIZE);
                x += PIP_SIZE + ITEM_PADDING;

                // draw skill pips, column 2
                for (int i = 7; i <= item.SkillInfo.Level && i <= 12; ++i)
                    graphics.FillRectangle(Brushes.Black, x, y + item.Image.Height - (i - 6) * (PIP_SIZE + ITEM_PADDING) + 1, PIP_SIZE, PIP_SIZE);
                x += PIP_SIZE + ITEM_PADDING;

                // draw skill pips, column 3
                for (int i = 13; i <= item.SkillInfo.Level && i <= 18; ++i)
                    graphics.FillRectangle(Brushes.Black, x, y + item.Image.Height - (i - 12) * (PIP_SIZE + ITEM_PADDING) + 1, PIP_SIZE, PIP_SIZE);
                x += PIP_SIZE + ITEM_PADDING;

                // one more pixel of spacing
                x += ITEM_PADDING;
            }
        }

        public DwarfListItem DwarfHitTest(DataGridViewCellMouseEventArgs e)
        {
            var list = GetValue(e.RowIndex) as IList<DwarfListItem>;
            if (list == null) return null;
            var style = this.InheritedStyle;

            int index = (e.X - style.Padding.Left) / (ITEM_WIDTH + ITEM_PADDING);
            if (index < 0 || list.Count <= index)
                return null;
            else
                return list[index];
        }
    }

    internal enum DwarfListMode
    {
        Dwarf,
        Labor,
    }

    internal class DwarfListItem
    {
        public DwarfListItem(Image image, Dwarf dwarf, DwarfLabor labor)
        {
            this.Image = image;
            this.Dwarf = dwarf;
            this.Labor = labor;
            this.SkillInfo = dwarf.GetSkillInfo(labor);
        }

        public Image Image { get; private set; }
        public Dwarf Dwarf { get; private set; }
        public DwarfLabor Labor { get; private set; }
        public dfproto.SkillInfo SkillInfo { get; private set; }

        public float SkillPercent
        {
            get { return this.SkillInfo.Level / GameData.SKILL_MAX; }
        }

        public string GetCaption(DwarfListMode mode)
        {
            switch (mode)
            {
                case DwarfListMode.Dwarf:
                    return this.Dwarf.Name;
                case DwarfListMode.Labor:
                    return this.Labor.Caption;
                default:
                    return "?";
            }
        }
    }
}
