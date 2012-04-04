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
        public const string CATEGORY_HAULING = "Hauling";
        public const string CATEGORY_OTHER = "Other";

        private static DFHackReply<GetWorldInfoOut> m_world;
        private static DFHackReply<ListEnumsOut> m_enums;

        private static Dictionary<int, DwarfLabor> m_labors;
        private static Dictionary<int, DwarfProfession> m_professions;
        private static Dictionary<int, DwarfSkill> m_skills;
        private static Dictionary<int, DwarfSkill> m_skills_by_labor;

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

            m_labors = job_skills.Data.LaborList.ToDictionary(l => l.Id, l => new DwarfLabor(l));
            m_professions = job_skills.Data.ProfessionList.ToDictionary(p => p.Id, p => new DwarfProfession(p));
            m_skills = job_skills.Data.SkillList.ToDictionary(s => s.Id, s => new DwarfSkill(s));
            m_skills_by_labor = job_skills.Data.SkillList
                .Where(s => s.Labor != NONE)
                .ToDictionary(s => s.Labor, s => m_skills[s.Id]);
        }

        public static GetWorldInfoOut World
        {
            get { return m_world.Data; }
        }

        public static void UpdateLabors(IEnumerable<Dwarf> dwarves)
        {
            var lookup_assigned = dwarves
                .SelectMany(d => d.Labors.Select(l => new { Dwarf = d, Labor = l }))
                .ToLookup(r => r.Labor, r => r.Dwarf);
            var lookup_skilled = dwarves
                .SelectMany(d => d.Skills.Select(s => new { Dwarf = d, SkillPair = s }))
                .Where(o => o.SkillPair.Value.Level > 0 && !o.Dwarf.Labors.Contains(o.SkillPair.Key.Labor))
                .ToLookup(o => o.SkillPair.Key.Labor);

            foreach (var labor in m_labors.Values)
            {
                labor.UnitsAssigned.Clear();
                labor.UnitsAssigned.AddRange(lookup_assigned[labor].Select(d => new DwarfListItem
                {
                    UnitID = d.UnitID,
                    SkillID = labor.Skill.ID,
                    SkillLevel = d.GetSkillLevel(labor),
                    Image = d.Image,
                }));

                labor.UnitsPotential.Clear();
                labor.UnitsPotential.AddRange(lookup_skilled[labor].Select(o => new DwarfListItem
                {
                    UnitID = o.Dwarf.UnitID,
                    SkillID = labor.Skill.ID,
                    SkillLevel = o.SkillPair.Value.Level,
                    Image = o.Dwarf.Image,
                }));
            }
        }

        public static IEnumerable<DwarfLabor> GetLabors()
        {
            return m_labors.Values.OrderBy(l => Tuple.Create(l.Category, l.Caption));
        }

        public static DwarfLabor GetLaborDefault()
        {
            return m_labors[NONE];
        }

        public static DwarfLabor GetLabor(int id)
        {
            DwarfLabor l;

            if (m_labors.TryGetValue(id, out l))
                return l;
            else
                return GetLaborDefault();
        }

        public static DwarfProfession GetProfessionDefault()
        {
            return m_professions[NONE];
        }

        public static DwarfProfession GetProfession(int id)
        {
            DwarfProfession p;

            if (m_professions.TryGetValue(id, out p))
                return p;
            else
                return GetProfessionDefault();
        }

        public static DwarfSkill GetSkillDefault()
        {
            return m_skills[NONE];
        }

        public static DwarfSkill GetSkill(int id)
        {
            DwarfSkill s;

            if (m_skills.TryGetValue(id, out s))
                return s;
            else
                return GetSkillDefault();
        }

        public static DwarfSkill GetSkillForLabor(int id)
        {
            DwarfSkill s;

            if (m_skills_by_labor.TryGetValue(id, out s))
                return s;
            else
                return GetSkillDefault();
        }

        public static string FormatName(dfproto.NameInfo name)
        {
            return string.Format("{0} {1}", name.FirstName, name.LastName);
        }
    }
}
