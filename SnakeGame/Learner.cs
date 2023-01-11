using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
    internal class Learner
    {
        Random rand = new Random();
        public Dictionary<string, double> QTable { get;set; }

        public double Epsilon = 1;

        public enum Action
        {
            Left, Right, Up, Down
        }

        private int GetActionInt(Action action)
        {
            return (int)action;
        }

        public void InitializeQTable()
        {
            QTable = new Dictionary<string, double>();
            var possibleStates = GameState.GenerateAllPossibleGameStates();
            foreach (var possibleState in possibleStates)
            {
                foreach(var action in (Action[])Enum.GetValues(typeof(Action))) 
                {
                    var possibleStateString = GameState.GetGameStateActionString(possibleState);
                    var actionString = GetActionInt(action).ToString();
                    QTable.Add(possibleStateString + actionString, 0);
                }
            }
        }

        public Action GetAction(List<Circle> snake, Circle food)
        {
            Action action;
            var actions = (Action[])Enum.GetValues(typeof(Action));
            if (rand.NextDouble() < Epsilon)
            {
                action = actions[rand.Next(actions.Count())];
            }
            else
            {
                var stateString = GameState.GetGameStateActionString(GetGameState(snake, food));
                var bestStateString = actions
                    .Select(act => stateString + GetActionInt(act).ToString())
                    .OrderByDescending(x => QTable[x])
                    .First();
                action = (Action)(bestStateString[bestStateString.Length - 1] - '0');
            }
            return action;
        }

        private GameState GetGameState(List<Circle> snake, Circle food)
        {
            var snakeHead = snake[0];
            return new GameState
            {
                FoodHorizontalState = GetFoodHorizontalState(food, snakeHead),
                FoodVerticalState = GetFoodVerticalState(food, snakeHead),
                Surroundings = GetSurroundings(snake)
            };
        }

        private List<bool> GetSurroundings(List<Circle> snake)
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

        private bool IsPositionSafe(Circle position, List<Circle> snake)
        {
            // out of bounds
            if (position.X < 0 || position.Y < 0) return false;
            if (position.X > Settings.Width || position.Y > Settings.Height) return false;
            // snake hits tail ( any part of the snake except the head has the same X and Y)
            if (snake.Skip(1).Any(circle => circle.X == position.X && circle.Y == position.Y)) return false;
            return true;
        }

        private int GetFoodHorizontalState(Circle food, Circle snakeHead)
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
