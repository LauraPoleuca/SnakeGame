using System;
using System.Collections.Generic;

namespace SnakeGame
{
    internal class QTableKey
    {
        public GameState GameState { get; set; }
        public Learner.Action Action { get; set; }

        public override bool Equals(object obj)
        {
            return obj is QTableKey key &&
                   EqualityComparer<GameState>.Default.Equals(GameState, key.GameState) &&
                   EqualityComparer<Learner.Action>.Default.Equals(Action, key.Action);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GameState, Action);
        }
    }
}
