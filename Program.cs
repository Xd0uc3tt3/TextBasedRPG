using System;
using System.IO;

namespace TextBasedRPG
{
    class Program
    {
        // Player stats
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
            // Console setup
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Load map
            char[,] map = LoadMapFromFile("map.txt");
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            // Window and buffer size
            Console.SetWindowSize(Math.Min(cols + 2, 200), Math.Min(rows + 8, 50));
            Console.SetBufferSize(Math.Min(cols + 2, 200), Math.Min(rows + 8, 50));

            // Player starting position
            int playerRow = 0;
            int playerCol = 11;

            // Main game loop
            while (isAlive)
            {
                // Clear previous messages
                ClearMessageLine(rows + 5);
                ClearMessageLine(rows + 6);

                // Draw map and HUD
                Console.SetCursorPosition(0, 0);
                PrintMapWithPlayerAndEnemy(map, playerRow, playerCol);
                ShowHud();

                // Player attacks enemy
                if (playerRow == enemyRow && playerCol == enemyCol && enemyHealth > 0)
                {
                    enemyHealth -= 10;
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

                // Enemy AI movement
                MoveEnemy(map, rows, cols, playerRow, playerCol);

                // Enemy attacks player
                if (playerRow == enemyRow && playerCol == enemyCol && enemyHealth > 0)
                {
                    Console.SetCursorPosition(0, rows + 6);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Enemy hits you for 5 damage!");
                    Console.ResetColor();
                    TakeDamage(5);
                }

                // Player movement input
                ConsoleKeyInfo key = Console.ReadKey(true);
                int newRow = playerRow;
                int newCol = playerCol;

                switch (key.Key)
                {
                    case ConsoleKey.W: newRow--; break; // move up
                    case ConsoleKey.S: newRow++; break; // move down
                    case ConsoleKey.A: newCol--; break; // move left
                    case ConsoleKey.D: newCol++; break; // move right
                    case ConsoleKey.Escape: return;     // exit
                }

                // Check if move is valid
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

            // Game over
            Console.Clear();
            Console.WriteLine("Game Over!");
        }

        // Clear a line in the console
        static void ClearMessageLine(int line)
        {
            Console.SetCursorPosition(0, line);
            Console.Write(new string(' ', 80)); // clear line with spaces
        }

        // Enemy movement logic
        static void MoveEnemy(char[,] map, int rows, int cols, int playerRow, int playerCol)
        {
            if (enemyHealth <= 0) return;

            int newRow = enemyRow;
            int newCol = enemyCol;

            // Determine behavior based on enemy health
            bool chasePlayer = enemyHealth > enemyMaxHealth / 2; // chase if health > 50%

            if (chasePlayer)
            {
                // Move towards player
                if (playerRow < enemyRow) newRow--;
                else if (playerRow > enemyRow) newRow++;

                if (playerCol < enemyCol) newCol--;
                else if (playerCol > enemyCol) newCol++;
            }
            else
            {
                // Move away from player
                if (playerRow < enemyRow) newRow++;
                else if (playerRow > enemyRow) newRow--;

                if (playerCol < enemyCol) newCol++;
                else if (playerCol > enemyCol) newCol--;
            }

            // Ensure new position is valid
            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
            {
                char tile = map[newRow, newCol];
                if (tile != '^' && tile != '~' && !(newRow == playerRow && newCol == playerCol))
                {
                    enemyRow = newRow;
                    enemyCol = newCol;
                }
                else
                {
                    // Random move if blocked
                    int dir = rnd.Next(0, 4);
                    newRow = enemyRow;
                    newCol = enemyCol;
                    switch (dir)
                    {
                        case 0: newRow--; break;
                        case 1: newRow++; break;
                        case 2: newCol--; break;
                        case 3: newCol++; break;
                    }
                    if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                    {
                        tile = map[newRow, newCol];
                        if (tile != '^' && tile != '~' && !(newRow == playerRow && newCol == playerCol))
                        {
                            enemyRow = newRow;
                            enemyCol = newCol;
                        }
                    }
                }
            }
        }

        // Load map from file
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

        // Draw map with player and enemy
        static void PrintMapWithPlayerAndEnemy(char[,] map, int playerRow, int playerCol)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            // Draw top border
            Console.Write("+");
            for (int i = 0; i < cols; i++) Console.Write("-");
            Console.WriteLine("+");

            // Draw map rows
            for (int r = 0; r < rows; r++)
            {
                Console.Write("|");
                for (int c = 0; c < cols; c++)
                {
                    if (r == playerRow && c == playerCol)
                        Console.Write("@"); // player
                    else if (r == enemyRow && c == enemyCol && enemyHealth > 0)
                        Console.Write("E"); // enemy
                    else
                        Console.Write(map[r, c]); // map tile
                }
                Console.WriteLine("|");
            }

            // Draw bottom border
            Console.Write("+");
            for (int i = 0; i < cols; i++) Console.Write("-");
            Console.WriteLine("+");

            Console.WriteLine("Use WASD to move. ESC to exit.");
        }

        // Display player and enemy HUD
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

        // Apply damage to player
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

        // Heal player
        static void Heal(int healAmount)
        {
            health += healAmount;
            if (health > maxHealth) health = maxHealth;
        }

        // Regenerate shield
        static void RegenerateShield(int regenAmount)
        {
            shield += regenAmount;
            if (shield > maxShield) shield = maxShield;
        }

        // Revive player
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
