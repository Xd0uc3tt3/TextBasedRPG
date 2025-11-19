using System;
using System.IO;

namespace TextBasedRPG
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            char[,] map = LoadMapFromFile("map.txt");

            PrintScaledMap(map, 3);
        }

        static char[,] LoadMapFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            int cols = lines[0].Length;

            int rows = lines.Length;
            char[,] map = new char[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    map[r, c] = lines[r][c];
                }
            }

            return map;
        }

        static void PrintScaledMap(char[,] map, int scale)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            DrawBorder(cols * scale);

            for (int r = 0; r < rows; r++)
            {
                for (int v = 0; v < scale; v++)
                {
                    Console.Write("|");
                    for (int c = 0; c < cols; c++)
                    {
                        for (int h = 0; h < scale; h++)
                        {
                            Console.Write(map[r, c]);
                        }
                    }
                    Console.WriteLine("|");
                }
            }

            DrawBorder(cols * scale);
        }

        static void DrawBorder(int length)
        {
            Console.Write("+");
            for (int i = 0; i < length; i++)
                Console.Write("-");
            Console.WriteLine("+");
        }
    }
}

