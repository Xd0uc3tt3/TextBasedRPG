using System;
using System.IO;

namespace TextBasedRPG
{
    class Program
    {
        static int maxHealth = 100;
        static int health = maxHealth;

        static int maxShield = 100;
        static int shield = maxShield;

        static int lives = 3;
        static bool isAlive = true;

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
                Console.WriteLine();
                ShowHud();

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

        static void ShowHud()
        {
            string characterName = "Brutus Jr";

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{characterName,-15}");

            string healthStatus;
            if (health >= 100)
            {
                healthStatus = "Perfect Health";
            }
            else if (health >= 90)
            {
                healthStatus = "Healthy";
            }
            else if (health >= 50)
            {
                healthStatus = "Hurt";
            }
            else if (health >= 10)
            {
                healthStatus = "Badly Hurt";
            }
            else
            {
                healthStatus = "Immediate Danger";
            }

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write($"{healthStatus,25}");


            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{("Health:" + health + "/" + maxHealth),20}");


            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{("Shield:" + shield + "/" + maxShield),20}");


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{("Lives:" + lives),10}");
            Console.ResetColor();
        }

        static void TakeDamage(int damageAmount)
        {
            if (shield > 0)
            {
                shield -= damageAmount;
                if (shield < 0)
                {
                    health += shield;
                    shield = 0;
                }
            }
            else
            {
                health -= damageAmount;
            }

            if (health <= 0)
                Revive();
        }

        static void Heal(int healAmount)
        {
            health += healAmount;
            if (health > maxHealth) health = maxHealth;
        }

        static void RegenerateShield(int regenAmount)
        {
            shield += regenAmount;
            if (shield > maxShield) shield = maxShield;
        }

        static void Revive()
        {
            if (lives > 0)
            {
                health = maxHealth;
                shield = maxShield;
                lives--;
            }
            else
            {
                isAlive = false;
            }
        }

    }
}

