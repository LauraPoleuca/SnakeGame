using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
    internal class Learner
    {
        Random rand = new Random();
        public Dictionary<QTableKey, double> QTable { get;set; }
        public List<(GameState, Action)> History = new List<(GameState, Action)>();

        public double Epsilon = 0.1;
        public double LearningRate = 0.1;
        public double Gammma = 0.99;

        public enum Action
        {
            Left, Right, Up, Down
        }

        public void InitializeQTable()
        {
            QTable = new Dictionary<QTableKey, double>();
            var possibleStates = GameState.GenerateAllPossibleGameStates();
            foreach (var possibleState in possibleStates)
            {
                foreach(var action in (Action[])Enum.GetValues(typeof(Action))) 
                {
                    QTable.Add(new QTableKey { GameState = possibleState, Action = action }, 0);
                }
            }
        }

        public Action GetAction(List<Circle> snake, Circle food)
        {
            Action action;
            var actions = GetAllActions();
            var gameState = GetGameState(snake, food);
            if (rand.NextDouble() < Epsilon)
            {
                action = actions[rand.Next(actions.Count())];
            }
            else
            {
                var bestQTableKey = actions
                    .Select(act => new QTableKey
                    {
                        GameState = gameState,
                        Action = act,
                    })
                    .OrderByDescending(x => QTable[x])
                    .First();
                if (QTable[bestQTableKey] == 0)
                {
                    var bestQTableKeys = actions
                    .Select(act => new QTableKey
                    {
                        GameState = gameState,
                        Action = act,
                    })
                    .Where(x => QTable[x] == 0);
                    bestQTableKey = bestQTableKeys.ToList()[rand.Next(bestQTableKeys.Count() - 1)];
                }
                action = bestQTableKey.Action;
            }
            History.Add((gameState, action));
            return action;
        }

        private static Action[] GetAllActions()
        {
            return (Action[])Enum.GetValues(typeof(Action));
        }

        public void UpdateQTable(string reason, List<Circle> snake, Circle food)
        {
            var newState = GetGameState(snake, food);
            var currentQTableKey = new QTableKey
            {
                GameState = History.Last().Item1,
                Action = History.Last().Item2,
            };
            int reward = GetReward(reason, snake, food);
            var asd = GetAllActions().Select(x => new QTableKey
            {
                GameState = newState,
                Action= x,
            }).Max(x => QTable[x]);
            QTable[currentQTableKey] = QTable[currentQTableKey] +
                    LearningRate * (reward + Gammma * asd - QTable[currentQTableKey]);
        }

        private static int GetReward(string reason, List<Circle> snake, Circle food)
        {
            var reward = 0;
            if (!string.IsNullOrEmpty(reason)) // it the reason is not empty, that means some end state event occured
            {
                reward = -1;
            }
            else if (snake[0].X == food.X && snake[0].Y == food.Y)
            {
                reward = 5;
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
                Surroundings = GetSurroundings(snake)
            };
        }

        private static List<bool> GetSurroundings(List<Circle> snake)
        {
            var snakeHead = snake[0];
            var possibleSnakeHeadPositions = new List<Circle>
            { 
                new Circle {X = snake[0].X - 1, Y = snake[0].Y},
                new Circle {X = snake[0].X + 1, Y = snake[0].Y},
                new Circle {X = snake[0].X, Y = snake[0].Y - 1},
                new Circle {X = snake[0].X, Y = snake[0].Y + 1},
            };
            return possibleSnakeHeadPositions.Select(pos => IsPositionSafe(pos, snake)).ToList();
        }

        private static bool IsPositionSafe(Circle position, List<Circle> snake)
        {
            // out of bounds
            if (position.X < 0 || position.Y < 0) return false;
            if (position.X > Settings.Width || position.Y > Settings.Height) return false;
            // snake hits tail ( any part of the snake except the head has the same X and Y)
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
