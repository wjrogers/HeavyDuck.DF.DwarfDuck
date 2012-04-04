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
        private const int ITEM_WIDTH = 18;
        private const int ITEM_PADDING = 1;

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
            foreach (var dwarf in list)
            {
                if (dwarf.Image == null) continue;

                int y = rect_paint.Top + rect_paint.Height / 2 - dwarf.Image.Height / 2;

                if (1f <= dwarf.SkillPercent)
                {
                    graphics.DrawImageUnscaled(dwarf.Image, x, y);
                }
                else
                {
                    int y_filled = (int)Math.Min(dwarf.Image.Height, Math.Round(dwarf.Image.Height * dwarf.SkillPercent));
                    var dest = new Rectangle(x, y + dwarf.Image.Height - y_filled, dwarf.Image.Width, y_filled);
                    var src = new Rectangle(0, dwarf.Image.Height - y_filled, dwarf.Image.Width, y_filled);

                    graphics.DrawImageUnscaled(dwarf.Image, x, y);
                }

                x += dwarf.Image.Width + ITEM_PADDING;
            }
        }
    }

    internal class DwarfListItem
    {
        private const float SKILL_MAX = 18f;

        public int UnitID { get; set; }
        public int SkillID { get; set; }
        public int SkillLevel { get; set; }
        public Image Image { get; set; }

        public float SkillPercent
        {
            get { return this.SkillLevel / SKILL_MAX; }
        }
    }
}
