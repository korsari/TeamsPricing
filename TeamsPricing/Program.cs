﻿using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using FileHelpers;

namespace TeamsPricing
{
    class Program
    {
        static void Main(string[] args)
        {
            Tuple<string, string>[] selectionRules = GetRules(@"C:\Git\TeamsPricing\Rules.csv");
            Dictionary<string, Dictionary<string, string>> selectionRulesTable = GetRulesTable(@"C:\Git\TeamsPricing\RulesTable.csv");
            Dictionary<string, TeamFootball[]> groups = GetGroups(@"C:\Git\TeamsPricing\Teams.csv");
            int[] prices = { 1000, 500, 250, 250, 125, 125, 125, 125, 50, 50, 50, 50, 50, 50, 50, 50, 25, 25, 25, 25, 25, 25, 25, 25 };
            int nIterations = 1000000;
            string goaslMethod = "QuickWeighted";

            Dictionary<string, float> mcFinalPrices = groups.SelectMany(x => x.Value).ToDictionary(x => x.Name, x => 0f);

            Competition euro2016 = new Competition(groups, selectionRules, selectionRulesTable, goaslMethod);
            TeamFootball[] result;
            for (int iteration = 0; iteration < nIterations; iteration++)
            {
                result = euro2016.PlayCompetition();
                for (int j = 0; j < result.Length; j++)
                {
                    mcFinalPrices[result[j].Name] += prices[j];
                }
                if (iteration % (nIterations / 10) == 0)
                {
                    Console.WriteLine(iteration / (nIterations / 100) + "%");
                }
                euro2016.Reset();
            }

            //pq tu fais des toList() a tout bout de champs !!! c'est couteu en temps et en memoire
            //List<KeyValuePair<string,float>> outputFinal = mcFinalPrices.ToList();

            // essaye d'utiliser var dans tes foreach.
            // ca t'evite a rechanger tout si tu changes tonn type de collection
            foreach (var team in mcFinalPrices.OrderByDescending(x => x.Value))
            {
                Console.WriteLine("{0} = {1}$", team.Key, team.Value / nIterations);
            }
            Console.ReadLine();
        }

        public static Tuple<string, string>[] GetRules(string filePath)
        {
            var engine = new FileHelperEngine<Rules>();
            var records = engine.ReadFile(filePath);
            Tuple<string, string>[] result = new Tuple<string, string>[records.Length];
            for (int i = 0; i < records.Length; i++)
            {
                // ca te fait economiser un acces memoire !
                var record = records[i];
                result[i] = new Tuple<string, string>(record.Team1, record.Team2);
            }
            return result;
        }

        public static Dictionary<string, TeamFootball[]> GetGroups(string filePath)
        {
            var engine = new FileHelperEngine<Teams>();
            var records = engine.ReadFile(filePath);
            // redondant ta declaration
            var resultList = new Dictionary<string, List<TeamFootball>>();
            foreach (Teams team in records)
            {
                List<TeamFootball> groupTeams;
                if (!resultList.TryGetValue(team.Group, out groupTeams))
                {
                    groupTeams = new List<TeamFootball>();
                }
                groupTeams.Add(new TeamFootball(team.Name, team.Points));
            }

            // je comprends pas pq tu passes tout ca en array...
            // je te laisse corriger !!!
            var result = new Dictionary<string, TeamFootball[]>();
            foreach (var res in resultList)
            {
                result.Add(res.Key, res.Value.ToArray());
            }
            return result;
        }

        public static Dictionary<string, Dictionary<string, string>> GetRulesTable(string filePath)
        {
            var engine = new FileHelperEngine<RulesTable>();
            var records = engine.ReadFile(filePath);

            // c'est pas du detail, ca t'evite de resizer ton dico !!!
            var result = records.ToDictionary(x => x.Key, x => new Dictionary<string, string> {
                    { "1A", x.Team1 },
                    { "1B", x.Team2 },
                    { "1C", x.Team3 },
                    { "1D", x.Team4 }
                });

            return result;
        }

        public static void ReworkTeamPoints(Dictionary<string, TeamFootball[]> groups, string method)
        {
            List<float> pointsList = new List<float>();
            // je te laisse corriger ca avec du linq comme plus haut
            foreach (KeyValuePair<string, TeamFootball[]> group in groups)
            {
                foreach (TeamFootball team in group.Value)
                {
                    pointsList.Add(team.Points);
                }
            }
            switch (method)
            {
                // dirty dirty ton case sur des strings
                // utilise une enum !
                case "stupid":
                    Tuple<double, double> meanVar = Statistics.MeanVariance(pointsList);
                    foreach (KeyValuePair<string, TeamFootball[]> group in groups)
                    {
                        foreach (TeamFootball team in group.Value)
                        {
                            // harmonise moi tout ca
                            // soit tu utilises des doubles soit des float !
                            team.Points = (team.Points - (float)meanVar.Item1) / (float)meanVar.Item2;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    [DelimitedRecord(",")]
    // precise toujours ce qu'est ta classe
    // Et faudrait mieux que tu bosses avec des Ids (short)
    // plus robuste aux changement, aux ameliorations et moins lourd en mémoire !
    public class Rules
    {
        // property => Maj
        public string Team1;
        public string Team2;
    }

    [DelimitedRecord(",")]
    public class RulesTable
    {
        public string Key;
        public string Team1;
        public string Team2;
        public string Team3;
        public string Team4;
    }

    [DelimitedRecord(",")]
    public class Teams
    {
        public string Group;
        public string Name;
        public float Points;
    }

}
