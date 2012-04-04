using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace HeavyDuck.DF.DwarfDuck
{
    internal class DwarfListColumn : DataGridViewColumn
    {
        public DwarfListColumn() : base(new DwarfListCell()) { }
    }

    internal class DwarfListCell : DataGridViewTextBoxCell
    {
        private const int ITEM_WIDTH = 18;
        private const int ITEM_PADDING = 1;

        public override Type FormattedValueType
        {
            get { return typeof(IEnumerable<DwarfListItem>); }
        }

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, System.ComponentModel.TypeConverter valueTypeConverter, System.ComponentModel.TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            return value as IEnumerable<DwarfListItem>;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            int x = cellBounds.Left + ITEM_PADDING;

            // draw standard border stuffs
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, null, null, errorText, cellStyle, advancedBorderStyle, paintParts);

            // is our value a list of units?
            var list = value as IEnumerable<DwarfListItem>;
            if (list == null)
                return;

            // draw our list of dwarfs
            foreach (var dwarf in list)
            {
                if (dwarf.Image == null) continue;

                int y = cellBounds.Top +  cellBounds.Height / 2 - dwarf.Image.Height / 2;

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
