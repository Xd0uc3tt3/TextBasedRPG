using System;
using System.IO;

namespace TextRPGMap
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            char[,] map = LoadMapFromFile("map.txt");
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            Console.SetWindowSize(Math.Min(cols + 2, 200), Math.Min(rows + 2, 50));
            Console.SetBufferSize(Math.Min(cols + 2, 200), Math.Min(rows + 2, 50));

            int playerRow = 0;
            int playerCol = 11;

            while (true)
            {
                Console.SetCursorPosition(0, 0);
                PrintMapWithPlayer(map, playerRow, playerCol);

                ConsoleKeyInfo key = Console.ReadKey(true);
                int newRow = playerRow;
                int newCol = playerCol;

                switch (key.Key)
                {
                    case ConsoleKey.W: newRow--; break;
                    case ConsoleKey.S: newRow++; break;
                    case ConsoleKey.A: newCol--; break;
                    case ConsoleKey.D: newCol++; break;
                    case ConsoleKey.Escape: return;
                }

                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                {
                    char tile = map[newRow, newCol];
                    if (tile != '^' && tile != '~')
                    {
                        playerRow = newRow;
                        playerCol = newCol;
                    }
                }
            }
        }

        static char[,] LoadMapFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            int cols = lines[0].Length;

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
                        Console.Write("@");
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

