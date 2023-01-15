using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var g = new GameState
            //{
            //    FoodHorizontalState = -1,
            //    FoodVerticalState= -1,
            //    Surroundings = new List<bool> { true, false, true, false },
            //};
            //var h = new GameState
            //{
            //    FoodHorizontalState = -1,
            //    FoodVerticalState = -1,
            //    Surroundings = new List<bool> { true, false, true, false },
            //    Food = new Circle()
            //};
            //var a = g.Equals(h);
            //var t1 = new QTableKey 
            //{ 
            //    GameState= g,
            //    Action = Learner.Action.Down
            //};
            //var t2 = new QTableKey
            //{
            //    GameState = g,
            //    Action = Learner.Action.Down
            //};
            //var t3 = new QTableKey
            //{
            //    GameState = g,
            //    Action = Learner.Action.Left
            //};
            //var x = t1.GetHashCode() == t2.GetHashCode();
            //var y = t1.GetHashCode() == t3.GetHashCode();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
