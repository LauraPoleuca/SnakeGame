using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
    internal class Learner : ILearner
    {
        Random rand = new Random();
        public Dictionary<QTableKey, double> QTable { get; set; }
        public QTableKey PreviousQTableKey = new QTableKey();

        public double Epsilon;
        public double LearningRate = 0.7;
        public double Gamma = 0.5;
        public double EpsilonInit = 0.2;
        public double EpsilonEnd = 0.05;
        public int NumberOfEpisodes = 0;
        public int MaxNoOfEpisodes = 100;

        public void InitializeQTable()
        {
            QTable = new Dictionary<QTableKey, double>();
            var possibleStates = GameState.GenerateAllPossibleGameStates();
            foreach (var possibleState in possibleStates)
            {
                foreach (var action in GetAllActions())
                {
                    QTable.Add(new QTableKey
                    {
                        GameState = possibleState,
                        Action = action
                    }, 0);
                }
            }
        }

        public Action GetAction(List<Circle> snake, Circle food)
        {
            var gameState = GetGameState(snake, food);
            QTableKey bestQTableKey = rand.NextDouble() < Epsilon ?
                GetRandomQTableKey(gameState) :
                GetBestQTableKey(gameState);
            PreviousQTableKey = new QTableKey(bestQTableKey);
            DecayEpsilon();
            return bestQTableKey.Action;
        }

        public void UpdateQTable(string reason, List<Circle> snake, Circle food)
        {
            var newState = GetGameState(snake, food);
            var currentQTableKey = PreviousQTableKey;
            var reward = GetReward(reason, snake, food, currentQTableKey.GameState);
            var bestExpectedQValue = GetBestExpectedQValue(newState, reason);
            QTable[currentQTableKey] = (1 - LearningRate) * QTable[currentQTableKey] + LearningRate * (reward + Gamma * bestExpectedQValue);
            NumberOfEpisodes++;
        }

        private void DecayEpsilon()
        {
            var r = Math.Max((MaxNoOfEpisodes - NumberOfEpisodes) / MaxNoOfEpisodes, 0);
            Epsilon = (EpsilonInit - EpsilonEnd) * r + EpsilonEnd;
        }

        private QTableKey GetRandomQTableKey(GameState gameState)
        {
            var actions = GetAllActions();
            return new QTableKey
            {
                GameState = gameState,
                Action = actions[rand.Next(actions.Count())]
            };
        }

        private QTableKey GetBestQTableKey(GameState gameState)
        {
            var actions = GetAllActions();
            QTableKey bestQTableKey = actions
                .Select(act => new QTableKey
                {
                    GameState = gameState,
                    Action = act,
                })
                .OrderByDescending(x => QTable[x])
                .First();
            if (QTable[bestQTableKey] == 0)
            {
                //if the best choice QTable value is 0, there may be more choices with the same value
                //in this case, choose between all the keys that have the QTable value 0
                //if this is missing, especially in the very early trainings, the qtablekey will more often have the Left action
                var bestQTableKeys = actions
                .Select(act => new QTableKey
                {
                    GameState = gameState,
                    Action = act,
                })
                .Where(x => QTable[x] == 0);
                bestQTableKey = bestQTableKeys.ToList()[rand.Next(bestQTableKeys.Count() - 1)];
            }

            return bestQTableKey;
        }

        private static Action[] GetAllActions()
        {
            return (Action[])Enum.GetValues(typeof(Action));
        }

        private double GetBestExpectedQValue(GameState newState, string reason)
        {
            if(!string.IsNullOrEmpty(reason))
            {
                // game reaches end state, no need to look at future states
                return 0;
            }
            return GetAllActions()
                .Select(x => new QTableKey
                {
                    GameState = newState,
                    Action = x,
                })
                .Max(x => QTable[x]);
        }

        private static int GetReward(string reason, List<Circle> snake, Circle food, GameState oldState)
        {
            var snakeHead = snake[0];
            int reward;
            if (!string.IsNullOrEmpty(reason)) // it the reason is not empty, that means some end state event occured
            {
                reward = -100;
            }
            else if (snakeHead.X == food.X && snakeHead.Y == food.Y)
            {
                reward = 10;
            }
            else if (snakeHead.GetDistance(food) < oldState.SnakeHead.GetDistance(food)) // if the current distance is smaller, give a reward
            {
                reward = 1;
            }
            else
            {
                reward = -1;
            }

            return reward;
        }

        public static GameState GetGameState(List<Circle> snake, Circle food)
        {
            var snakeHead = snake[0];
            return new GameState
            {
                FoodHorizontalState = GetFoodHorizontalState(food, snakeHead),
                FoodVerticalState = GetFoodVerticalState(food, snakeHead),
                Surroundings = GetSurroundings(snake),
                Food = new Circle(food),
                SnakeHead = new Circle(snakeHead),
            };
        }

        private static List<bool> GetSurroundings(List<Circle> snake)
        {
            var snakeHead = snake[0];
            var possibleSnakeHeadPositions = new List<Circle>
            {
                new Circle {X = snakeHead.X - 1, Y = snakeHead.Y},
                new Circle {X = snakeHead.X + 1, Y = snakeHead.Y},
                new Circle {X = snakeHead.X, Y = snakeHead.Y - 1},
                new Circle {X = snakeHead.X, Y = snakeHead.Y + 1},
            };
            return possibleSnakeHeadPositions.Select(pos => IsPositionSafe(pos, snake)).ToList();
        }

        private static bool IsPositionSafe(Circle position, List<Circle> snake)
        {
            // out of bounds
            if (position.X < 0 || position.Y < 0) return false;
            if (position.X > 35 || position.Y > 41) return false;
            // snake hits tail (any part of the snake except the head has the same X and Y)
            if (snake.Skip(1).Any(circle => circle.X == position.X && circle.Y == position.Y)) return false;
            return true;
        }

        private static int GetFoodHorizontalState(Circle food, Circle snakeHead)
        {
            int foodHorizontalState = 0;
            if (snakeHead.X < food.X) // food to the right
            {
                foodHorizontalState = 1;
            }
            else if (snakeHead.X > food.X) // food to the left
            {
                foodHorizontalState = -1;
            }
            return foodHorizontalState;
        }

        private static int GetFoodVerticalState(Circle food, Circle snakeHead)
        {
            int foodVerticalState = 0;
            if (snakeHead.Y < food.Y) // food above
            {
                foodVerticalState = 1;
            }
            else if (snakeHead.Y > food.Y) // food below
            {
                foodVerticalState = -1;
            }
            return foodVerticalState;
        }
    }
}
