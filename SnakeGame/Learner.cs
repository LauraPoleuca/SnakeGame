using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
    internal class Learner
    {
        public Dictionary<(GameState, Action), double> QTable { get;set; }

        public enum Action
        {
            Left, Right, Up, Down
        }

        public void InitializeQTable()
        {
            QTable = new Dictionary<(GameState, Action), double>();
            var possibleStates = GameState.GenerateAllPossibleGameStates();
            foreach (var possibleState in possibleStates)
            {
                QTable.Add((possibleState, Action.Left), 0);
                QTable.Add((possibleState, Action.Right), 0);
                QTable.Add((possibleState, Action.Up), 0);
                QTable.Add((possibleState, Action.Down), 0);
            }
        }

        public string GetAction(List<Circle> snake, Circle food)
        {
            var state = GetGameState(snake, food);
            throw new NotImplementedException();
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
