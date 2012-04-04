using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeavyDuck.DF.DwarfDuck
{
    internal class DwarfLabor : IEquatable<DwarfLabor>
    {
        private readonly dfproto.UnitLaborAttr m_attr;
        private readonly Lazy<DwarfSkill> m_skill;

        public DwarfLabor(dfproto.UnitLaborAttr attr)
        {
            m_attr = attr;
            m_skill = new Lazy<DwarfSkill>(() => GameData.GetSkillForLabor(m_attr.Id));

            this.UnitsAssigned = new List<DwarfListItem>();
            this.UnitsPotential = new List<DwarfListItem>();
        }

        public int ID
        {
            get { return m_attr.Id; }
        }

        public string Key
        {
            get { return m_attr.Key; }
        }

        public string Caption
        {
            get { return m_attr.Caption; }
        }

        public string Category
        {
            get
            {
                if (this.HasSkill)
                    return this.Skill.Category;
                else if (m_attr.Key.StartsWith("HAUL_"))
                    return GameData.CATEGORY_HAULING;
                else
                    return GameData.CATEGORY_OTHER;
            }
        }

        public DwarfSkill Skill
        {
            get { return m_skill.Value; }
        }

        public bool HasSkill
        {
            get { return m_skill.Value.ID != GameData.NONE; }
        }

        public List<DwarfListItem> UnitsAssigned { get; private set; }
        public List<DwarfListItem> UnitsPotential { get; private set; }

        public bool Equals(DwarfLabor other)
        {
            return other != null
                && other.ID == this.ID;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DwarfLabor);
        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        public override string ToString()
        {
            return this.Caption;
        }
    }
}
