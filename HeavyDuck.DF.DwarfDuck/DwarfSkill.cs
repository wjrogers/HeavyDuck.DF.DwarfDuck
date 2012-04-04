using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeavyDuck.DF.DwarfDuck
{
    internal class DwarfSkill : IEquatable<DwarfSkill>
    {
        public const string CLASS_NORMAL = "Normal";

        private readonly dfproto.JobSkillAttr m_attr;
        private readonly Lazy<DwarfProfession> m_profession;
        private readonly Lazy<DwarfLabor> m_labor;

        public DwarfSkill(dfproto.JobSkillAttr attr)
        {
            m_attr = attr;
            m_profession = new Lazy<DwarfProfession>(() => GameData.GetProfession(m_attr.Profession));
            m_labor = new Lazy<DwarfLabor>(() => GameData.GetLabor(m_attr.Labor));
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

        public string CaptionNoun
        {
            get { return m_attr.CaptionNoun; }
        }

        public string Class
        {
            get { return m_attr.Type; }
        }

        public string Category
        {
            get
            {
                if (this.Class == CLASS_NORMAL && this.HasProfession)
                    return this.Profession.Category;
                else if (this.Class == CLASS_NORMAL)
                    return GameData.CATEGORY_OTHER;
                else
                    return this.Class;
            }
        }

        public DwarfLabor Labor
        {
            get { return m_labor.Value; }
        }

        public bool HasLabor
        {
            get { return m_labor.Value.ID != GameData.NONE; }
        }

        public DwarfProfession Profession
        {
            get { return m_profession.Value; }
        }

        public bool HasProfession
        {
            get { return m_profession.Value.ID != GameData.NONE; }
        }

        public bool Equals(DwarfSkill other)
        {
            return other != null
                && other.ID == this.ID;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DwarfSkill);
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
