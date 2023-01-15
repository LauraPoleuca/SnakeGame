using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {

        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();

        int maxWidth;
        int maxHeight;

        int score;

        Random rand = new Random();

        bool goLeft, goRight, goDown, goUp;
        Learner learner;

        string currentDirection = "left";
        int gameCounter = 0;
        int maxScore = -1;

        public Form1()
        {
            InitializeComponent();
            gameTimer.Interval = 1000 / 1000;
            new Settings();
            learner = new Learner();
            learner.InitializeQTable();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && Settings.directions != "right")
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right && Settings.directions != "left")
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up && Settings.directions != "down")
            {
                goUp = true;
            }
            if (e.KeyCode == Keys.Down && Settings.directions != "up")
            {
                goDown = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }
        }

        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            // setting the directions
            string reason = null;
            if (true)
            {
                var action = learner.GetAction(Snake, food);
                switch (action)
                {
                    case Learner.Action.Left:
                        Settings.directions = "left";
                        break;
                    case Learner.Action.Right:
                        Settings.directions = "right";
                        break;
                    case Learner.Action.Up:
                        Settings.directions = "up";
                        break;
                    case Learner.Action.Down:
                        Settings.directions = "down";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (goLeft)
                {
                    Settings.directions = "left";
                }
                if (goRight)
                {
                    Settings.directions = "right";
                }
                if (goDown)
                {
                    Settings.directions = "down";
                }
                if (goUp)
                {
                    Settings.directions = "up";
                }
            }
            if (Settings.directions == "left" && currentDirection == "right")
            {
                Settings.directions = "right";
            }
            if (Settings.directions == "right" && currentDirection == "left")
            {
                Settings.directions = "left";
            }
            if (Settings.directions == "up" && currentDirection == "down")
            {
                Settings.directions = "down";
            }
            if (Settings.directions == "down" && currentDirection == "up")
            {
                Settings.directions = "up";
            }

            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.directions)
                    {
                        case "left":
                            Snake[i].X--;
                            break;
                        case "right":
                            Snake[i].X++;
                            break;
                        case "down":
                            Snake[i].Y++;
                            break;
                        case "up":
                            Snake[i].Y--;
                            break;
                    }

                    if (Snake[i].X < 0)
                    {
                        reason = "Screen";
                        GameOver();
                    }
                    if (Snake[i].X > maxWidth)
                    {
                        reason = "Screen";
                        GameOver();
                    }
                    if (Snake[i].Y < 0)
                    {
                        reason = "Screen";
                        GameOver();
                    }
                    if (Snake[i].Y > maxHeight)
                    {
                        reason = "Screen";
                        GameOver();
                    }

                    if (Snake[i].X == food.X && Snake[i].Y == food.Y)
                    {
                        EatFood();
                    }

                    for (int j = 1; j < Snake.Count; j++)
                    {

                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            reason = "Tail";
                            GameOver();
                        }
                    }
                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
            currentDirection = Settings.directions;

            learner.UpdateQTable(reason, Snake, food);

            picCanvas.Invalidate();
        }

        private void UpdatePictureBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush snakeColour;

            for (int i = 0; i < Snake.Count; i++)
            {
                if (i == 0)
                {
                    snakeColour = Brushes.Teal;
                }
                else
                {
                    snakeColour = Brushes.LightSeaGreen;
                }

                canvas.FillEllipse(snakeColour, new Rectangle
                    (
                    Snake[i].X * Settings.Width,
                    Snake[i].Y * Settings.Height,
                    Settings.Width, Settings.Height
                    ));
            }


            canvas.FillEllipse(Brushes.MediumVioletRed, new Rectangle
            (
            food.X * Settings.Width,
            food.Y * Settings.Height,
            Settings.Width, Settings.Height
            ));
        }

        private void RestartGame()
        {
            maxWidth = picCanvas.Width / Settings.Width - 1;
            maxHeight = picCanvas.Height / Settings.Height - 1;

            Snake.Clear();

            startButton.Enabled = false;
            score = 0;
            txtScore.Text = "Score: " + score;

            Circle head = new Circle { X = 16, Y = 16 };
            Snake.Add(head); // adding the head part of the snake to the list

            for (int i = 0; i < 3; i++)
            {
                Circle body = new Circle();
                Snake.Add(body);
            }

            food = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };

            gameTimer.Start();

        }

        private void EatFood()
        {
            score += 1;

            txtScore.Text = "Score: " + score;

            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            Snake.Add(body);

            food = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };
        }

        private void GameOver()
        {
            gameTimer.Stop();
            startButton.Enabled = true;
            gameCounter++;
            if (maxScore < score)
            {
                Console.WriteLine("NEW HIGH SCORE!");
                maxScore = score;
            }
            Console.WriteLine("Game " + gameCounter + " finished with a score of " + score);
            //TODO: reorganize code
            //split methods
            // save and load qvalues
            //
            RestartGame();
        }
    }
}
