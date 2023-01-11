using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
    internal class GameState
    {
        public int FoodVerticalState { get; set; } // 1 food is above, -1 food is below, 0 food is on the same line
        public int FoodHorizontalState { get; set; } // 1 food is right of the head, -1 is left, 0 is on the same line
        public List<bool> Surroundings { get; set; }

        public static List<GameState> GenerateAllPossibleGameStates()
        {
            var possibleGameStates = new List<GameState>();
            foreach (var foodVerticalState in new int[] { -1, 0, 1 })
            {
                foreach (var foodHorizontalState in new int[] { -1, 0, 1})
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

        public static string GetGameStateActionString(GameState gameState)
        {
            return gameState.FoodVerticalState.ToString() +
                gameState.FoodHorizontalState.ToString() +
                string.Join("", gameState.Surroundings.Select(x => x ? "1" : "0"));
        }
    }
}
