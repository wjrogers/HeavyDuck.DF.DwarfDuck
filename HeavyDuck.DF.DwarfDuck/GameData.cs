using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dfproto;
using HeavyDuck.DF.DFHack;
using System.Drawing;

namespace HeavyDuck.DF.DwarfDuck
{
    internal static class GameData
    {
        public const int NONE = -1;
        public const int SKILL_LEGENDARY = 15;
        public const int SKILL_MAX = 20;
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
                .ToLookup(o => o.SkillPair.Key.Labor, o => o.Dwarf);

            foreach (var labor in m_labors.Values)
            {
                labor.UnitsAssigned.Clear();
                labor.UnitsAssigned.AddRange(lookup_assigned[labor].Select(d => new DwarfListItem(d.Image, d, labor)));

                labor.UnitsPotential.Clear();
                labor.UnitsPotential.AddRange(lookup_skilled[labor].Select(d => new DwarfListItem(d.Image, d, labor)));
            }
        }

        public static IEnumerable<DwarfLabor> GetLabors()
        {
            foreach (var group in m_labors.GroupBy(p => p.Value.Category).OrderBy(g => GetCategoryOrder(g.Key)))
            {
                yield return new DwarfLaborHeader(group.Key);

                foreach (var p in group.OrderBy(p => p.Value.Caption))
                {
                    if (p.Key != NONE)
                        yield return p.Value;
                }
            }
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

        public static Tuple<Color, Color> GetCategoryColors(string category)
        {
            switch (category)
            {
                case "Other":
                    return Tuple.Create(Color.Black, Color.LightGray);
                case "Miner":
                    return Tuple.Create(Color.White, Color.Gray);
                case "Hauling":
                    return Tuple.Create(Color.Black, Color.DarkGray);
                case "Woodworker":
                    return Tuple.Create(Color.White, Color.DarkGoldenrod);
                case "Stoneworker":
                    return Tuple.Create(Color.White, Color.Black);
                case "Administrator":
                    return Tuple.Create(Color.White, Color.Purple);
                case "Ranger":
                    return Tuple.Create(Color.White, Color.ForestGreen);
                case "Medical":
                    return Tuple.Create(Color.White, Color.DeepPink);
                case "Farmer":
                    return Tuple.Create(Color.White, Color.Brown);
                case "Craftsman":
                    return Tuple.Create(Color.White, Color.CornflowerBlue);
                case "Alchemist":
                    return Tuple.Create(Color.White, Color.DarkOrchid);
                case "Fishery Worker":
                    return Tuple.Create(Color.White, Color.Navy);
                case "Metalsmith":
                    return Tuple.Create(Color.White, Color.DarkSlateGray);
                case "Jeweler":
                    return Tuple.Create(Color.White, Color.DarkGreen);
                case "Engineer":
                    return Tuple.Create(Color.White, Color.Firebrick);
                default:
                    return Tuple.Create(Color.Black, Color.LightGray);
            }
        }

        public static int GetCategoryOrder(string category)
        {
            switch (category)
            {
                case "Other":
                    return int.MaxValue;
                case "Miner":
                    return 0;
                case "Hauling":
                    return int.MaxValue - 2;
                case "Woodworker":
                    return 1;
                case "Stoneworker":
                    return 2;
                case "Administrator":
                    return 3;
                case "Ranger":
                    return 4;
                case "Medical":
                    return 5;
                case "Farmer":
                    return 6;
                case "Craftsman":
                    return 7;
                case "Alchemist":
                    return 12;
                case "Fishery Worker":
                    return 8;
                case "Metalsmith":
                    return 9;
                case "Jeweler":
                    return 10;
                case "Engineer":
                    return 11;
                default:
                    return int.MaxValue - 1;
            }
        }
    }
}
