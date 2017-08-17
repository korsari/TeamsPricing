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
        private MethodToUse Method;

        public GoalsModel()
        {
            Rand = new Random();
        }

        public GoalsModel(Random rand)
        {
            Rand = rand;
        }

        public GoalsModel(MethodToUse method)
        {
            Method = method;
            Rand = new Random();
        }

        public GoalsModel(MethodToUse method, Random rand)
        {
            Method = method;
            Rand = rand;
        }

        public TeamFootball PlayMatch(TeamFootball team1, TeamFootball team2, bool allowDrawn)
            // returns true for winner, false for loser, and (true,true) if drawn
            // also populates the teams points numbers
        {
            // TODO: -find a better drawing system
            //       -populate team1 and team2 with the goals
            int goals1, goals2;
            switch (Method)
            {
                case MethodToUse.stupid:
                    (goals1, goals2) = StupidMethod(team1.Points, team2.Points, allowDrawn);
                    break;
                case MethodToUse.poisson:
                    (goals1, goals2) = Poisson(team1.Points, team2.Points, allowDrawn);
                    break;
                case MethodToUse.quickWeighted:
                    (goals1, goals2) = QuickRandWeightedDrawn(team1.Points, team2.Points, allowDrawn);
                    break;
                default:
                    (goals1, goals2) = StupidMethod(team1.Points, team2.Points, allowDrawn);
                    break;
            }
            if (goals1 == goals2)
            {
                team1.OneNada(goals1,goals2);
                team2.OneNada(goals2,goals1);
                return team1;
            }
            if (goals1 > goals2)
            {
                team1.OneUp(goals1, goals2);
                team2.OneDown(goals2, goals1);
                return team1;
            }
            else
            {
                team1.OneDown(goals1, goals2);
                team2.OneUp(goals2, goals1);
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

    public enum MethodToUse
    {
        stupid,
        poisson,
        quickWeighted,
        none
    }
}
