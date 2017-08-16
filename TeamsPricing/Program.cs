using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
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
            Dictionary<string, float> MCFinalPrices = new Dictionary<string, float>();
            int nIterations = 100000;

            foreach( KeyValuePair<string, TeamFootball[]> group in groups)
            {
                foreach (TeamFootball team in group.Value)
                    MCFinalPrices.Add(team.Name, 0);
            }

            Competition Euro2016 = new Competition(groups, selectionRules, selectionRulesTable);
            TeamFootball[] result;
            for (int iteration = 0; iteration < nIterations; iteration++)
            {
                result = Euro2016.PlayCompetition();
                for (int j = 0; j < result.Length; j++)
                {
                    MCFinalPrices[result[j].Name] += prices[j];
                }
                if (iteration % (nIterations/10) == 0)
                    Console.WriteLine(iteration/(nIterations/100)+ "%");
                Euro2016.Reset();
            }

            foreach (KeyValuePair<string, float> team in MCFinalPrices.ToList())
                MCFinalPrices[team.Key] /= nIterations;

            List<KeyValuePair<string,float>> outputFinal = MCFinalPrices.ToList();
            outputFinal.Sort(
                delegate (KeyValuePair<string, float> team1, KeyValuePair<string, float> team2)
                {
                    return team1.Value.CompareTo(team2.Value);
                });
            outputFinal.Reverse();
            foreach (KeyValuePair<string, float> team in outputFinal)
                Console.WriteLine("{0} = {1}$", team.Key, team.Value);
                //Console.WriteLine(team.Value);
            Console.ReadLine();

        }

        public static Tuple<string, string>[] GetRules(string filePath)
        {
            var engine = new FileHelperEngine<Rules>();
            var records = engine.ReadFile(filePath);
            Tuple<string, string>[] result = new Tuple<string, string>[records.Length];
            for (int i = 0; i < records.Length; i++)
                result[i] = new Tuple<string, string>(records[i].team1, records[i].team2);
            return result;
        }

        public static Dictionary<string, TeamFootball[]> GetGroups(string filePath)
        {
            var engine = new FileHelperEngine<Teams>();
            var records = engine.ReadFile(filePath);
            Dictionary<string, List<TeamFootball>> resultList = new Dictionary<string, List<TeamFootball>>();
            foreach (Teams team in records)
            {
                if ( !resultList.ContainsKey(team.group))
                    resultList.Add(team.group, new List<TeamFootball>());
                resultList[team.group].Add(new TeamFootball(team.name, team.points));                    
            }
            Dictionary<string, TeamFootball[]> result= new Dictionary<string, TeamFootball[]>();
            foreach (KeyValuePair<string, List<TeamFootball>> res in resultList)
                result.Add(res.Key, res.Value.ToArray());
            return result;
        }

        public static Dictionary<string, Dictionary<string, string>> GetRulesTable(string filePath)
        {
            var engine = new FileHelperEngine<RulesTable>();
            var records = engine.ReadFile(filePath);
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
            foreach (RulesTable rules in records)
            {
                Dictionary<string, string> tmp = new Dictionary<string, string>();
                tmp.Add("1A", rules.team1);
                tmp.Add("1B", rules.team2);
                tmp.Add("1C", rules.team3);
                tmp.Add("1D", rules.team4);
                result.Add(rules.key, tmp);
            }
            return result;
        }
    }

    [DelimitedRecord(",")]
    class Rules
    {
        public string team1;
        public string team2;
    }

    [DelimitedRecord(",")]
    class RulesTable
    {
        public string key;
        public string team1;
        public string team2;
        public string team3;
        public string team4;
    }

    [DelimitedRecord(",")]
    class Teams
    {
        public string group;
        public string name;
        public float points;
    }
    
}
