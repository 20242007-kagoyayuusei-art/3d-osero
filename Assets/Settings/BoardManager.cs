using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // ---- Unity 上で Prefab をアサインするための変数 ----
    public GameObject cellPrefab;   // 1マスのPrefab
    public GameObject blackPrefab;  // 黒石Prefab
    public GameObject whitePrefab;  // 白石Prefab

    const int SIZE = 4;
    int[,] board = new int[SIZE, SIZE];  // オセロ盤（0=空,1=黒,2=白）
    int currentPlayer = 1;               // 1=黒, 2=白

    void Start()
    {
        InitBoard();   // 初期配置（黒白4つ）
        CreateBoard(); // 8×8のマスを生成
    }

    // ---------------------------
    // 初期の4つの石を盤情報にセット
    // ---------------------------
    void InitBoard()
    {
        int mid = SIZE / 2;
        board[mid - 1, mid - 1] = 2;
        board[mid,     mid]     = 2;
        board[mid - 1, mid]     = 1;
        board[mid,     mid - 1] = 1;
    }

    // ---------------------------
    // クリックできる「マス」を8×8並べる
    // ---------------------------
    void CreateBoard()
    {
        for (int r = 0; r < SIZE; r++)
        {
            for (int c = 0; c < SIZE; c++)
            {
                // Prefab を生成
                GameObject cell = Instantiate(cellPrefab, new Vector3(c, -r, 0), Quaternion.identity);

                // Cell スクリプトに座標をセット
                Cell cellScript = cell.GetComponent<Cell>();
                cellScript.r = r;
                cellScript.c = c;
            }
        }

        // 初期配置の4つを表示
        SpawnStone(SIZE/2 - 1, SIZE/2 - 1, 2);
        SpawnStone(SIZE/2    , SIZE/2    , 2);
        SpawnStone(SIZE/2 - 1, SIZE/2    , 1);
        SpawnStone(SIZE/2    , SIZE/2 - 1, 1);
    }

    // ---------------------------
    // マスがクリックされた時に Cell.cs から呼ばれる
    // ---------------------------
    public void OnCellClicked(int r, int c)
    {
        // 置ける場所か判定
        if (IsValidMove(r, c, currentPlayer))
        {
            PlaceStone(r, c, currentPlayer);      // 盤情報を更新
            SpawnStone(r, c, currentPlayer);      // 見た目の石を追加
            currentPlayer = 3 - currentPlayer;    // 手番交代（1→2、2→1）
        }
        else
        {
            Debug.Log("そこには置けません！");
        }
    }

    // ---------------------------
    // 石を見た目として生成する
    // ---------------------------
    void SpawnStone(int r, int c, int player)
    {
        Vector3 pos = new Vector3(c, -r, 0);
        GameObject prefab = (player == 1) ? blackPrefab : whitePrefab;
        Instantiate(prefab, pos, Quaternion.identity);
    }


    // ---------------------------
    // ★ここから下はあなたの CUI 版を Unity 用にほぼ移植
    // ---------------------------

    bool IsValidMove(int r, int c, int player)
    {
        if (r < 0 || r >= SIZE || c < 0 || c >= SIZE) return false;
        if (board[r, c] != 0) return false;

        int opponent = 3 - player;

        int[] dr = { -1, -1, -1, 0, 1, 1, 1, 0 };
        int[] dc = { -1, 0, 1, 1, 1, 0, -1, -1 };

        for (int i = 0; i < 8; i++)
        {
            int rr = r + dr[i];
            int cc = c + dc[i];
            bool foundOpponent = false;

            while (rr >= 0 && rr < SIZE && cc >= 0 && cc < SIZE && board[rr, cc] == opponent)
            {
                foundOpponent = true;
                rr += dr[i];
                cc += dc[i];
            }

            if (foundOpponent && rr >= 0 && rr < SIZE && cc >= 0 && cc < SIZE && board[rr, cc] == player)
            {
                return true;
            }
        }
        return false;
    }

    void PlaceStone(int r, int c, int player)
    {
        board[r, c] = player;
        int opponent = 3 - player;

        int[] dr = { -1, -1, -1, 0, 1, 1, 1, 0 };
        int[] dc = { -1, 0, 1, 1, 1, 0, -1, -1 };

        for (int i = 0; i < 8; i++)
        {
            int rr = r + dr[i];
            int cc = c + dc[i];
            bool foundOpponent = false;

            while (rr >= 0 && rr < SIZE && cc >= 0 && cc < SIZE && board[rr, cc] == opponent)
            {
                foundOpponent = true;
                rr += dr[i];
                cc += dc[i];
            }

            // 裏返す
            if (foundOpponent && rr >= 0 && rr < SIZE && cc >= 0 && cc < SIZE && board[rr, cc] == player)
            {
                rr = r + dr[i];
                cc = c + dc[i];
                while (board[rr, cc] == opponent)
                {
                    board[rr, cc] = player;

                    // 石の見た目も反転させたい場合、ここで対応
                    // （今は見た目の反転は後で作る）

                    rr += dr[i];
                    cc += dc[i];
                }
            }
        }
    }
}
