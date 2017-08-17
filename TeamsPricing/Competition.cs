using System;
using System.Collections.Generic;
using System.Linq;


namespace TeamsPricing
{
    class Competition
    {
        private Dictionary<string, TeamFootball[]> Groups;
        private int nTeams;
        private Tuple<TeamFootball,TeamFootball>[] MatchesKnockout;
        private Tuple<string, string>[] SelectionRules;
        private Dictionary<string, Dictionary<string, string>> SelectionRulesTable;
        private GoalsModel Goals;

        public Competition(Dictionary<string, TeamFootball[]> groups, Tuple<string, string>[] selectionRules, 
            Dictionary<string, Dictionary<string, string>> selectionRulesTable, MethodToUse methodToUse=MethodToUse.none)
        {
            nTeams = 0;
            foreach (KeyValuePair<string, TeamFootball[]> group in groups)
            {
                nTeams += group.Value.Length;
            }
            MatchesKnockout = new Tuple<TeamFootball, TeamFootball>[15];
            Groups = new Dictionary<string, TeamFootball[]>();
            Groups = groups;
            SelectionRules = selectionRules;
            SelectionRulesTable = selectionRulesTable;
            Goals = new GoalsModel(methodToUse);
        }

        public TeamFootball[] PlayCompetition()
        {
            TeamFootball[] finalRanking = new TeamFootball[nTeams];
            int k = 0;
            foreach (TeamFootball[] group in Groups.Values)
            {
                foreach (TeamFootball team in group)
                {
                    finalRanking[k] = team;
                    k++;
                }
            }
            //First we play all the groups games to rank teams in each group
            foreach (TeamFootball[] group in Groups.Values)
            {
                for (int i = 0; i < group.Length-1; i++)
                {
                    for (int j = i+1; j < group.Length; j++)
                    {
                        TeamFootball team1 = group[i];
                        TeamFootball team2 = group[j];
                        TeamFootball winner;
                        if (team1.Name != team2.Name)
                        {
                            winner = Goals.PlayMatch(team1, team2, true);
                        }
                    }
                }
            }
            populateMatches();

            // We now play the knockout phase
            for (int i = 8; i>0; i/=2)
            {
                for (int j = 0; j < i; j+=2)
                {
                    Tuple<TeamFootball, TeamFootball> match1 = MatchesKnockout[i - 1 + j];
                    Tuple<TeamFootball, TeamFootball> match2 = MatchesKnockout[i + j];
                    TeamFootball winnerOne = Goals.PlayMatch(match1.Item1, match1.Item2, false);
                    TeamFootball winnerTwo = Goals.PlayMatch(match2.Item1, match2.Item2, false);
                    if (i>1)
                        MatchesKnockout[(i + j) / 2 - 1] = new Tuple<TeamFootball, TeamFootball>(winnerOne, winnerTwo);

                }
            }
            Array.Sort(finalRanking);
            Array.Reverse(finalRanking);
            return finalRanking;
        }

        private void populateMatches()
        // Function that populates the first roud of matches based on the groups matches results
        // we first sort each group wrt 1/ their score 2/ their fifa ranking
        // we then populate the first knockout matches based on the tournament rules
        {
            Tuple<TeamFootball, string>[] thirdsOfGroups = new Tuple<TeamFootball, string>[Groups.Count];
            int i = 0;
            foreach (KeyValuePair<string, TeamFootball[]> group in Groups)
            {
                Array.Sort(group.Value);
                Array.Reverse(group.Value);
                thirdsOfGroups[i] = new Tuple<TeamFootball, string>(group.Value[2], group.Key);
                i++;
            }
            Array.Sort(thirdsOfGroups);
            Array.Reverse(thirdsOfGroups);
            string[] fourBests = new string[4] {
                thirdsOfGroups[0].Item2,
                thirdsOfGroups[1].Item2,
                thirdsOfGroups[2].Item2,
                thirdsOfGroups[3].Item2 };
            Array.Sort(fourBests);
            string keyRules = String.Concat(fourBests);

            i = 0;
            foreach (Tuple<string, string> match in SelectionRules)
            {
                int rank1 = (int)char.GetNumericValue(match.Item1[0]);
                string group1 = match.Item1[1].ToString();
                int rank2;
                string group2;
                if (match.Item2 != "table")
                {
                    rank2 = (int)Char.GetNumericValue(match.Item2[0]);
                    group2 = match.Item2[1].ToString();
                }
                else
                {
                    string match2 = SelectionRulesTable[keyRules][match.Item1];
                    rank2 = (int)Char.GetNumericValue(match2[0]);
                    group2 = match2[1].ToString();

                }
                MatchesKnockout[i + 8-1] = new Tuple<TeamFootball, TeamFootball>(Groups[group1][rank1 - 1], Groups[group2][rank2 - 1]);
                i++;
            }

        }

        public void Reset()
        {
            foreach (var team in Groups.SelectMany(x => x.Value))
            {
                   team.Reset();
            }
            Array.Clear(MatchesKnockout,0,MatchesKnockout.Length);
        }

    }
}
