using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeavyDuck.DF.DwarfDuck
{
    internal class LaborRecord
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<DwarfListItem> AssignedUnits { get; set; }
        public List<DwarfListItem> SkilledUnits { get; set; }
    }
}
