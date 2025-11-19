using System;
using System.IO;

namespace TextRPGMap
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Load map from file
            char[,] map = LoadMapFromFile("map.txt");
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            // Set console buffer and window size to match map
            Console.SetWindowSize(Math.Min(cols + 2, 200), Math.Min(rows + 2, 50));
            Console.SetBufferSize(Math.Min(cols + 2, 200), Math.Min(rows + 2, 50));

            // Player start position
            int playerRow = 0;
            int playerCol = 0;

            while (true)
            {
                Console.SetCursorPosition(0, 0);
                PrintMapWithPlayer(map, playerRow, playerCol);

                // Handle movement
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.W:
                        if (playerRow > 0) playerRow--;
                        break;
                    case ConsoleKey.S:
                        if (playerRow < rows - 1) playerRow++;
                        break;
                    case ConsoleKey.A:
                        if (playerCol > 0) playerCol--;
                        break;
                    case ConsoleKey.D:
                        if (playerCol < cols - 1) playerCol++;
                        break;
                    case ConsoleKey.Escape:
                        return; // exit
                }
            }
        }

        static char[,] LoadMapFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File '{filePath}' not found!");
                Environment.Exit(1);
            }

            string[] lines = File.ReadAllLines(filePath);
            int cols = lines[0].Length;
            foreach (var line in lines)
            {
                if (line.Length != cols)
                {
                    Console.WriteLine("Error: All rows in map.txt must have the same number of characters.");
                    Environment.Exit(1);
                }
            }

            int rows = lines.Length;
            char[,] map = new char[rows, cols];

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    map[r, c] = lines[r][c];

            return map;
        }

        static void PrintMapWithPlayer(char[,] map, int playerRow, int playerCol)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            Console.Write("+");
            for (int i = 0; i < cols; i++) Console.Write("-");
            Console.WriteLine("+");

            for (int r = 0; r < rows; r++)
            {
                Console.Write("|");
                for (int c = 0; c < cols; c++)
                {
                    if (r == playerRow && c == playerCol)
                        Console.Write("@"); // player
                    else
                        Console.Write(map[r, c]);
                }
                Console.WriteLine("|");
            }

            Console.Write("+");
            for (int i = 0; i < cols; i++) Console.Write("-");
            Console.WriteLine("+");

            Console.WriteLine("Use WASD to move. ESC to exit.");
        }
    }
}
