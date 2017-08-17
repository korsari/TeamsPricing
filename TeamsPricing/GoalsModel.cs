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
                case "QuickWeighted":
                    (goals1, goals2) = QuickRandWeightedDrawn(team1.Points, team2.Points, allowDrawn);
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
                goals1 = (int)Math.Max(0, Rand.Next(0, 3) + points1);
                goals2 = (int)Math.Max(0, Rand.Next(0, 3) + points2);
            } while (allowDrawn && goals1 == goals2);
            return (goals1, goals2);
        }

        private (int,int) Poisson(float points1, float points2, bool allowDrawn)
        {
            
            return (0, 0);
        }

        private (int,int) QuickRandWeightedDrawn(float points1, float points2, bool allowDrawn)
        {
            int points1Int = (int)(points1 * 100);
            int points2Int = (int)(points2 * 100);
            int maxDrawnProbability = 20; // Max probability of a drawn in %
            // drawnProbability is weighted to reflect the two teams points difference
            int drawnProbability = (int)(maxDrawnProbability / Math.Max(1, Math.Abs(points1 - points2)));
            int r = Rand.Next(0, 100);
            int goals1, goals2;
            if (r < drawnProbability && allowDrawn)
            {
                goals1 = goals2 = Math.Min(points1Int, points2Int);
            }
            else
            {
                r = Rand.Next(points1Int + points2Int);
                if (points1 < points2)
                {
                    goals1 = points1Int;
                    goals2 = r;
                }
                else
                {
                    goals1 = r;
                    goals2 = points2Int;
                }
            }
            return (goals1, goals2);
        }
    }
}
