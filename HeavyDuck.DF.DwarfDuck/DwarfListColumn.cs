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
        private const int SKILL_WIDTH = 9;

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

                int y = rect_paint.Top + rect_paint.Height / 2 - item.Image.Height / 2;

                if (1f <= item.SkillPercent)
                {
                    graphics.DrawImageUnscaled(item.Image, x, y);
                }
                else
                {
                    var rect_skill = new Rectangle(
                        x + item.Image.Width - SKILL_WIDTH,
                        y + item.Image.Height - SKILL_WIDTH,
                        SKILL_WIDTH,
                        SKILL_WIDTH);

                    graphics.DrawImageUnscaled(item.Image, x, y);
                    graphics.FillEllipse(Brushes.White, rect_skill);
                    graphics.FillPie( Brushes.Blue, rect_skill, 0, Math.Min(360, 360 * item.SkillPercent));
                }

                x += item.Image.Width + ITEM_PADDING;
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
        public DwarfListItem(Image image, Dwarf dwarf, DwarfLabor labor, DwarfListMode mode)
        {
            this.Image = image;
            this.Dwarf = dwarf;
            this.Labor = labor;
            this.Mode = mode;
            this.SkillInfo = dwarf.GetSkillInfo(labor);
        }

        public Image Image { get; private set; }
        public Dwarf Dwarf { get; private set; }
        public DwarfLabor Labor { get; private set; }
        public DwarfListMode Mode { get; private set; }
        public dfproto.SkillInfo SkillInfo { get; private set; }

        public string Caption
        {
            get { return this.Mode == DwarfListMode.Labor ? Labor.Caption : Dwarf.Name; }
        }

        public float SkillPercent
        {
            get { return this.SkillInfo.Level / GameData.SKILL_MAX; }
        }
    }
}
