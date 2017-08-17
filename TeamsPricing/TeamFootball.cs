using System;

namespace TeamsPricing
{
    class TeamFootball : IComparable
    {
        public float Points { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }

        public int Won { get; set; }

        public int Lost { get; set; }

        public int Drawn { get; set; } 
        
        public int GoalsFor { get; set; }
        
        public int GoalsAgainst { get; set; }

        public int GoalsDifference { get; set; }

        public TeamFootball(string name, float points)
        {
            Points = points;
            Name = name;
            Score = 0;
            Won = 0;
            Lost = 0;
            Drawn = 0;
            GoalsFor = 0;
            GoalsAgainst = 0;
            GoalsDifference = 0;
        }

        public void OneUp(int goalsFor, int goalsAgainst)
        {
            GoalsFor += goalsFor;
            GoalsAgainst += goalsAgainst;
            GoalsDifference += goalsFor - goalsAgainst;
            Won++;
            Score += 3;
        }

        public void OneNada(int goalsFor, int goalsAgainst)
        {
            GoalsFor += goalsFor;
            GoalsAgainst += goalsAgainst;
            GoalsDifference += goalsFor - goalsAgainst;
            Drawn++;
            Score++;
        }

        public void OneDown(int goalsFor, int goalsAgainst)
        {
            GoalsFor += goalsFor;
            GoalsAgainst += goalsAgainst;
            GoalsDifference += goalsFor - goalsAgainst;
            Lost++;
        }

        public void Reset()
        {
            Score = 0;
            Won = 0;
            Lost = 0;
            Drawn = 0;
            GoalsFor = 0;
            GoalsAgainst = 0;
            GoalsDifference = 0;
        }

        public void Print()
        {
            Console.WriteLine("Team {0} scored {1} points by winning {2} and losing {3} matches and {4} drawn.", Name, Score, Won, Lost, Drawn);
            Console.WriteLine("Details: Goals for= {0} Goals against={1} => Goals Difference={2}", GoalsFor, GoalsAgainst, GoalsDifference);
            Console.ReadLine();
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            TeamFootball opponent = obj as TeamFootball;
            if (opponent != null)
            {
                // Below comparison implements: https://en.wikipedia.org/wiki/UEFA_Euro_2016#Tiebreakers
                // skippings criteria 1,2,3,4 as we don t record scores per match
                if (Score != opponent.Score)
                    return Score.CompareTo(opponent.Score);
                else if (GoalsDifference != opponent.GoalsDifference)
                    return GoalsDifference.CompareTo(opponent.GoalsDifference);
                else
                    return Points.CompareTo(opponent.Points);
            }
            else
            {
                throw new ArgumentException("Object in comparison is not TeamFootball");
            }
        }
    }

}
