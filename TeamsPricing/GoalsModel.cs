using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TeamsPricing
{
    class GoalsModel
    {
        private Random Rand;
        private string MethodToUse;

        public GoalsModel()
        {
            Rand = new Random();
        }

        public GoalsModel(Random rand)
        {
            Rand = rand;
        }

        public GoalsModel(string methodToUse)
        {
            MethodToUse = methodToUse;
            Rand = new Random();
        }

        public GoalsModel(string methodToUse, Random rand)
        {
            MethodToUse = methodToUse;
            Rand = rand;
        }

        public TeamFootball PlayMatch(TeamFootball team1, TeamFootball team2, bool allowDrawn)
            // returns true for winner, false for loser, and (true,true) if drawn
            // also populates the teams points numbers
        {
            // TODO: -find a better drawing system
            //       -populate team1 and team2 with the goals
            int goals1, goals2;
            switch (MethodToUse)
            {
                case "stupid":
                    (goals1, goals2) = StupidMethod(team1.Points, team2.Points, allowDrawn);
                    break;
                case "poisson":
                    (goals1, goals2) = Poisson(team1.Points, team2.Points, allowDrawn);
                    break;
                default:
                    (goals1, goals2) = StupidMethod(team1.Points, team2.Points, allowDrawn);
                    break;
            }
            if (goals1 == goals2)
            {
                team1.Drawn++;
                team2.Drawn++;
                return team1;
            }
            if (goals1 > goals2)
            {
                team1.Won++;
                team1.GoalFor += goals1;
                team1.GoalAgainst += goals1;
                team2.Lost++;
                team2.GoalFor += goals2;
                team2.GoalAgainst += goals2;
                return team1;
            }
            else
            {
                team2.Won++;
                team2.GoalFor += goals2;
                team2.GoalAgainst += goals2;
                team1.Lost++;
                team1.GoalFor += goals1;
                team1.GoalAgainst += goals1;
                return team2;
            }
            
        }

        private (int,int) StupidMethod(float points1, float points2, bool allowDrawn)
        {
            int goals1, goals2;
            do
            {
                goals1 = (int)Math.Max(0, Rand.Next(0, 5) + points1);
                goals2 = (int)Math.Max(0, Rand.Next(0, 5) + points2);
            } while (allowDrawn && goals1 == goals2);
            return (goals1, goals2);
        }

        private (int,int) Poisson(float points1, float points2, bool allowDrawn)
        {
            return (0, 0);
        }

        private (int,int) QuickRand(float points1, float points2, bool allowDrawn)
        {
            /*int distance = Math.Max(0, 1 / (points2 == points1 ? 1 : Math.Abs(points2 - points1)) * 10000 - 2000);
            if (Rand.Next(0, 10000) < distance)
            {
                team1.Drawn++;
                team2.Drawn++;
                return team1;
            }
            int r = Rand.Next(0, Math.Max(points1, points2));
            bool SelfWins = r < points1 && (points1 > points2 ? r > points2 : true);
            if (SelfWins)
            {
                team1.Won++;
                team2.Lost++;
                return team1;
            }
            else
            {
                team1.Lost++;
                team2.Lost++;
                return team2;
            }*/
            return (0, 0);
        }
    }
}
