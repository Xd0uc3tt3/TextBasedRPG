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

        // Enemy stats
        static int enemyMaxHealth = 50;
        static int enemyHealth = enemyMaxHealth;
        static int enemyRow = 5;
        static int enemyCol = 8;
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            char[,] map = LoadMapFromFile("map.txt");
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            Console.SetWindowSize(Math.Min(cols + 2, 200), Math.Min(rows + 8, 50));
            Console.SetBufferSize(Math.Min(cols + 2, 200), Math.Min(rows + 8, 50));

            int playerRow = 0;
            int playerCol = 11;

            while (isAlive)
            {
                // Clear previous messages
                ClearMessageLine(rows + 5);
                ClearMessageLine(rows + 6);

                // Draw map and HUD
                Console.SetCursorPosition(0, 0);
                PrintMapWithPlayerAndEnemy(map, playerRow, playerCol);
                ShowHud();

                // Player attacks enemy if on the same tile
                if (playerRow == enemyRow && playerCol == enemyCol && enemyHealth > 0)
                {
                    enemyHealth -= 10; // player deals 10 damage
                    Console.SetCursorPosition(0, rows + 5);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("You hit the enemy for 10 damage!");
                    Console.ResetColor();
                    if (enemyHealth <= 0)
                    {
                        enemyHealth = 0;
                        Console.SetCursorPosition(0, rows + 6);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Enemy defeated!");
                        Console.ResetColor();
                    }
                }

                // Enemy moves
                MoveEnemy(map, rows, cols, playerRow, playerCol);

                // Enemy damages player if on same tile
                if (playerRow == enemyRow && playerCol == enemyCol && enemyHealth > 0)
                {
                    Console.SetCursorPosition(0, rows + 6);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Enemy hits you for 5 damage!");
                    Console.ResetColor();
                    TakeDamage(5);
                }

                // Player movement
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

            Console.Clear();
            Console.WriteLine("Game Over!");
        }

        static void ClearMessageLine(int line)
        {
            Console.SetCursorPosition(0, line);
            Console.Write(new string(' ', 80)); // clear line with spaces
        }

        static void MoveEnemy(char[,] map, int rows, int cols, int playerRow, int playerCol)
        {
            if (enemyHealth <= 0) return;

            int dir = rnd.Next(0, 4);
            int newRow = enemyRow;
            int newCol = enemyCol;

            switch (dir)
            {
                case 0: newRow--; break;
                case 1: newRow++; break;
                case 2: newCol--; break;
                case 3: newCol++; break;
            }

            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
            {
                char tile = map[newRow, newCol];
                if (tile != '^' && tile != '~' && !(newRow == playerRow && newCol == playerCol))
                {
                    enemyRow = newRow;
                    enemyCol = newCol;
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

        static void PrintMapWithPlayerAndEnemy(char[,] map, int playerRow, int playerCol)
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
                    else if (r == enemyRow && c == enemyCol && enemyHealth > 0)
                        Console.Write("E");
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
                healthStatus = "Perfect Health";
            else if (health >= 90)
                healthStatus = "Healthy";
            else if (health >= 50)
                healthStatus = "Hurt";
            else if (health >= 10)
                healthStatus = "Badly Hurt";
            else
                healthStatus = "Immediate Danger";

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write($"{healthStatus,25}");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{("Health:" + health + "/" + maxHealth),20}");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{("Shield:" + shield + "/" + maxShield),20}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{("Lives:" + lives),10}");

            // Enemy HUD
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Enemy Health: {enemyHealth}/{enemyMaxHealth}");
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
