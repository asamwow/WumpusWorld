using System;

namespace WumpusWorld
{
    class Program
    {

        public static Random random;
        public static Cell[,] board;
        public static bool[,] revealedCells;
        public static int boardSize = 4;
        public static Cell playerCell;
        public static bool alive = true;
        public static int gold = 2;
        public static int arrows = 5;

        static void Main(string[] args)
        {
            random = new Random();

            int wompuses = random.Next(2, 4);
            int pits = random.Next(2, 3);

            board = new Cell[boardSize, boardSize];
            revealedCells = new bool[boardSize, boardSize];

            for (int x = 0; x < boardSize; x++) {
                for (int y = 0; y < boardSize; y++) {
                    board[x, y] = new Cell(Cell.Item.None, new Position(x, y));
                    revealedCells[x, y] = false;
                }
            }

            playerCell = board[0, 0];
            revealedCells[0, 0] = true;

            // place gold
            for (int i = 0; i < gold; i++) {
                int x, y;
                do {
                    x = random.Next(0, boardSize-1);
                    y = random.Next(0, boardSize-1);
                } while (x == 0 && y == 0);
                board[x, y].item = Cell.Item.Gold;
            }

            // place wompuses
            for (int i = 0; i < wompuses; i++) {
                int x, y;
                do {
                    x = random.Next(0, boardSize-1);
                    y = random.Next(0, boardSize-1);
                } while ((x == 0 && y == 0) || board[x, y].item != Cell.Item.None);
                board[x, y].item = Cell.Item.Wompus;
            }

            // place pit
            // place wompuses
            for (int i = 0; i < pits; i++) {
                int x, y;
                do {
                    x = random.Next(0, boardSize-1);
                    y = random.Next(0, boardSize-1);
                } while ((x == 0 && y == 0) || board[x, y].item != Cell.Item.None);
                board[x, y].item = Cell.Item.Pit;
            }

            while (gold != 0 && alive) {
                PrintBoard();
                PrintSenses();
                Console.WriteLine("Please Input a command followed by a direction, or enter help");
                Tuple<string, Position> input = getUserInstruction();
                switch (input.Item1) {
                    case "help":
                        Console.WriteLine("move <up, down, left, right>");
                        Console.WriteLine("arrow <up, down, left, right>");
                        break;
                    case "move":
                        Console.WriteLine(Move(input.Item2));
                        break;
                    case "arrow":
                    case "fire":
                    case "firearrow":
                    case "shoot":
                    case "shootarrow":
                        Console.WriteLine(FireArrow(input.Item2));
                        break;
                    default:
                        Console.WriteLine("Invalid Command");
                        break;
                }
            }
            if (gold == 0) {
                Console.WriteLine("You found all the gold! Congratulations!");
            }
            Console.WriteLine("Game Over");
        }

        public static char getChar(Cell.Item item) {
            switch (item) {
                case Cell.Item.Gold:
                    return 'g';
                case Cell.Item.None:
                    return '.';
                case Cell.Item.Pit:
                    return 'X';
                case Cell.Item.Wompus:
                    return 'W';
                default:
                    return ' ';
            }
        }

        public static void PrintBoard() {
            for (int y = 0; y < boardSize; y++) {
                string row = "";
                for (int x = 0; x < boardSize; x++) {
                    if (playerCell.position == new Position(x, y)) {
                        row += 'X';
                    } else if (revealedCells[x, y]) {
                        row += getChar(board[x, y].item);
                    } else {
                        row += '_';
                    }
                    row += ' ';
                }
                Console.WriteLine(row);
            }
        }

        public static void PrintSenses() {
            for (int i = 0; i < 4; i++) {
                Position direction;
                switch (i) {
                    case 0:
                        direction = Position.down;
                        break;
                    case 1:
                        direction = Position.right;
                        break;
                    case 2:
                        direction = Position.up;
                        break;
                    default:
                        direction = Position.left;
                        break;
                }
                Position nearbyCell = playerCell.position + direction;
                if (nearbyCell.x < 0 || nearbyCell.x >= boardSize || nearbyCell.y < 0 || nearbyCell.y >= boardSize) {
                    continue;
                }
                if (GetCell(nearbyCell).item == Cell.Item.Wompus) {
                    Console.WriteLine("You smell a foul stench.");
                } else if (GetCell(nearbyCell).item == Cell.Item.Pit) {
                    Console.WriteLine("You here wind rushing.");
                } else if (GetCell(nearbyCell).item == Cell.Item.Gold) {
                    Console.WriteLine("You see the glow of gold!");
                }
            }
        }

        /// Returns string of what happens to the player when he moves in the direction
        public static string Move(Position direction) {
            Position newPosition = playerCell.position + direction;
            if (newPosition.x < 0 || newPosition.x >= boardSize || newPosition.y < 0 || newPosition.y >= boardSize) {
                return "You examine the cave wall and find nothing of interest.";
            }
            playerCell = GetCell(newPosition);
            RevealCell(newPosition);
            if (playerCell.item == Cell.Item.Wompus) {
                alive = false;
                return "You are eaten by a wompus!";
            }
            if (playerCell.item == Cell.Item.Pit) {
                alive = false;
                return "You fall into a pit!";
            }
            if (playerCell.item == Cell.Item.Gold) {
                gold--;
                return "You find some gold!";
            }
            return "";
        }

        public static string FireArrow(Position direction) {
            if (arrows == 0) {
                return "You have no arrows.";
            }
            arrows --;
            Position newPosition = playerCell.position + direction;
            string arrowCount = "\nYou now have " + arrows + " arrows.";
            if (newPosition.x < 0 || newPosition.x >= boardSize || newPosition.y < 0 || newPosition.y >= boardSize) {
                return "You fire an arrow at the cave wall to little effect" + arrowCount;
            }
            if (GetCell(newPosition).item == Cell.Item.Wompus) {
                GetCell(newPosition).item = Cell.Item.None;
                return "You hear the death cry of a wompus!" + arrowCount;
            }
            return "You hear your arrow fly and hit the ground in the distance." + arrowCount;
        }

        public static Tuple<string, Position> getUserInstruction () {
            string command = "";
            do {
                string input = Console.ReadLine();
                for (int i = 0; i < input.Length; i++) {
                    if (input[i] == ' ') {
                        string directionString = "";
                        for (int j = i+1; j < input.Length; j++) {
                            if (input[j] == ' ') {
                                break;
                            }
                            directionString += Char.ToLower(input[j]);
                        }
                        Tuple<string, Position> returnInput = new Tuple<string, Position>(command, Position.Parse(directionString));
                        if (returnInput.Item2 == Position.zero) {
                            break;
                        }
                        return returnInput;
                    }
                    command += Char.ToLower(input[i]);
                }
                if (command == "help") {
                    return new Tuple<string, Position>(command, Position.zero);
                }
                Console.WriteLine("Sorry, that command was invalid, type \"help\" for list of commands");
            } while (true);
        }

        public static Cell GetCell(Position position) {
            return board[position.x, position.y];
        }

        public static void RevealCell(Position position) {
            revealedCells[position.x, position.y]  = true;
        }
    }
}
