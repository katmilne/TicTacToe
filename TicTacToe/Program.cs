using System;
using System.Collections.Generic;

class Game
{
    private Player player1;
    private Player player2;
    private Board board;

    public Game()
    {
        player1 = new Player("Player 1", 'X');
        player2 = new ComputerPlayer("Computer", 'O');
        board = new Board();
    }

    public void StartGame()
    {
        Console.WriteLine("Welcome to Tic Tac Toe!");
        Console.WriteLine("1. Play against a friend");
        Console.WriteLine("2. Play against the computer");
        int choice = int.Parse(Console.ReadLine());

        if (choice == 1)
        {
            PlayAgainstFriend();
        }
        else if (choice == 2)
        {
            PlayAgainstComputer();
        }
        else
        {
            Console.WriteLine("Invalid choice. Please try again.");
            StartGame();
        }
    }

    public void PlayAgainstFriend()
    {
        Console.Clear();
        Console.WriteLine("You are playing against a friend!");
        board.InitializeBoard();

        bool gameover = false;

        while (!gameover)
        {
            board.Display();
            Player currentPlayer = (board.MoveCount % 2 == 0) ? player1 : player2;

            Console.WriteLine($"{currentPlayer.Name}'s turn. Enter row and column (e.g., 1 2):");
            string[] input = Console.ReadLine().Split(' ');
            int row = int.Parse(input[0]);
            int col = int.Parse(input[1]);

            if (board.IsValidMove(row, col))
            {
                board.MakeMove(row, col, currentPlayer.Symbol);
                board.IncrementMoveCount();

                if (board.CheckWinCondition(currentPlayer.Symbol))
                {
                    board.Display();
                    Console.WriteLine($"{currentPlayer.Name} wins!");
                    gameover = true;
                }
                else if (board.CheckDraw())
                {
                    board.Display();
                    Console.WriteLine("It's a draw!");
                    gameover = true;
                }
            }
            else
            {
                Console.WriteLine("Invalid move. Try again.");
            }
        }
    }

    public void PlayAgainstComputer()
    {
        Console.Clear();
        Console.WriteLine("You are playing against the computer!");
        board.InitializeBoard();

        bool gameover = false;

        while (!gameover)
        {
            board.Display();
            Player currentPlayer = (board.MoveCount % 2 == 0) ? player1 : player2;

            if (currentPlayer == player1)
            {
                Console.WriteLine($"{currentPlayer.Name}'s turn. Enter row and column (e.g., 1 2):");
                string[] input = Console.ReadLine().Split(' ');
                int row = int.Parse(input[0]);
                int col = int.Parse(input[1]);

                if (board.IsValidMove(row, col))
                {
                    board.MakeMove(row, col, currentPlayer.Symbol);
                    board.IncrementMoveCount();
                }
                else
                {
                    Console.WriteLine("Invalid move. Try again.");
                    continue;
                }
            }
            else
            {
                Console.WriteLine($"{currentPlayer.Name}'s turn (Computer).");
                int computerMove = ((ComputerPlayer)currentPlayer).MakeMove(board);
                int row = computerMove / 3;
                int col = computerMove % 3;

                board.MakeMove(row, col, currentPlayer.Symbol);
                board.IncrementMoveCount();
            }

            if (board.CheckWinCondition(currentPlayer.Symbol))
            {
                board.Display();
                if (currentPlayer == player1)
                    Console.WriteLine($"{currentPlayer.Name} wins!");
                else
                    Console.WriteLine("Computer wins!");
                gameover = true;
            }
            else if (board.CheckDraw())
            {
                board.Display();
                Console.WriteLine("It's a draw!");
                gameover = true;
            }
        }
    }
}

class Player
{
    public string Name { get; set; }
    public char Symbol { get; set; }

    public Player(string name, char symbol)
    {
        Name = name;
        Symbol = symbol;
    }
}

class ComputerPlayer : Player
{
    public ComputerPlayer(string name, char symbol) : base(name, symbol) { }

    public int MakeMove(Board board)
    {
        return MiniMax(board, Symbol).Move;
    }

    private (int Score, int Move) MiniMax(Board board, char player)
    {
        List<int> availableMoves = board.GetAvailableMoves();
        List<(int Score, int Move)> moves = new List<(int Score, int Move)>();

        if (board.CheckWinCondition(Symbol))
        {
            return (1, -1);
        }
        else if (board.CheckWinCondition((Symbol == 'X') ? 'O' : 'X'))
        {
            return (-1, -1);
        }
        else if (availableMoves.Count == 0)
        {
            return (0, -1);
        }

        foreach (int move in availableMoves)
        {
            Board newBoard = new Board();
            newBoard.CopyFrom(board);
            newBoard.MakeMove(move / 3, move % 3, player);

            int score = MiniMax(newBoard, (player == 'X') ? 'O' : 'X').Score;
            moves.Add((score, move));
        }

        if (player == Symbol)
        {
            int maxScoreIndex = moves.FindIndex(m => m.Score == moves.Max(x => x.Score));
            return moves[maxScoreIndex];
        }
        else
        {
            int minScoreIndex = moves.FindIndex(m => m.Score == moves.Min(x => x.Score));
            return moves[minScoreIndex];
        }
    }
}

class Board
{
    private char[,] board;
    public int MoveCount { get; private set; }

    public Board()
    {
        board = new char[3, 3];
    }

    public void InitializeBoard()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                board[row, col] = ' ';
            }
        }
        MoveCount = 0;
    }

    public List<int> GetAvailableMoves()
    {
        List<int> moves = new List<int>();
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (board[row, col] == ' ')
                {
                    moves.Add(row * 3 + col);
                }
            }
        }
        return moves;
    }

    public void CopyFrom(Board source)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                board[row, col] = source.board[row, col];
            }
        }
        MoveCount = source.MoveCount;
    }

    public void Display()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Console.Write(board[row, col]);
                if (col < 2)
                {
                    Console.Write(" | ");
                }
            }
            Console.WriteLine();
            if (row < 2)
            {
                Console.WriteLine("---------");
            }
        }
    }

    public bool IsValidMove(int row, int col)
    {
        return row >= 0 && row < 3 && col >= 0 && col < 3 && board[row, col] == ' ';
    }

    public void MakeMove(int row, int col, char symbol)
    {
        board[row, col] = symbol;
    }

    public void IncrementMoveCount()
    {
        MoveCount++;
    }

    public bool CheckWinCondition(char symbol)
    {
        for (int row = 0; row < 3; row++)
        {
            if (board[row, 0] == symbol && board[row, 1] == symbol && board[row, 2] == symbol)
                return true;
        }

        for (int col = 0; col < 3; col++)
        {
            if (board[0, col] == symbol && board[1, col] == symbol && board[2, col] == symbol)
                return true;
        }

        if (board[0, 0] == symbol && board[1, 1] == symbol && board[2, 2] == symbol)
            return true;

        if (board[0, 2] == symbol && board[1, 1] == symbol && board[2, 0] == symbol)
            return true;

        return false;
    }

    public bool CheckDraw()
    {
        foreach (char cell in board)
        {
            if (cell == ' ')
            {
                return false;
            }
        }
        return true;
    }
}

class Program
{
    static void Main()
    {
        Game game = new Game();
        game.StartGame();
    }
}
