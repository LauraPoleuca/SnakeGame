using System;
using System.Collections.Generic;

namespace SnakeGame
{
    internal class QTableKey
    {
        public GameState GameState { get; set; }
        public Action Action { get; set; }

        public override bool Equals(object obj)
        {
            return obj is QTableKey key &&
                   EqualityComparer<GameState>.Default.Equals(GameState, key.GameState) &&
                   EqualityComparer<Action>.Default.Equals(Action, key.Action);
        }

        public QTableKey() { }

        public QTableKey(QTableKey other)
        {
            GameState = new GameState(other.GameState);
            Action = other.Action;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GameState, Action);
        }
    }
}
