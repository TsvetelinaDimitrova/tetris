using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Tetris
{
    public class Board
    {
        private int Rows;
        private int Cols;
        private int Score;
        private int linesCompleted;
        private string gameOverMessage;
        private bool isgameOver = false;
        private Figure currFigure;
        private Label[,] Cell;

        private Brush emptyBrush = Brushes.Transparent;
        private Brush borderBrush = Brushes.DarkSlateGray;
        
        /**
         * generates Tetris Grid and single Cells inside
         *
         */
        public Board(Grid TetrisGrid)
        {
            Rows = TetrisGrid.RowDefinitions.Count;
            Cols = TetrisGrid.ColumnDefinitions.Count;

            Score = 0;
            linesCompleted = 0;
            gameOverMessage = "";
            
            // emptyBrush is default cell color, will be used to differentiate empty cells
            Cell = new Label[Cols, Rows];
            for (int i = 0; i < Cols; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    Cell[i, j] = new Label();
                    Cell[i, j].Background = emptyBrush;
                    Cell[i, j].BorderBrush = borderBrush;
                    Cell[i, j].BorderThickness = new Thickness(1, 1, 1, 1);
                    Grid.SetRow(Cell[i,j],j);
                    Grid.SetColumn(Cell[i,j],i);
                    TetrisGrid.Children.Add(Cell[i, j]);
                    

                }
            }

            currFigure = new Figure();
            generateCurrFigure();
        }

        //--------------------------
        //         GETTERS
        //--------------------------
        public int getScore()
        {
            return Score;
        }

        public int getCompletedLines()
        {
            return linesCompleted;
        }

        public string getGameOverText()
        {
            return gameOverMessage;
        }


        /**
         * Generates new Figure
         */
        private void generateCurrFigure()
        {
            //where to draw?
            Point Position = currFigure.getCurrPosition();
            //what to draw?
            Point[] Shape = currFigure.getCurrShape();
            // color
            Brush Color = currFigure.getCurrColor();
            
            
        
            foreach (Point S in Shape)
            {
                Cell[(int) (S.X + Position.X) + ((Cols / 2) - 1),
                    (int) (S.Y + Position.Y) +2 ].Background = Color;
            }
        }

        private void deleteCurrFigure()
        {
            Point Position = currFigure.getCurrPosition();
            Point[] Shape = currFigure.getCurrShape();
            foreach (Point S in Shape)
            {
                Cell[(int)(S.X + Position.X) + ((Cols / 2) - 1),
                    (int)(S.Y + Position.Y) + 2].Background = emptyBrush;
            }
        }

        private void CheckCompletedRows()
        {
            bool full;
            for (int i = Rows -1; i > 0; i--)
            {
                full = true;
                for (int j = 0; j < Cols; j++)
                {
                    //if anywhere Background  is emptyBrush it means line is not full
                    if (Cell[j, i].Background == emptyBrush)
                    {
                        full  = false;
                    }
                }
                if (full)
                {
                    RemoveCompletedRow(i);
                    Score += 100;
                    linesCompleted += 1;
                     i++;
                }
            }
        }

        private void RemoveCompletedRow(int row)
        {
            for (int i = row; i > 2; i--)
            {
                for (int j = 0; j < Cols ; j++)
                {
                  
                    Cell[j, i].Background = Cell[j, i -1].Background;
                }
            }
        }

        // ------------------------
        //        Move commands              
        //-------------------------
        
        public void CurrFigureMovLeft()
        {
            Point Position = currFigure.getCurrPosition();
            Point[] Shape = currFigure.getCurrShape();
            bool move = true;
            deleteCurrFigure();
            
            // situations to block movement
            foreach (Point S in Shape)
            {
                // touching grid
                if (((int) (S.X + Position.X) + ((Cols / 2) - 1) - 1) < 0)
                {
                    move = false;
                }
                //  cell is already occupied
                else if (Cell[((int) (S.X + Position.X) + ((Cols / 2) - 1) - 1),
                    (int) (S.Y + Position.Y) + 2].Background != emptyBrush)
                {
                    move = false;
                }
            }
            if (move) {
                currFigure.movLeft();
                generateCurrFigure();
            }
            else
            {
                generateCurrFigure();
            }
        }
        public void CurrFigureMovRight () 
        {
            Point Position = currFigure.getCurrPosition();
            Point[] Shape = currFigure.getCurrShape();
            bool move = true;
            deleteCurrFigure();
            foreach (Point S in Shape)
            {
                
                if (((int)(S.X + Position.X) + ((Cols / 2) - 1) + 1)>= Cols)
                {
                    move = false;
                }
                
                else if (Cell[((int)(S.X + Position.X) + ((Cols / 2) - 1) + 1),
                    (int)(S.Y + Position.Y) + 2].Background != emptyBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currFigure.movRight();
                generateCurrFigure();
            }
            else
            {
                generateCurrFigure();
            }
        }
        public void CurrFigureMovDown()
        {
            Point Position = currFigure.getCurrPosition();
            Point[] Shape = currFigure.getCurrShape();
            bool move = true;
            deleteCurrFigure();
            foreach (Point S in Shape)
            {
                
                if (((int)(S.Y + Position.Y) + ((Cols / 2) - 1) -1) >= Rows)
                {
                    move = false;
                }
                
                else if (Cell[((int)(S.X + Position.X) + ((Cols / 2) - 1) ),
                    (int)(S.Y + Position.Y) + 2+1].Background != emptyBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currFigure.movDown();
                generateCurrFigure();
            }
            else
            {
                // checks for game over
                // if movement is blocked we check if anything from highest row is occupied . If yes, game over
                for (int j = 0; j < Cols; j++)
                    {
    
                        if (Cell[j, 1].Background != emptyBrush )
                        {
                            isgameOver = true;
                        }
                    }
                    // if every cell in a row is full
                    if (isgameOver)
                    {
                        gameOverMessage = "GAME OVER";
                    generateCurrFigure();
                    
                    return;

                    }

             
                else
                {
                    generateCurrFigure();
                    CheckCompletedRows();
                    currFigure = new Figure();
                }
               
                
               
            }
        }
        public void CurrFigureMovRotate()
        {
            Point Position = currFigure.getCurrPosition();
            Point[] S = new Point[4];
            Point[] Shape = currFigure.getCurrShape();
            bool move = true;
            Shape.CopyTo(S, 0);
            deleteCurrFigure();
            for (int i = 0; i <S.Length ; i++)
            {
                double x = S[i].X;
                S[i].X = S[i].Y *-1;
                S[i].Y = x;
                //collision with Y axis
                if (((int) ((S[i].Y + Position.Y) + 2)) >= Rows)
                {
                    move = false;
                }
                //collision with left side of grid
                else if (((int)(S[i].X + Position.X) + ((Cols / 2) - 1)) < 0)
                {
                    move = false;
                }
                //collision with right side of grid
                else if (((int) (S[i].X + Position.X) + ((Cols / 2) - 1)) >= Cols) 
                {
                    move = false;
                }
              //check if another figure is blocking rotation  
                else if (Cell[((int) (S[i].X + Position.X) + ((Cols / 2) - 1)),
                    (int)(S[i].Y + Position.Y) +2].Background != emptyBrush)
                {
                    move = false;
                }
            
            }

            if (move)
            {
                currFigure.movRotate();
                generateCurrFigure();
            }
            else
            {
                generateCurrFigure();
            }
        }
        
    }

    public class Figure
    {
        private Point currPosition;
        private Point[] currShape;
        private Brush currColor;
        private bool rotate;

        public Figure()
        {
            currPosition = new Point(0, 0);
            currColor = Brushes.Transparent;
            currShape = setRandomShape();
        }

        public Brush getCurrColor()
        {
            return currColor;
        }

        public Point getCurrPosition()
        {
            return currPosition;
        }

        public void movLeft()
        {
            currPosition.X -= 1;
        }
        public void movRight()
        {
            currPosition.X +=1;
        }

        public void movDown()
        {
            currPosition.Y += 1;
        }

        public void movRotate()
        {
            if (rotate)
            {
                for (int i = 0; i < currShape.Length; i++)
                {
                    double x = currShape[i].X;
                    currShape[i].X = currShape[i].Y * -1;
                    currShape[i].Y = x;
                }
            }
        }

        public Point[] getCurrShape()
        {
            return currShape;
        }
        
        //blueprints generate figures
        private Point[] setRandomShape()
        {
            Random rand = new Random();
            switch (rand.Next() % 7)
            {
                case 0: // I
                    rotate = true;
                    currColor = Brushes.Cyan;
                    return new Point[]
                    {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(2,0),
                    };
                case 1: // J
                    rotate = true;
                    currColor = Brushes.Blue;
                    return new Point[]
                    {
                        new Point(1,-1), // new Point(1, 1), // changed here
                        new Point(-1,0),
                        new Point(0,0),
                        new Point(1,0),
                    };
                case 2: //L
                    rotate = true;
                    currColor = Brushes.Orange;
                    return new Point[]
                    {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(-1,-1),
                    };
                case 3: // O
                    rotate = false;
                    currColor = Brushes.Yellow;
                    return new Point[]
                    {
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,0),
                        new Point(1,1),
                    };
                case 4:// S
                    rotate = true;
                    currColor = Brushes.LimeGreen;
                    return new Point[]
                    {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,-1),
                        new Point(1,0),
                    };
                case 5: // T
                    rotate = true;
                    currColor = Brushes.Purple;
                    return new Point[]
                    {
                        new Point(0,0),
                        new Point(1,0),
                        new Point(0,-1),
                        new Point(1,1),
                    };
                case 6: // Z
                    rotate = true;
                    currColor = Brushes.Red;
                    return new Point[]
                    {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,1),
                        new Point(1,1),
                    };

                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer Timer;
        private Board tetrisBoard;
        public MainWindow()
        {
            InitializeComponent();
        }

        void MainWindow_Initialized(object sender, EventArgs e)
        {
            //
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(GameTick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            GameStart();
        }

        private void GameStart()
        {
            MainGrid.Children.Clear();
            tetrisBoard = new Board(MainGrid);
            Timer.Start();
        }

        void GameTick(object sender, EventArgs e)
        {
            Scores.Content = "Score: " + tetrisBoard.getScore().ToString("0"); 
            Lines.Content = "Lines: " + tetrisBoard.getCompletedLines().ToString("0");
            GameOverText.Content = tetrisBoard.getGameOverText();
            tetrisBoard.CurrFigureMovDown();
         
        }

        private void GamePause()
        {
            if (Timer.IsEnabled) Timer.Stop();
            else Timer.Start();
        }

        private void KeyDownAction(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if(Timer.IsEnabled) tetrisBoard.CurrFigureMovLeft();
                    break;
                case Key.Right:
                    if (Timer.IsEnabled) tetrisBoard.CurrFigureMovRight();
                    break;
                case Key.Down:
                    if (Timer.IsEnabled) tetrisBoard.CurrFigureMovDown();
                    break;
                case Key.Up:
                    if (Timer.IsEnabled) tetrisBoard.CurrFigureMovRotate();
                    break;
                case Key.F2:
                    GameStart();
                    break;
                case Key.F3:
                    GamePause();
                    break;
                default:
                    break;
            }
        }
    }
}
