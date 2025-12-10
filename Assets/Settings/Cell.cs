using UnityEngine;

public class Cell : MonoBehaviour
{
    // このマスの行(row)と列(column)
    public int r;  // row
    public int c;  // column

    private BoardManager board;

    void Start()
    {
        // Unity 新API（警告が出ない）
        board = FindFirstObjectByType<BoardManager>();
    }

    void OnMouseDown()
    {
        if (board != null)
        {
            board.OnCellClicked(r, c);
        }
        else
        {
            Debug.LogError("BoardManager が見つかりません。");
        }
    }
}
