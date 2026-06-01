using System;
using System.Collections.Generic;
using System.IO;

namespace LabirintusJatek
{
    class Program
    {
        static bool english = false;

        static string gameTitle;
        static string chooseMode;
        static string normalModeText;
        static string hardModeText;
        static string stepsText;
        static string roomsText;
        static string movementText;
        static string saveLoadText;
        static string exitQuestionText;
        static string gameCompletedText;
        static string invalidMapText;
        static string directionsText;
        static string roomFoundText;

        static char[,] map;

        static int playerRow;
        static int playerCol;

        static int steps = 0;

        static bool hardMode = false;
        static bool[,] discovered;

        static string message = "";

        static HashSet<string> foundRooms = new HashSet<string>();

        static List<char> roads = new List<char>()
        {
            '╬','═','╦','╩','║','╣','╠','╗','╝','╚','╔'
        };

        static int GetRoomNumber(char[,] map)
        {
            int db = 0;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == '█')
                    {
                        db++;
                    }
                }
            }

            return db;
        }

        static void SetLanguage()
        {
            if (english)
            {
                gameTitle = "LABYRINTH GAME";
                chooseMode = "Choose game mode:";
                normalModeText = "1 - Normal mode";
                hardModeText = "2 - Hard mode";

                stepsText = "Steps";
                roomsText = "Discovered rooms";
                movementText = "Movement: W A S D";
                saveLoadText = "Save: G   Load: H";

                exitQuestionText = "Are you sure you want to leave the labyrinth? Move there again.";

                gameCompletedText = "Game completed!";

                invalidMapText = "The map contains invalid characters!";

                directionsText = "Possible directions";

                roomFoundText = "New room discovered!";
            }
            else
            {
                gameTitle = "LABIRINTUS JÁTÉK";
                chooseMode = "Válassz játékmódot:";
                normalModeText = "1 - Normál mód";
                hardModeText = "2 - Nehéz mód";

                stepsText = "Lépések száma";
                roomsText = "Felfedezett termek";

                movementText = "Mozgás: W A S D";
                saveLoadText = "Mentés: G   Betöltés: H";

                exitQuestionText = "Biztos el akarod hagyni a labirintust? Menj arra újra.";

                gameCompletedText = "A játék teljesítve!";

                invalidMapText = "A pálya hibás karaktert tartalmaz!";

                directionsText = "Lehetséges irányok";

                roomFoundText = "Új terem felfedezve!";
            }

            Console.Title = gameTitle;
        }

        static string GetPossibleDirections()
        {
            string directions = "";

            if (CanMoveBetween(playerRow, playerCol, playerRow - 1, playerCol))
                directions += "W ";

            if (CanMoveBetween(playerRow, playerCol, playerRow + 1, playerCol))
                directions += "S ";

            if (CanMoveBetween(playerRow, playerCol, playerRow, playerCol - 1))
                directions += "A ";

            if (CanMoveBetween(playerRow, playerCol, playerRow, playerCol + 1))
                directions += "D ";

            return directions.Trim();
        }

        static bool IsInvalidMap(char[,] map)
        {
            List<char> valid = new List<char>()
            {
                '.','█',
                '╬','═','╦','╩','║','╣','╠','╗','╝','╚','╔'
            };

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (!valid.Contains(map[i, j]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static bool IsOpenUp(char c)
        {
            return c == '║' || c == '╬' || c == '╣' || c == '╠' ||
                   c == '╩' || c == '╝' || c == '╚';
        }

        static bool IsOpenDown(char c)
        {
            return c == '║' || c == '╬' || c == '╣' || c == '╠' ||
                   c == '╦' || c == '╗' || c == '╔';
        }

        static bool IsOpenLeft(char c)
        {
            return c == '═' || c == '╬' || c == '╦' || c == '╩' ||
                   c == '╣' || c == '╗' || c == '╝';
        }

        static bool IsOpenRight(char c)
        {
            return c == '═' || c == '╬' || c == '╦' || c == '╩' ||
                   c == '╠' || c == '╔' || c == '╚';
        }

        static bool CanMoveBetween(int r1, int c1, int r2, int c2)
        {
            if (r2 < 0 || c2 < 0 || r2 >= map.GetLength(0) || c2 >= map.GetLength(1))
            {
                char current = map[r1, c1];

                if (r2 < r1 && IsOpenUp(current))
                    return true;

                if (r2 > r1 && IsOpenDown(current))
                    return true;

                if (c2 < c1 && IsOpenLeft(current))
                    return true;

                if (c2 > c1 && IsOpenRight(current))
                    return true;

                return false;
            }

            char a = map[r1, c1];
            char b = map[r2, c2];

            if (b == '█')
            {
                if (r2 == r1 - 1)
                    return IsOpenUp(a);

                if (r2 == r1 + 1)
                    return IsOpenDown(a);

                if (c2 == c1 - 1)
                    return IsOpenLeft(a);

                if (c2 == c1 + 1)
                    return IsOpenRight(a);
            }

            if (a == '█')
            {
                if (!roads.Contains(b))
                    return false;

                if (r2 == r1 - 1)
                    return IsOpenDown(b);

                if (r2 == r1 + 1)
                    return IsOpenUp(b);

                if (c2 == c1 - 1)
                    return IsOpenRight(b);

                if (c2 == c1 + 1)
                    return IsOpenLeft(b);

                return false;
            }

            if (!roads.Contains(a) || !roads.Contains(b))
                return false;

            if (r2 == r1 - 1)
                return IsOpenUp(a) && IsOpenDown(b);

            if (r2 == r1 + 1)
                return IsOpenDown(a) && IsOpenUp(b);

            if (c2 == c1 - 1)
                return IsOpenLeft(a) && IsOpenRight(b);

            if (c2 == c1 + 1)
                return IsOpenRight(a) && IsOpenLeft(b);

            return false;
        }

        static char[,] LoadMap(string file)
        {
            string[] lines = File.ReadAllLines(file);

            int rows = lines.Length;
            int cols = lines[0].Length;

            char[,] tomb = new char[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tomb[i, j] = lines[i][j];
                }
            }

            return tomb;
        }

        static void Discover()
        {
            discovered[playerRow, playerCol] = true;

            if (!hardMode)
            {
                if (playerRow > 0)
                    discovered[playerRow - 1, playerCol] = true;

                if (playerRow < map.GetLength(0) - 1)
                    discovered[playerRow + 1, playerCol] = true;

                if (playerCol > 0)
                    discovered[playerRow, playerCol - 1] = true;

                if (playerCol < map.GetLength(1) - 1)
                    discovered[playerRow, playerCol + 1] = true;
            }
        }

        static void Draw()
        {
            Console.Clear();
            Console.WriteLine();

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (hardMode && !discovered[i, j])
                    {
                        Console.Write(' ');
                    }
                    else if (i == playerRow && j == playerCol)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(map[i, j]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(map[i, j]);
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{stepsText}: {steps}");
            Console.WriteLine($"{roomsText}: {foundRooms.Count}/{GetRoomNumber(map)}");
            Console.ResetColor();

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(movementText);
            Console.WriteLine(saveLoadText);
            Console.ResetColor();
            Console.WriteLine($"{directionsText}: {GetPossibleDirections()}");

            if (message != "")
            {
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        static bool exitQuestion = false;

        static void Move(int dr, int dc)
        {
            message = "";
            int newR = playerRow + dr;
            int newC = playerCol + dc;

            bool kilepes = false;

            if (newR < 0 && IsOpenUp(map[playerRow, playerCol]))
                kilepes = true;

            if (newR >= map.GetLength(0) && IsOpenDown(map[playerRow, playerCol]))
                kilepes = true;

            if (newC < 0 && IsOpenLeft(map[playerRow, playerCol]))
                kilepes = true;

            if (newC >= map.GetLength(1) && IsOpenRight(map[playerRow, playerCol]))
                kilepes = true;

            if (kilepes)
            {
                if (!exitQuestion)
                {
                    message = exitQuestionText;

                    exitQuestion = true;
                    return;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine(gameCompletedText);
                    Console.WriteLine($"{stepsText}: {steps}");
                    Console.WriteLine($"{roomsText}: {foundRooms.Count}/{GetRoomNumber(map)}");
                    Environment.Exit(0);
                }
            }

            exitQuestion = false;

            if (CanMoveBetween(playerRow, playerCol, newR, newC))
            {
                playerRow = newR;
                playerCol = newC;

                if (hardMode)
                {
                    Discover();
                }

                steps++;

                if (map[playerRow, playerCol] == '█')
                {
                    string room = playerRow + ":" + playerCol;

                    if (!foundRooms.Contains(room))
                    {
                        foundRooms.Add(room);
                        message = roomFoundText;
                    }
                }
            }
        }

        static void SaveGame()
        {
            using (StreamWriter sw = new StreamWriter("save.txt"))
            {
                sw.WriteLine(playerRow);
                sw.WriteLine(playerCol);
                sw.WriteLine(steps);
                sw.WriteLine(hardMode);

                foreach (string room in foundRooms)
                {
                    sw.WriteLine(room);
                }
            }

            message = english ? "Game saved!" : "Játék elmentve!";
        }

        static void LoadGame()
        {
            if (!File.Exists("save.txt"))
            {
                message = english ? "No save found!" : "Nincs mentés!";
                return;
            }

            string[] lines = File.ReadAllLines("save.txt");

            playerRow = int.Parse(lines[0]);
            playerCol = int.Parse(lines[1]);
            steps = int.Parse(lines[2]);
            hardMode = bool.Parse(lines[3]);

            foundRooms.Clear();

            for (int i = 4; i < lines.Length; i++)
            {
                foundRooms.Add(lines[i]);
            }

            if (hardMode)
            {
                discovered = new bool[map.GetLength(0), map.GetLength(1)];

                foreach (string room in foundRooms)
                {
                    string[] p = room.Split(':');

                    int r = int.Parse(p[0]);
                    int c = int.Parse(p[1]);

                    discovered[r, c] = true;
                }

                Discover();
            }

            message = english ? "Game loaded!" : "Játék betöltve!";
        }

        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Language / Nyelv");
            Console.WriteLine();
            Console.WriteLine("1 - Hungarian (Magyar)");
            Console.WriteLine("2 - English (Angol)");
            Console.WriteLine();

            ConsoleKeyInfo langKey = Console.ReadKey();

            if (langKey.Key == ConsoleKey.D2 ||
                langKey.Key == ConsoleKey.NumPad2)
            {
                english = true;
            }

            SetLanguage();

            Console.Clear();

            Console.WriteLine();
            Console.WriteLine(chooseMode);
            Console.WriteLine();
            Console.WriteLine(normalModeText);
            Console.WriteLine(hardModeText);
            Console.WriteLine();

            ConsoleKeyInfo modeKey = Console.ReadKey();
            if (modeKey.Key == ConsoleKey.D2 || modeKey.Key == ConsoleKey.NumPad2)
            {
                hardMode = true;
            }

            map = LoadMap("palya.txt");

            if (IsInvalidMap(map))
            {
                Console.WriteLine(invalidMapText);
                return;
            }

            playerRow = 1;
            playerCol = 0;

            discovered = new bool[map.GetLength(0), map.GetLength(1)];

            Discover();

            while (true)
            {
                Draw();

                ConsoleKeyInfo key = Console.ReadKey();

                if (key.Key == ConsoleKey.W)
                    Move(-1, 0);

                if (key.Key == ConsoleKey.S)
                    Move(1, 0);

                if (key.Key == ConsoleKey.A)
                    Move(0, -1);

                if (key.Key == ConsoleKey.D)
                    Move(0, 1);

                if (key.Key == ConsoleKey.G)
                    SaveGame();

                if (key.Key == ConsoleKey.H)
                    LoadGame();
            }
        }
    }
}