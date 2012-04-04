using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace HeavyDuck.DF.DwarfDuck
{
    internal class Dwarf
    {
        private readonly dfproto.BasicUnitInfo m_unit;
        private readonly HashSet<DwarfLabor> m_labors;
        private readonly Dictionary<DwarfSkill, dfproto.SkillInfo> m_skills;
        private readonly Lazy<DwarfProfession> m_profession;
        private readonly List<DwarfListItem> m_labors_view;
        private readonly List<DwarfListItem> m_labors_potential_view;
        private readonly int m_menial_count;

        public Dwarf(dfproto.BasicUnitInfo unit)
        {
            m_unit = unit;
            m_labors = new HashSet<DwarfLabor>(unit.LaborsList.Select(id => GameData.GetLabor(id)));
            m_skills = unit.SkillsList.ToDictionary(s => GameData.GetSkill(s.Id));
            m_profession = new Lazy<DwarfProfession>(() => GameData.GetProfession(m_unit.Profession));

            m_menial_count = m_labors.Count(l => !l.HasSkill);

            m_labors_view = m_labors.Where(l => l.HasSkill).Select(l => new DwarfListItem
            {
                UnitID = unit.UnitId,
                SkillID = l.Skill.ID,
                SkillLevel = GetSkillLevel(l),
                Image = l.Skill.Profession.Image,
            }).ToList();
            m_labors_potential_view = m_skills.Where(p => !m_labors.Contains(p.Key.Labor)).Select(p => new DwarfListItem
            {
                UnitID = unit.UnitId,
                SkillID = p.Key.ID,
                SkillLevel = p.Value.Level,
                Image = p.Key.Profession.Image,
            }).ToList();
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

        public int MenialLaborCount
        {
            get { return m_menial_count; }
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

        public int GetSkillLevel(DwarfLabor labor)
        {
            dfproto.SkillInfo info;

            if (m_skills.TryGetValue(labor.Skill, out info))
                return info.Level;
            else
                return 0;
        }
    }
}
