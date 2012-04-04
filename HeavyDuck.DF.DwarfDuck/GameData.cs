using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dfproto;
using HeavyDuck.DF.DFHack;

namespace HeavyDuck.DF.DwarfDuck
{
    internal static class GameData
    {
        public const int NONE = -1;

        private static DFHackReply<GetWorldInfoOut> m_world;
        private static DFHackReply<ListEnumsOut> m_enums;

        private static Dictionary<int, UnitLaborAttr> m_labors;
        private static Dictionary<int, ProfessionAttr> m_professions;
        private static Dictionary<int, JobSkillAttr> m_skills;

        public static void Initialize()
        {
            DFHackReply<ListJobSkillsOut> job_skills;

            using (var client = new DFHackClient())
            {
                client.Open();

                m_world = client.GetWorldInfo();
                m_enums = client.ListEnums();

                job_skills = client.ListJobSkills();
            }

            m_labors = job_skills.Data.LaborList.ToDictionary(l => l.Id);
            m_professions = job_skills.Data.ProfessionList.ToDictionary(p => p.Id);
            m_skills = job_skills.Data.SkillList.ToDictionary(s => s.Id);
        }

        public static GetWorldInfoOut World
        {
            get { return m_world.Data; }
        }

        public static IEnumerable<LaborRecord> GetLabors(IEnumerable<DwarfRecord> dwarves)
        {
            var lookup_assigned = dwarves
                .SelectMany(d => d.Labors.Select(l => new { Dwarf = d, Labor = l }))
                .ToLookup(r => r.Labor, r => r.Dwarf);
            var lookup_skilled = dwarves
                .SelectMany(d => d.Skills.Select(s => new { Dwarf = d, UnitSkill = s, SkillInfo = GameData.GetSkill(s.Id) }))
                .Where(o => o.UnitSkill.Level > 0 && !o.Dwarf.Labors.Contains(o.SkillInfo.Labor))
                .ToLookup(o => o.SkillInfo.Labor);

            foreach (var labor in m_labors.Values)
            {
                yield return new LaborRecord
                {
                    ID = labor.Id,
                    Name = labor.Caption,
                    AssignedUnits = lookup_assigned[labor.Id].Select(d => new DwarfListItem
                    {
                        UnitID = d.UnitID,
                        SkillLevel = d.GetSkillLevel(labor),
                        Image = DwarfGraphics.GetImage(d.Profession.Key),
                    }).ToList(),
                    SkilledUnits = lookup_skilled[labor.Id].Select(o => new DwarfListItem
                    {
                        UnitID = o.Dwarf.UnitID,
                        SkillID = o.SkillInfo.Id,
                        SkillLevel = o.UnitSkill.Level,
                        Image = DwarfGraphics.GetImage(o.Dwarf.Profession.Key),
                    }).ToList(),
                };
            }
        }

        public static UnitLaborAttr GetLabor(int id)
        {
            UnitLaborAttr l;

            if (m_labors.TryGetValue(id, out l))
                return l;
            else
                return m_labors[-1];
        }

        public static ProfessionAttr GetProfessionDefault()
        {
            return GetProfession(NONE);
        }

        public static ProfessionAttr GetProfession(int id)
        {
            ProfessionAttr p;

            if (m_professions.TryGetValue(id, out p))
                return p;
            else
                return m_professions[-1];
        }

        public static JobSkillAttr GetSkillDefault()
        {
            return GetSkill(NONE);
        }

        public static JobSkillAttr GetSkill(int id)
        {
            JobSkillAttr s;

            if (m_skills.TryGetValue(id, out s))
                return s;
            else
                return m_skills[-1];
        }

        public static JobSkillAttr GetSkillForLabor(UnitLaborAttr labor)
        {
            return GetSkillForLabor(labor.Id);
        }

        public static JobSkillAttr GetSkillForLabor(int labor)
        {
            return m_skills.Values.FirstOrDefault(s => s.Labor == labor);
        }
    }
}
