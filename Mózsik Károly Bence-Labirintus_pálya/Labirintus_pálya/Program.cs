namespace Labirintus_pálya;
using System;
using System.IO;
using System.Text;

internal class Program
{
    static int rows = 6;
    static int cols = 24;

    static char[,] map = new char[rows, cols];

    static int language = 0; // 0 = English, 1 = Magyar, 2 = Deutsch

    static char[] elements = {
        '.', '█',
        '═', '║',
        '╔', '╗',
        '╚', '╝',
        '╦', '╩',
        '╠', '╣',
        '╬'
        };

    static string[,] elementNames =
    {
        {
            "Empty",
            "Room",
            "Horizontal road",
            "Vertical road",
            "Top left corner",
            "Top right corner",
            "Bottom left corner",
            "Bottom right corner",
            "T junction down",
            "T junction up",
            "T junction right",
            "T junction left",
            "Crossroad"
        },
        {
            "Ures",
            "Terem",
            "Vizszintes jarat",
            "Fuggoleges jarat",
            "Bal felso sarok",
            "Jobb felso sarok",
            "Bal also sarok",
            "Jobb also sarok",
            "T elagazas lefele",
            "T elagazas felfele",
            "T elagazas jobbra",
            "T elagazas balra",
            "Keresztezodes"
        },
        {
            "Leer",
            "Raum",
            "Waagerechter Weg",
            "Senkrechter Weg",
            "Ecke oben links",
            "Ecke oben rechts",
            "Ecke unten links",
            "Ecke unten rechts",
            "T Kreuzung nach unten",
            "T Kreuzung nach oben",
            "T Kreuzung nach rechts",
            "T Kreuzung nach links",
            "Kreuzung"
        }
    };

    static int selectedElement = 0;
    static int cursorRow = 0;
    static int cursorCol = 0;

