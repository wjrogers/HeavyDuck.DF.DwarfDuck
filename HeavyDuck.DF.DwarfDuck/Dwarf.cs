using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace HeavyDuck.DF.DwarfDuck
{
    internal class Dwarf
    {
        private const int MISC_TRAIT_MIGRANT_COUNTER = 7;

        private readonly dfproto.BasicUnitInfo m_unit;
        private readonly HashSet<DwarfLabor> m_labors;
        private readonly Dictionary<DwarfSkill, dfproto.SkillInfo> m_skills;
        private readonly Lazy<DwarfProfession> m_profession;
        private readonly List<DwarfListItem> m_labors_view;
        private readonly List<DwarfListItem> m_labors_potential_view;
        private readonly int m_unskilled_count;
        private readonly int m_migrant_counter = 0;

        public Dwarf(dfproto.BasicUnitInfo unit)
        {
            m_unit = unit;
            m_labors = new HashSet<DwarfLabor>(unit.LaborsList.Select(id => GameData.GetLabor(id)));
            m_skills = unit.SkillsList.ToDictionary(s => GameData.GetSkill(s.Id));
            m_profession = new Lazy<DwarfProfession>(() => GameData.GetProfession(m_unit.Profession));

            m_unskilled_count = m_labors.Count(l => !l.HasSkill);

            // look for the migrant counter to identify new migrants
            var migrant_counter_trait = unit.MiscTraitsList.FirstOrDefault(t => t.Id == MISC_TRAIT_MIGRANT_COUNTER);
            if (migrant_counter_trait != null)
                m_migrant_counter = migrant_counter_trait.Value;

            m_labors_view = m_labors.Where(l => l.HasSkill).Select(l => new DwarfListItem(
                l.Skill.Profession.Image,
                this,
                l)).ToList();
            m_labors_potential_view = m_skills.Where(p => p.Key.HasLabor && !m_labors.Contains(p.Key.Labor)).Select(p => new DwarfListItem(
                p.Key.Profession.Image,
                this,
                p.Key.Labor)).ToList();
        }

        public int UnitID
        {
            get { return m_unit.UnitId; }
        }

        public string Name
        {
            get { return GameData.FormatName(m_unit.Name); }
        }

        public int Gender
        {
            get { return m_unit.Gender; }
        }

        public HashSet<DwarfLabor> Labors
        {
            get { return m_labors; }
        }

        public List<DwarfListItem> LaborsView
        {
            get { return m_labors_view; }
        }

        public List<DwarfListItem> LaborsPotentialView
        {
            get { return m_labors_potential_view; }
        }

        public int LaborsUnskilledCount
        {
            get { return m_unskilled_count; }
        }

        public Dictionary<DwarfSkill, dfproto.SkillInfo> Skills
        {
            get { return m_skills; }
        }

        public DwarfProfession Profession
        {
            get { return m_profession.Value; }
        }

        public Image Image
        {
            get { return this.Profession.Image; }
        }

        public List<DwarfListItem> AssignedLabors
        {
            get { return m_labors_view; }
        }

        public bool IsMigrant
        {
            get { return m_migrant_counter > 0; }
        }

        public dfproto.SkillInfo GetSkillInfo(DwarfLabor labor)
        {
            dfproto.SkillInfo info;

            if (m_skills.TryGetValue(labor.Skill, out info))
                return info;
            else
                return dfproto.SkillInfo.DefaultInstance;
        }
    }
}
