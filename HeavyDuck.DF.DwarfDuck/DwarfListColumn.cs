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
        private const int ITEM_BORDER = 1;
        private const int ITEM_PADDING = 3;

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
            return new Size(cellStyle.Padding.Horizontal + value.Sum(v => v.SkillInfo.Level + ITEM_PADDING), -1);
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

            var pen_border = new Pen(Color.FromArgb(128, Color.White), 1);

            // draw our list of dwarfs
            foreach (var item in list)
            {
                // center vertically in the cell I guess
                int height = rect_paint.Height - 3;
                int width = item.SkillInfo.Level + ITEM_BORDER * 2;
                int top = rect_paint.Top + 1;

                // draw the bar
                graphics.FillRectangle(Brushes.Navy, x, top, width, height);
                graphics.DrawRectangle(pen_border, x, top, width - 1, height);
                x += width + ITEM_PADDING;
            }
        }

        public DwarfListItem DwarfHitTest(DataGridViewCellMouseEventArgs e)
        {
            var list = GetValue(e.RowIndex) as IList<DwarfListItem>;
            if (list == null) return null;
            var style = this.InheritedStyle;

            // TODO: FIX
            int index = (e.X - style.Padding.Left) / (3 + ITEM_PADDING);
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