    static void fillMap()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                map[i, j] = '.';
            }
        }
    }


    static void DrawEditor()
    {
        Console.Clear();

        WriteColor("=================================\n", ConsoleColor.Cyan);
        WriteColor("   " + Text("LABYRINTH MAP EDITOR", "LABIRINTUS PALYASZERKESZTO", "LABYRINTH KARTENEDITOR") + "\n", ConsoleColor.Cyan);
        WriteColor("=================================\n\n", ConsoleColor.Cyan);

        WriteColor(Text("Language", "Nyelv", "Sprache"), ConsoleColor.Green);
        Console.WriteLine(": " + Text("English", "Magyar", "Deutsch"));
        Console.WriteLine();

        WriteColor(Text("Arrows", "Nyilak", "Pfeile"), ConsoleColor.Yellow);
        Console.Write(" - " + Text("Move", "Mozgas", "Bewegen") + "   ");

        WriteColor("A/D", ConsoleColor.Yellow);
        Console.Write(" - " + Text("Change element", "Elem valtas", "Element wechseln") + "   ");

        WriteColor("ENTER", ConsoleColor.Yellow);
        Console.Write(" - " + Text("Place", "Lerakas", "Platzieren") + "   ");

        WriteColor("M", ConsoleColor.Yellow);
        Console.Write(" - " + Text("Menu", "Menu", "Menu") + "   ");

        WriteColor("L", ConsoleColor.Yellow);
        Console.Write(" - " + Text("Language", "Nyelv", "Sprache") + "\n\n");

        WriteColor(Text("Current element: ", "Aktualis elem: ", "Aktuelles Element: "), ConsoleColor.Green);
        Console.Write(elementNames[language, selectedElement]);
        Console.Write(" (");
        Console.Write(elements[selectedElement]);
        Console.WriteLine(")\n");

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (i == cursorRow && j == cursorCol)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(map[i, j]);
                    Console.ResetColor();
                }
                else
                {
                    SetElementColor(map[i, j]);
                    Console.Write(map[i, j]);
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        WriteColor(Text("Room character: █\n", "Terem karakter: █\n", "Raum Zeichen: █\n"), ConsoleColor.Magenta);
        WriteColor(
            Text(
                "Exit: any road element on the edge of the map\n",
                "Kijarat: barmely jaratelem a palya szelen\n",
                "Ausgang: beliebiges Wegelement am Rand der Karte\n"
            ),
            ConsoleColor.Magenta
        );
    }

    static void WriteColor(string text, ConsoleColor color)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = prevColor;
    }

    static void SetElementColor(char c)
    {
        if (c == '.')
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        else if (c == '█')
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }
        else { Console.ForegroundColor = ConsoleColor.Cyan; }
    }

    static bool Menu()
    {
        Console.Clear();
        WriteColor("===== " + Text("MENU", "MENU", "MENU") + " =====\n\n", ConsoleColor.Magenta);

        WriteColor("1", ConsoleColor.Green);
        Console.WriteLine(" - " + Text("Continue editing", "Tovabb szerkesztes", "Weiter bearbeiten"));

        WriteColor("2", ConsoleColor.Yellow);
        Console.WriteLine(" - " + Text("Check and save", "Ellenorzes es mentes", "Prufen und speichern"));

        WriteColor("3", ConsoleColor.Red);
        Console.WriteLine(" - " + Text("Exit", "Kilepes", "Beenden"));

        ConsoleKey key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.D1)
        {
            return true;
        }
        if (key == ConsoleKey.D2)
        {
            SaveMap();
            Console.WriteLine();
            Console.WriteLine(Text("Press any key...", "Nyomjon meg egy billentyut...", "Drucken Sie eine Taste..."));
            Console.ReadKey(true);
            return true;

        }
        if (key == ConsoleKey.D3)
        {
            return false;

        }
        return true;
    }

    static void SaveMap()
    {
        Console.Clear();

        if (!isMapValid())
        {
            WriteColor(Text("ERROR!\n\n", "HIBA!\n\n", "FEHLER!\n\n"), ConsoleColor.Red);
            WriteColor(
                Text(
                    "The map cannot be saved.\n",
                    "A palya nem mentheto.\n",
                    "Die Karte kann nicht gespeichert werden.\n"
                ),
                ConsoleColor.Red
            );
            WriteColor(
                Text(
                    "There must be at least 1 room and 1 exit.\n",
                    "Legalabb 1 teremnek es 1 kijaratnak lennie kell.\n",
                    "Es muss mindestens 1 Raum und 1 Ausgang geben.\n"
                ),
                ConsoleColor.Red
            );
            return;
        }

        WriteColor(
            Text(
                "The map is valid.\n\n",
                "A palya megfelelo.\n\n",
                "Die Karte ist gultig.\n\n"
            ),
            ConsoleColor.Green
        );

        Console.CursorVisible = true;
        Console.Write(Text("File name: ", "Fajl neve: ", "Dateiname: "));
        string fileName = Console.ReadLine();
        Console.CursorVisible = false;

        if (string.IsNullOrWhiteSpace(fileName))
            fileName = "palya";

        if (!fileName.EndsWith(".txt"))
            fileName += ".txt";

        string[] lines = new string[rows];

        for (int i = 0; i < rows; i++)
        {
            string line = "";

            for (int j = 0; j < cols; j++)
            {
                line += map[i, j];
            }

            lines[i] = line;
        }

        File.WriteAllLines(fileName, lines, Encoding.UTF8);

        WriteColor(
            Text(
                "\nSuccessful save!\n",
                "\nSikeres mentes!\n",
                "\nErfolgreich gespeichert!\n"
            ),
            ConsoleColor.Green
        );

        Console.WriteLine(Text("File: ", "Fajl: ", "Datei: ") + fileName);
    }

    static bool isMapValid()
    {
        bool HasExit = false;
        bool HasDoor = false;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (map[i, j] == '█')
                {
                    HasDoor = true;
                }

                if (i == 0 || i == rows - 1 || j == 0 || j == cols - 1)
                {
                    if (IsRoad(map[i, j]))
                    {
                        HasExit = true;
                    }
                }
            }
        }

        return HasDoor && HasExit;
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        fillMap();

        bool running = true;

        while (running)
        {
            Console.CursorVisible = false;
            DrawEditor();
            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (cursorRow > 0)
                        cursorRow--;
                    break;

                case ConsoleKey.DownArrow:
                    if (cursorRow < rows - 1)
                        cursorRow++;
                    break;

                case ConsoleKey.LeftArrow:
                    if (cursorCol > 0)
                        cursorCol--;
                    break;

                case ConsoleKey.RightArrow:
                    if (cursorCol < cols - 1)
                        cursorCol++;
                    break;

                case ConsoleKey.A:
                    selectedElement--;
                    if (selectedElement < 0)
                        selectedElement = elements.Length - 1;
                    break;

                case ConsoleKey.D:
                    selectedElement++;
                    if (selectedElement >= elements.Length)
                        selectedElement = 0;
                    break;
                case ConsoleKey.Enter:
                    map[cursorRow, cursorCol] = elements[selectedElement];
                    break;

                case ConsoleKey.M:
                    running = Menu();
                    break;

                case ConsoleKey.L:
                    language++;

                    if (language > 2)
                    {
                        language = 0;
                    }
                    break;
            }

        }
    }

    static string Text(string english, string hungarian, string german)
    {
        if (language == 0)
        {
            return english;
        }
        else if (language == 1)
        {
            return hungarian;
        }
        else
        {
            return german;
        }
    }

    static bool IsRoad(char c)
    {
        return c == '═' ||
               c == '║' ||
               c == '╔' ||
               c == '╗' ||
               c == '╚' ||
               c == '╝' ||
               c == '╦' ||
               c == '╩' ||
               c == '╠' ||
               c == '╣' ||
               c == '╬';
    }
}