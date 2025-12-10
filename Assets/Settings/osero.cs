
using System;

class Othello
{
    const int SIZE = 8;
    static int[,] board = new int[SIZE, SIZE];
    static int currentPlayer = 1; // 1=黒, 2=白

    static void Main()
    {
        InitBoard();

        while (true)
        {
            Console.Clear();
            PrintBoard();

            if (!HasValidMove(currentPlayer))
            {
                Console.WriteLine($"{PlayerColor(currentPlayer)} の置ける場所がありません。パスします。");
                currentPlayer = 3 - currentPlayer; 
                if (!HasValidMove(currentPlayer))
                {
                    Console.WriteLine("両者置けないためゲーム終了！");
                    ShowResult();
                    return;
                }
                Console.ReadKey();
                continue;
            }

            Console.WriteLine($"{PlayerColor(currentPlayer)} の番です");
            Console.Write("行(0-7) を入力: ");
            int r = int.Parse(Console.ReadLine());
            Console.Write("列(0-7) を入力: ");
            int c = int.Parse(Console.ReadLine());

            if (IsValidMove(r, c, currentPlayer))
            {
                PlaceStone(r, c, currentPlayer);
                currentPlayer = 3 - currentPlayer; // 手番交代
            }
            else
            {
                Console.WriteLine("そこには置けません！");
                Console.ReadKey();
            }
        }
    }

    static void InitBoard()
    {
        int mid = SIZE / 2;
        board[mid - 1, mid - 1] = 2;
        board[mid, mid] = 2;
        board[mid - 1, mid] = 1;
        board[mid, mid - 1] = 1;
    }

    static void PrintBoard()
    {
        Console.WriteLine("   0 1 2 3 4 5 6 7");
        for (int r = 0; r < SIZE; r++)
        {
            Console.Write(r + "  ");
            for (int c = 0; c < SIZE; c++)
            {
                if (board[r, c] == 0) Console.Write(". ");
                else if (board[r, c] == 1) Console.Write("● ");
                else Console.Write("○ ");
            }
            Console.WriteLine();
        }
    }

    static string PlayerColor(int player)
    {
        return player == 1 ? "黒(●)" : "白(○)";
    }

    static bool IsValidMove(int r, int c, int player)
    {
        if (r < 0 || r >= SIZE || c < 0 || c >= SIZE) return false;
        if (board[r, c] != 0) return false;

        int opponent = 3 - player;

        int[] dr = { -1, -1, -1, 0, 1, 1, 1, 0 };
        int[] dc = { -1, 0, 1, 1, 1, 0, -1, -1 };

        for (int i = 0; i < 8; i++)
        {
            int rr = r + dr[i], cc = c + dc[i];
            bool foundOpponent = false;

            while (rr >= 0 && rr < SIZE && cc >= 0 && cc < SIZE && board[rr, cc] == opponent)
            {
                foundOpponent = true;
                rr += dr[i];
                cc += dc[i];
            }

            if (foundOpponent && rr >= 0 && rr < SIZE && cc >= 0 && cc < SIZE && board[rr, cc] == player)
                return true;
        }
        return false;
    }

    static void PlaceStone(int r, int c, int player)
    {
        board[r, c] = player;
        int opponent = 3 - player;

        int[] dr = { -1, -1, -1, 0, 1, 1, 1, 0 };
        int[] dc = { -1, 0, 1, 1, 1, 0, -1, -1 };

        for (int i = 0; i < 8; i++)
        {
            int rr = r + dr[i], cc = c + dc[i];
            bool foundOpponent = false;

            while (rr >= 0 && rr < SIZE && cc >= 0 && cc < SIZE && board[rr, cc] == opponent)
            {
                foundOpponent = true;
                rr += dr[i];
                cc += dc[i];
            }

            if (foundOpponent && rr >= 0 && rr < SIZE && cc >= 0 && cc < SIZE && board[rr, cc] == player)
            {
                rr = r + dr[i];
                cc = c + dc[i];
                while (board[rr, cc] == opponent)
                {
                    board[rr, cc] = player;
                    rr += dr[i];
                    cc += dc[i];
                }
            }
        }
    }

    static bool HasValidMove(int player)
    {
        for (int r = 0; r < SIZE; r++)
            for (int c = 0; c < SIZE; c++)
                if (IsValidMove(r, c, player))
                    return true;
        return false;
    }

    static void ShowResult()
    {
        int black = 0, white = 0;
        for (int r = 0; r < SIZE; r++)
            for (int c = 0; c < SIZE; c++)
                if (board[r, c] == 1) black++;
                else if (board[r, c] == 2) white++;

        Console.WriteLine($"黒: {black} / 白: {white}");
        if (black > white) Console.WriteLine("黒の勝ち！");
        else if (white > black) Console.WriteLine("白の勝ち！");
        else Console.WriteLine("引き分け！");
    }
}
