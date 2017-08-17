using System;

namespace TeamsPricing
{
    class TeamFootball : IComparable
    {
        public float Points { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }

        private int _won;

        public int Won
        {
            get { return _won; }
            set { _won = value;  
                // what !!!! huge side effect !!!
                // tu retires ca et tu fais un truc propre
                Score += 3; }
        }
        public int Lost { get; set; }
        private int _drawn;
        public int Drawn
        {
            get { return _drawn; }
            set { _drawn = value;  Score += 1; }
        }
        private int _goalFor;
        public int GoalFor {
            get { return _goalFor; }
            set
            {
                int prec = _goalFor;
                _goalFor = value;
                // je suis pas fan... 
                // pq GoalDifference te retourne pas directement GoalFor - GoalAgainst ?
                GoalDifference += value-prec;
            }
        }
        private int _goalAgainst;
        public int GoalAgainst
        {
            get { return _goalAgainst; }
            set
            {
                int prec = _goalAgainst;
                _goalAgainst = value;
                GoalDifference -= value-prec;
            }
        }
        public int GoalDifference { get; set; }

        public TeamFootball(string name, float points)
        {
            Points = points;
            Name = name;
            // Best practice, une ligne par initialisation
            // ca tevitera a te faire chier quand tu voudras changer qqch et c'est plus simple a trouver
            Score = Won = Lost = Drawn = GoalFor = GoalAgainst = GoalDifference = 0;
        }

        public void Reset()
        {
            Score = Won = Lost = Drawn = GoalFor = GoalAgainst = GoalDifference = 0;
        }

        public void Print()
        {
            // Tu as une methode ToString overridable qui est faite pour ca !
            Console.WriteLine("Team {0} scored {1} points by winning {2} and losing {3} matches and {4} drawn.", Name, Score, Won, Lost, Drawn);
            Console.WriteLine("Details: Goals for= {0} Goals against={1} => Goals Difference={2}", GoalFor, GoalAgainst, GoalDifference);
            Console.ReadLine();
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            TeamFootball opponent = obj as TeamFootball;
            if (opponent != null)
            {
                // Below comparison implements: https://en.wikipedia.org/wiki/UEFA_Euro_2016#Tiebreakers
                // skippings criteria 1,2,3,4 as we don t record scores per match
                if (Score != opponent.Score)
                    return Score.CompareTo(opponent.Score);
                else if (GoalDifference != opponent.GoalDifference)
                    return GoalDifference.CompareTo(opponent.GoalDifference);
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
