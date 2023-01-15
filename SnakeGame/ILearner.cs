using System.Collections.Generic;

namespace SnakeGame
{
    public enum Action
    {
        Left, Right, Up, Down
    }

    internal interface ILearner
    {
        void InitializeQTable();

        void UpdateQTable(string reason, List<Circle> snake, Circle food);

        Action GetAction(List<Circle> snake, Circle food);
    }
}
