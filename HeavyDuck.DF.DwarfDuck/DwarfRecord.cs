using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dfproto;

namespace HeavyDuck.DF.DwarfDuck
{
    internal class DwarfRecord
    {
        private readonly List<DwarfListItem> m_assigned_labors;
        private readonly List<DwarfListItem> m_other_skills;
        private readonly ProfessionAttr m_profession;

        public DwarfRecord(BasicUnitInfo unit)
        {
            this.UnitID = unit.UnitId;
            this.Name = unit.Name;
            this.GenderID = unit.Gender;
            this.Skills = unit.SkillsList;
            this.Labors = unit.LaborsList;

            m_assigned_labors = this.Labors
                .Select(l => GetListItem(GameData.GetLabor(l)))
                .ToList();
            m_other_skills = this.Skills
                .Select(s => GetListItem(s))
                .Where(i => !m_assigned_labors.Any(j => j.SkillID == i.SkillID))
                .ToList();
            m_profession = GameData.GetProfession(unit.Profession);
        }

        public int UnitID { get; private set; }
        public NameInfo Name { get; private set; }
        public int GenderID { get; private set; }
        public IList<SkillInfo> Skills { get; private set; }
        public IList<int> Labors { get; private set; }

        public ProfessionAttr Profession
        {
            get { return m_profession; }
        }

        public List<DwarfListItem> AssignedLabors
        {
            get { return m_assigned_labors; }
        }

        public List<DwarfListItem> OtherSkills
        {
            get { return m_other_skills; }
        }

        public DwarfListItem GetListItem(UnitLaborAttr labor)
        {
            SkillInfo unit_skill = null;
            var skill = GameData.GetSkillForLabor(labor);

            if (skill != null)
                unit_skill = this.Skills.FirstOrDefault(s => s.Id == skill.Id);

            return GetListItem(unit_skill, skill);
        }

        public DwarfListItem GetListItem(SkillInfo unit_skill)
        {
            return GetListItem(unit_skill, GameData.GetSkill(unit_skill.Id));
        }

        public DwarfListItem GetListItem(SkillInfo unit_skill, JobSkillAttr skill)
        {
            if (skill == null)
                return GetListItem(unit_skill, skill, GameData.GetProfessionDefault());
            else
                return GetListItem(unit_skill, skill, GameData.GetProfession(skill.Profession));
        }

        public DwarfListItem GetListItem(SkillInfo unit_skill, JobSkillAttr skill, ProfessionAttr profession)
        {
            if (skill == null)
                skill = GameData.GetSkillDefault();

            return new DwarfListItem
            {
                UnitID = this.UnitID,
                SkillID = skill.Id,
                SkillLevel = (unit_skill ?? SkillInfo.DefaultInstance).Level,
                Image = DwarfGraphics.GetImage(profession),
            };
        }

        public int GetSkillLevel(UnitLaborAttr labor)
        {
            // find the skill for this labor
            var skill = GameData.GetSkillForLabor(labor);
            if (skill == null) return 0;

            // find this unit's level in the skill
            var unit_skill = this.Skills.FirstOrDefault(s => s.Id == skill.Id);
            if (unit_skill == null) return 0;

            // return it
            return unit_skill.Level;
        }
    }
}
