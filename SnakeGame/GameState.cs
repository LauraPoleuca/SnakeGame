using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
    internal class GameState
    {
        public Circle SnakeHead { get; set; }
        public Circle Food { get; set; }
        public int FoodVerticalState { get; set; } // 1 food is above, -1 food is below, 0 food is on the same line
        public int FoodHorizontalState { get; set; } // 1 food is right of the head, -1 is left, 0 is on the same line
        public List<bool> Surroundings { get; set; }

        public static List<GameState> GenerateAllPossibleGameStates()
        {
            var possibleGameStates = new List<GameState>();
            foreach (var foodVerticalState in new int[] { -1, 0, 1 })
            {
                foreach (var foodHorizontalState in new int[] { -1, 0, 1 })
                {
                    foreach (var surroundingRep in Enumerable.Range(0, 16))
                    {
                        possibleGameStates.Add(new GameState
                        {
                            FoodHorizontalState = foodHorizontalState,
                            FoodVerticalState = foodVerticalState,
                            Surroundings = new List<bool>
                                {
                                    surroundingRep / 1 % 2 == 0,
                                    surroundingRep / 2 % 2 == 0,
                                    surroundingRep / 4 % 2 == 0,
                                    surroundingRep / 8 % 2 == 0,
                                }
                        });
                    }
                }
            }
            return possibleGameStates;
        }

        public override bool Equals(object obj)
        {
            return obj is GameState state &&
                   //EqualityComparer<Circle>.Default.Equals(Food, state.Food) &&
                   FoodVerticalState == state.FoodVerticalState &&
                   FoodHorizontalState == state.FoodHorizontalState &&
                   new SurroundingsComparer().Equals(Surroundings, state.Surroundings);
        }

        private class SurroundingsComparer : EqualityComparer<List<bool>>
        {
            public override bool Equals(List<bool> x, List<bool> y)
            {
                if (x.Count != y.Count) return false;
                for (int index = 0; index < x.Count; index++)
                {
                    if (x[index] != y[index]) return false;
                }
                return true;
            }

            public override int GetHashCode(List<bool> obj)
            {
                HashCode hash = new HashCode();
                foreach (var state in obj)
                {
                    hash.Add(state);
                }
                return hash.ToHashCode();
            }
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(FoodVerticalState);
            hash.Add(FoodHorizontalState);
            foreach (var state in Surroundings)
            {
                hash.Add(state);
            }
            return hash.ToHashCode();
        }
    }
}
