using System;
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
            Tuple<string, string>[] selectionRules = GetRules("D:\\C#\\FootballBets\\Rules.csv");
            Dictionary<string, Dictionary<string, string>> selectionRulesTable = GetRulesTable("D:\\C#\\FootballBets\\RulesTable.csv");
            Dictionary <string, TeamFootball[]> groups = GetGroups("D:\\C#\\FootballBets\\Teams.csv");
            int[] prices = {1000,500,250,250,125,125,125,125,50,50,50,50,50,50,50,50,25,25,25,25,25,25,25,25 };
            int nIterations = 1000000;
            MethodToUse goaslMethod = MethodToUse.quickWeighted;

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

            foreach (var team in mcFinalPrices.OrderByDescending(x => x.Value))
            {
                Console.WriteLine("{0} = {1}$", team.Key, team.Value/nIterations);
                //Console.WriteLine(team.Value/nIterations);
            }
            Console.ReadLine();
        }

        public static Tuple<string, string>[] GetRules(string filePath)
        {
            var engine = new FileHelperEngine<Rules>();
            var records = engine.ReadFile(filePath);
            Tuple<string, string>[] result = new Tuple<string, string>[records.Length];
            for (int i = 0; i < records.Length; i++)
                result[i] = new Tuple<string, string>(records[i].Team1, records[i].Team2);
            return result;
        }

        public static Dictionary<string, TeamFootball[]> GetGroups(string filePath)
        {
            var engine = new FileHelperEngine<Teams>();
            var records = engine.ReadFile(filePath);
            var resultList = new Dictionary<string, List<TeamFootball>>();
            foreach (Teams team in records)
            {
                if (!resultList.ContainsKey(team.Group))
                {
                    resultList.Add(team.Group, new List<TeamFootball>());
                }
                resultList[team.Group].Add(new TeamFootball(team.Name, team.Points));
            }
            var result= new Dictionary<string, TeamFootball[]>();
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
            var result = records.ToDictionary(x => x.Key, x => new Dictionary<string, string> {
                    { "1A", x.Team1 },
                    { "1B", x.Team2 },
                    { "1C", x.Team3 },
                    { "1D", x.Team4 }
                });
            return result;
        }

        public static void ReworkTeamPoints(Dictionary<string, TeamFootball[]> groups, MethodToUse method)
        {
            List<float> pointsList = new List<float>();
            foreach (TeamFootball team in groups.SelectMany(x => x.Value))
            {
                pointsList.Add(team.Points);
            }
            switch (method)
            {
                case MethodToUse.stupid:
                    Tuple<double, double> meanVar = Statistics.MeanVariance(pointsList);
                    foreach (TeamFootball team in groups.SelectMany(x => x.Value))
                    {
                        team.Points = (team.Points - (float)meanVar.Item1) / (float)meanVar.Item2;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    [DelimitedRecord(",")]
    public class Rules
    {
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
