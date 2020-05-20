using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Media;
using System.IO;
using System.Reflection;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace Snake
{
    struct Position
    {
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }


    class Program
    {
        // When snake eat food score will updated
        static int score = 0;

        // Score reach to win the game
        static int winScore = 10500;

        static int selected = 0;

        // Main Menu Listing
        // Eddie
        public static string MainMenu(List<string> menuList)
        {

            for (int i = 0; i < menuList.Count; i++)
            {

                if (selected == i)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(menuList[i]);
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(menuList[i]);
                    Console.ResetColor();
                }
            }

            //Arrow key to move move and select
            ConsoleKeyInfo press = Console.ReadKey(true);
            if (press.Key == ConsoleKey.UpArrow)
            {
                if ((selected - 1) < 0)
                {
                    selected = menuList.Count - 1;
                }
                else
                {
                    selected--;
                }
            }
            else if (press.Key == ConsoleKey.DownArrow)
            {
                if ((selected + 1) > menuList.Count - 1)
                {
                    selected = 0;
                }
                else
                {
                    selected++;
                }
            }

            else if (press.Key == ConsoleKey.Enter)
            {
                return menuList[selected];
            }
            else
            {
                return "";
            }

            Console.Clear();
            return "";
        }

        /// draw the food
        /// </summary>
        // Updating the score - Brandon
        static void UpdateScore()
        {
            score = score + 150;
            Console.ForegroundColor = ConsoleColor.Yellow;
            string scoretxt = "Score: " + score;
            int setscoreheight = (Console.WindowTop);
            int setscorewidth = ((Console.WindowWidth / 2) - 5);
            Console.SetCursorPosition(setscorewidth, setscoreheight);
            Console.Write(scoretxt);
        }

        // Addbackground music and die music ?
        public static void GameOverMusic()
        {
            SoundPlayer gameover = new SoundPlayer();
            gameover.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\die.wav";
            gameover.Play();
        }

        public static void WinMusic()
        {
            SoundPlayer winnings = new SoundPlayer();
            winnings.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\winning.wav";
            winnings.Play();
        }

        public void BackMusic()
        {
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\snake.wav";
            player.PlayLooping();
        }

        public static void MenuMusic()
        {
            SoundPlayer menusound = new SoundPlayer();
            menusound.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\mainmenu.wav";
            menusound.PlayLooping();
        }

        // draw the food function
        public static void DrawFood()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Write("\u2665\u2665");
        }

        //draw bonus food
        public static void DrawBonusFood()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("$");
        }

        // draw the obstacles 
        public static void DrawObs()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Write("\u2592");
        }

        // draw the snake body 
        public void SnakeBody()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("*");
        }

        // the direction of the snake in an array
        public void Dir(Position[] directions)
        {
            directions[0] = new Position(0, 1);
            directions[1] = new Position(0, -1);
            directions[2] = new Position(1, 0);
            directions[3] = new Position(-1, 0);
        }

        // generate the obstacle 
        public void RandomObstacles(List<Position> obstacles)
        {
            Random rand = new Random();
            obstacles.Add(new Position(rand.Next(0, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            obstacles.Add(new Position(rand.Next(0, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            obstacles.Add(new Position(rand.Next(0, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            obstacles.Add(new Position(rand.Next(0, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            obstacles.Add(new Position(rand.Next(0, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));

            foreach (Position obstacle in obstacles)
            {

                Console.SetCursorPosition(obstacle.col, obstacle.row);
                DrawObs();
            }
        }

        // indicate the user input 
        public void UserInput(ref int direction, byte right, byte left, byte down, byte up)
        {

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey(true);// fixed only errow key valid for the key input-- Lee Jun Yee
                if (userInput.Key == ConsoleKey.LeftArrow)
                {
                    if (direction != right) direction = left;
                }
                if (userInput.Key == ConsoleKey.RightArrow)
                {
                    if (direction != left) direction = right;
                }
                if (userInput.Key == ConsoleKey.UpArrow)
                {
                    if (direction != down) direction = up;
                }
                if (userInput.Key == ConsoleKey.DownArrow)
                {
                    if (direction != up) direction = down;
                }

               

            }
        }

        public void GenFood(ref Position food, Queue<Position> snakeElements, List<Position> obstacles)
        {
            Random randomNumbersGenerator = new Random();
            do
            {
                food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                    randomNumbersGenerator.Next(0, Console.WindowWidth));
            }

            while (snakeElements.Contains(food) || obstacles.Contains(food));
            Console.SetCursorPosition(food.col, food.row);
            DrawFood();
        }

        // generate new bonus food 
        public void GenBonusFood(ref Position bonusfood, Queue<Position> snakeElements, List<Position> obstacles)
        {

            Random randomNumbersGenerator = new Random();
            do
            {
                bonusfood = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                    randomNumbersGenerator.Next(0, Console.WindowWidth));
            }

            while (snakeElements.Contains(bonusfood) || obstacles.Contains(bonusfood));
            Console.SetCursorPosition(bonusfood.col, bonusfood.row);
            DrawBonusFood();
        }

        public void NewObstacle(ref Position food, Queue<Position> snakeElements, List<Position> obstacles)
        {
            Random randomNumbersGenerator = new Random();

            Position obstacle = new Position();
            do
            {
                obstacle = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                    randomNumbersGenerator.Next(0, Console.WindowWidth));
            }

            while (snakeElements.Contains(obstacle) || obstacles.Contains(obstacle) || (food.row == obstacle.row && food.col == obstacle.col));
            obstacles.Add(obstacle);
            Console.SetCursorPosition(obstacle.col, obstacle.row);
            DrawObs();
        }

        // Top center score
        public void ScoreText(string scoretxt, int x)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            int setheight = Console.WindowTop;
            int setwidth = ((Console.WindowWidth - scoretxt.Length) / 2);
            Console.SetCursorPosition(setwidth, setheight);
            Console.WriteLine(scoretxt);
        }

        // bonus score text 


        // Game over text at center of screen
        // Eddie
        public void GameOverText(string text, int x)
        {
            int setheight = ((Console.WindowHeight / 2) - x);
            int setwidth = ((Console.WindowWidth - text.Length) / 2);
            Console.SetCursorPosition(setwidth, setheight);
            Console.WriteLine(text);
        }

        // only exit when enter
        // Eddie
        public void EnterExit(string exittxt, int x)
        {
            int setheight = ((Console.WindowHeight / 2) + 2);
            int setwidth = ((Console.WindowWidth - exittxt.Length) / 2);
            Console.SetCursorPosition(setwidth, setheight);
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
            System.Environment.Exit(0);

        }

        // Draw player's name
        // Eddie
        public void PNames(string plname)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            int setheight = Console.WindowTop + 1 ;
            int setwidth = ((Console.WindowWidth - plname.Length) / 2); ;
            Console.SetCursorPosition(setwidth, setheight);
            Console.WriteLine(plname);
        }
        // draw time
        public void TPlace(string tplace)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            int setheight = Console.WindowTop + 2;
            int setwidth = ((Console.WindowWidth - tplace.Length) / 2); ;
            Console.SetCursorPosition(setwidth, setheight);
            Console.WriteLine(tplace);
        }

        // create player's score txt file
        // Eddie
        public static void Scoretxtfile()
        {
            string Scorefile = "Player's Score.txt";
            if (System.IO.File.Exists(Scorefile))
            {
                return;
            }
            else
            {
                string hrname = "Name \t \t";
                string hrscore = "Score \n";
                string hrline = "------------------------------\n";
                System.IO.File.WriteAllText("Player's Score.txt", hrname + hrscore + hrline);
            }
            return;
            
        }

        // Enter player's name
        // Check the Input is not empty
        // Eddie
        public static string InputName(string Input)
        {
            string PlayerName = "";
            do
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                int setheight = ((Console.WindowHeight / 2) - 1);
                int setwidth = ((Console.WindowWidth - Input.Length) / 2); ;
                Console.SetCursorPosition(setwidth, setheight);
                Console.WriteLine(Input);

                int readheight = (Console.WindowHeight / 2);
                int readwidth = ((Console.WindowWidth - Input.Length) / 2) + 6 ;
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(readwidth, readheight);
                PlayerName = Console.ReadLine();
                if (string.IsNullOrEmpty(PlayerName))
                {
                    Console.Clear();
                    int reheight = ((Console.WindowHeight / 2) - 1);
                    int rewidth = ((Console.WindowWidth - Input.Length) / 2); ;
                    Console.SetCursorPosition(rewidth, reheight);
                    Console.WriteLine("Please Enter Your Name");
                }
            } while (string.IsNullOrEmpty(PlayerName));
            return PlayerName;
        }

        //Help text to center of the screen
        public void Helptxt(string text, int x)
        {
            int setheight = ((Console.WindowHeight / 2) - x);
            int setwidth = ((Console.WindowWidth - text.Length) / 2);
            Console.SetCursorPosition(setwidth, setheight);
            Console.WriteLine(text);
        }


        // Main
        static void Main(string[] args)
        {

            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;
            int lastFoodTime = 0;
            int lastBonusTime = 0;
            int foodDissapearTime = 15000;
            int BonusFoodDisappearTime = 15000 / 3;
            int negativePoints = 0;
            Position[] directions = new Position[4];

            Program snake = new Program();
            //play main menu music
            MenuMusic();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
           
                

                List<string> menuListing = new List<string>()

            {
                "Start Game",
                "Previous Score",
                "Help",
                "Exit"
            };

            Console.CursorVisible = false;
            while (true)
            {

                Scoretxtfile();

                string selectedList = MainMenu(menuListing);
                if (selectedList == "Start Game")
                {
                    Console.Clear();

                    string PlayersName = InputName("Please Enter Your Name");

                    Console.Clear();

                    {
                        // play background music
                        snake.BackMusic();
                        // indicate direction with the index of array
                        snake.Dir(directions);
                        // reset the obstacle posion
                        List<Position> obstacles = new List<Position>();
                        snake.RandomObstacles(obstacles);

                        double sleepTime = 100;
                        int direction = right;
                        Random randomNumbersGenerator = new Random();
                        Console.BufferHeight = Console.WindowHeight;
                        lastFoodTime = Environment.TickCount;
                        lastBonusTime = Environment.TickCount;

                        // Set Score to the top right
                        string scoretxt = "Score: 0";
                        snake.ScoreText(scoretxt, -5);
                        snake.PNames("Name: " + PlayersName);
                       


                        Queue<Position> snakeElements = new Queue<Position>();
                        for (int i = 0; i <= 3; i++)
                        {
                            snakeElements.Enqueue(new Position(0, i));
                        }

                        Position food = new Position();
                        snake.GenFood(ref food, snakeElements, obstacles);

                        Position bonusfood = new Position();
                        snake.GenBonusFood(ref bonusfood, snakeElements, obstacles);

                        lastFoodTime = Environment.TickCount;
                        lastBonusTime = Environment.TickCount;

                        foreach (Position position in snakeElements)
                        {
                            Console.SetCursorPosition(position.col, position.row);
                            snake.SnakeBody();
                        }

                        while (true)
                        {
                            TimeSpan timeSpan = TimeSpan.FromSeconds(Convert.ToInt32(stopwatch.Elapsed.TotalSeconds));
                            snake.TPlace("Time: " + timeSpan);
                            negativePoints++;

                            // old check user input
                            snake.UserInput(ref direction, right, left, down, up);

                            Position snakeHead = snakeElements.Last();
                            Position nextDirection = directions[direction];

                            Position snakeNewHead = new Position(snakeHead.row + nextDirection.row,
                                snakeHead.col + nextDirection.col);

                            if (snakeNewHead.col < 0) snakeNewHead.col = Console.WindowWidth - 1;
                            if (snakeNewHead.row < 0) snakeNewHead.row = Console.WindowHeight - 1;
                            if (snakeNewHead.row >= Console.WindowHeight) snakeNewHead.row = 0;
                            if (snakeNewHead.col >= Console.WindowWidth) snakeNewHead.col = 0;

                            if (snakeElements.Contains(snakeNewHead) || obstacles.Contains(snakeNewHead))
                            {
                                //record the player's name and score in txt file
                                //Eddie
                                string playerText = PlayersName + "\t\t";
                                using (StreamWriter w = new StreamWriter("Player's Score.txt", true))
                                {
                                    w.WriteLine(playerText + score);
                                }

                                //----------Play Game Over Music------------
                                GameOverMusic();

                                //--------------------Game over text in red color----------------
                                Console.ForegroundColor = ConsoleColor.Red;

                                //------------first line--------------
                                string gameovertxt = "Gameover!";
                                snake.GameOverText(gameovertxt, 1);

                                int userPoints = (snakeElements.Count - 6) * 100 - negativePoints;
                                //if (userPoints < 0) userPoints = 0;
                                userPoints = Math.Max(userPoints, 0);

                                //------------second line--------------
                                string pointtxt = PlayersName + " your points are: " + score;
                                snake.GameOverText(pointtxt, 0);

                                string timertxt = PlayersName +" your total play time is :"+ timeSpan;
                                snake.GameOverText(timertxt, -1);

                                //------------third line--------------
                                string exittxt = "Press Enter to Exit";
                                snake.GameOverText(exittxt, -2);

                                ////------------exit line--------------
                                string continuetxt = "Press any key to continue . . . .";
                                snake.EnterExit(continuetxt, -3);
                            }

                            Console.SetCursorPosition(snakeHead.col, snakeHead.row);
                            snake.SnakeBody();

                            snakeElements.Enqueue(snakeNewHead);
                            Console.SetCursorPosition(snakeNewHead.col, snakeNewHead.row);
                            Console.ForegroundColor = ConsoleColor.Gray;
                            if (direction == right) Console.Write(">");
                            if (direction == left) Console.Write("<");
                            if (direction == up) Console.Write("^");
                            if (direction == down) Console.Write("v");


                            if (snakeNewHead.col == food.col && snakeNewHead.row == food.row)
                            {
                                // feeding the snake
                                do
                                {
                                    food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                                        randomNumbersGenerator.Next(0, Console.WindowWidth));
                                }

                                while (snakeElements.Contains(food) || obstacles.Contains(food));
                                lastFoodTime = Environment.TickCount;
                                Console.SetCursorPosition(food.col, food.row);
                                DrawFood();
                                Position obstacle = new Position();
                                
                                sleepTime--;
                                UpdateScore();


                                if (score >= winScore)
                                {
                                    //record the player's name and score in txt file 
                                    //Eddie
                                    string playerText = PlayersName + "\t\t";
                                    using (StreamWriter w = new StreamWriter("Player's Score.txt", true))
                                    {
                                        w.WriteLine(playerText + score);
                                    }

                                    // won text in green color
                                    Console.ForegroundColor = ConsoleColor.Green;

                                    //play winning music
                                    WinMusic();

                                    //------------first line--------------
                                    string wintxt = "You Won!";
                                    snake.GameOverText(wintxt, 1);

                                    int userPoints = (snakeElements.Count - 6) * 100 - negativePoints;
                                    //if (userPoints < 0) userPoints = 0;
                                    userPoints = Math.Max(userPoints, 0);

                                    //------------second line--------------
                                    string pointtxt = "Your points are: " + score;
                                    snake.GameOverText(pointtxt, 0);

                                    //------------third line--------------
                                    string exittxt = "Press Enter to Exit";
                                    snake.GameOverText(exittxt, -1);

                                    ////------------exit line--------------
                                    string continuetxt = "Press any key to continue . . .";
                                    snake.EnterExit(continuetxt, 0);
                                }

                                do
                                {
                                    obstacle = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                                        randomNumbersGenerator.Next(0, Console.WindowWidth));
                                }
                                while (snakeElements.Contains(obstacle) ||
                                   obstacles.Contains(obstacle) ||
                                   (food.row != obstacle.row && food.col != obstacle.row));
                                obstacles.Add(obstacle);
                                Console.SetCursorPosition(obstacle.col, obstacle.row);
                                DrawObs();

                            }

                            if (snakeNewHead.col == bonusfood.col && snakeNewHead.row == bonusfood.row)
                            {
                                score += 150;
                                UpdateScore();

                                // If reach score == winScore(10500), WIN   
                                if (score >= winScore)
                                {
                                    //record the player's name and score in txt file 
                                    //Eddie
                                    string playerText = PlayersName + "\t\t";
                                    using (StreamWriter w = new StreamWriter("Player's Score.txt", true))
                                    {
                                        w.WriteLine(playerText + score);
                                    }

                                    // won text in green color
                                    Console.ForegroundColor = ConsoleColor.Green;

                                    //play winning music
                                    WinMusic();

                                    //------------first line--------------
                                    string wintxt = "You Won!";
                                    snake.GameOverText(wintxt, 1);

                                    int userPoints = (snakeElements.Count - 6) * 100 - negativePoints;
                                    //if (userPoints < 0) userPoints = 0;
                                    userPoints = Math.Max(userPoints, 0);

                                    //------------second line--------------
                                    string pointtxt = "Your points are: " + score;
                                    snake.GameOverText(pointtxt, 0);

                                    //------------third line--------------
                                    string exittxt = "Press Enter to Exit";
                                    snake.GameOverText(exittxt, -1);

                                    ////------------exit line--------------
                                    string continuetxt = "Press any key to continue . . .";
                                    snake.EnterExit(continuetxt, 0);
                                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
                                    Console.Clear();

                                }

                            }
                            else
                            {
                                // moving...
                                Position last = snakeElements.Dequeue();
                                Console.SetCursorPosition(last.col, last.row);
                                Console.Write(" ");
                            }

                            if (Environment.TickCount - lastFoodTime >= foodDissapearTime)
                            {
                                negativePoints = negativePoints + 50;
                                Console.SetCursorPosition(food.col, food.row);
                                Console.Write(" ");
                                snake.GenFood(ref food, snakeElements, obstacles);
                                lastFoodTime = Environment.TickCount;
                            }

                            // bomus food disapear 
                            else if (Environment.TickCount - lastBonusTime >= BonusFoodDisappearTime)
                            {
                                negativePoints = negativePoints + 50;
                                Console.SetCursorPosition(bonusfood.col, bonusfood.row);
                                Console.Write(" ");
                                snake.GenBonusFood(ref bonusfood, snakeElements, obstacles);
                                lastBonusTime = Environment.TickCount;
                            }

                            Console.SetCursorPosition(food.col, food.row);
                            DrawFood();

                            sleepTime -= 0.01;

                            Thread.Sleep((int)sleepTime);           
                        }
                    }
                 }


                else if (selectedList == "Previous Score")
                {
                    Console.Clear();

                    //read the text file
                    string[] lines = System.IO.File.ReadAllLines("Player's Score.txt");

                    //Display the file using foreach loop.
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    System.Console.WriteLine("\n \t Previous Player \n");
                    foreach (string line in lines)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("\t" + line);
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n \t Press Enter to Exit");
                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
                    Console.Clear();

                }

                else if (selectedList == "Help")
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    snake.Helptxt("Help", 5);
                    Console.ForegroundColor = ConsoleColor.White;
                    snake.Helptxt("You must enter your name before the game start", 3);
                    snake.Helptxt("To move Upward press Up Arrow Key [^]", 2);
                    snake.Helptxt("To move Downward press Down Arrow Key [v]", 1);
                    snake.Helptxt("To move Left press Left Arrow Key [<]", 0);
                    snake.Helptxt("To move Right press Right Arrow Key [>]", -1);
                    Console.ForegroundColor = ConsoleColor.Red;
                    snake.Helptxt("Press Enter to Exit", -3);
                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
                    Console.Clear();
                }
                else if (selectedList == "Exit")
                {
                    System.Environment.Exit(0);
                }

                }

            }
        }
    }
