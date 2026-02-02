using UnityEngine;
using UnityEngine.InputSystem;

public class SelfPlayer : BasePlayer
{
    public override Stone.Color MyColor { get { return Stone.Color.Black; } }

    private Transform _cachedCursorTransform;
    private Vector3Int _cursorPos = new Vector3Int(0, 0, 0);
    private Vector3Int? _desidedPos = null;

    private int _processingPlayerTurn = 0;

    public override bool TryGetSelected(out int x, out int y, out int z)
    {
        if (_desidedPos.HasValue)
        {
            var pos = _desidedPos.Value;
            x = pos.x;
            y = pos.y;
            z = pos.z;
            return true;
        }
        return base.TryGetSelected(out x, out y, out z);
    }

    private void Start()
    {
        _cachedCursorTransform = Game.Instance.Cursor.transform;
    }

    private void Update()
    {
        switch (Game.Instance.CurrentState)
        {
            case Game.State.None:
            case Game.State.Initializing:
            case Game.State.WhiteTurn:
                break;

            case Game.State.BlackTurn:
                ExecTurn();
                break;

            case Game.State.Result:
                // TODO
                break;
        }
    }

    private void ExecTurn()
    {
        var currentTurn = Game.Instance.CurrentTurn;
        if (_processingPlayerTurn != currentTurn)
        {
            ShowDots();
            _processingPlayerTurn = currentTurn;
            _desidedPos = null;
            Game.Instance.Cursor.SetActive(true);
        }

        // マウスクリック処理
        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            TrySelectByMouseClick();
        }

        var keyboard = Keyboard.current;
        if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame)
        {
            TryCursorMove(-1, 0, 0);
        }
        else if (keyboard.upArrowKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame)
        {
            TryCursorMove(0, 0, 1);
        }
        else if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame)
        {
            TryCursorMove(1, 0, 0);
        }
        else if (keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame)
        {
            TryCursorMove(0, 0, -1);
        }
        else if (keyboard.qKey.wasPressedThisFrame)
        {
            TryCursorMove(0, -1, 0);
        }
        else if (keyboard.eKey.wasPressedThisFrame)
        {
            TryCursorMove(0, 1, 0);
        }

        if (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
        {
            if (Game.Instance.CalcTotalReverseCount(MyColor, _cursorPos.x, _cursorPos.y, _cursorPos.z) > 0)
            {
                _desidedPos = _cursorPos;
                Game.Instance.Cursor.SetActive(false);
                HideDots();
            }
        }
    }

    private bool TryCursorMove(int deltaX, int deltaY, int deltaZ)
    {
        var x = _cursorPos.x;
        var y = _cursorPos.y;
        var z = _cursorPos.z;
        x += deltaX;
        y += deltaY;
        z += deltaZ;
        if (x < 0 || Game.XNUM <= x)
            return false;
        if (y < 0 || Game.YNUM <= y)
            return false;
        if (z < 0 || Game.ZNUM <= z)
            return false;

        _cursorPos.x = x;
        _cursorPos.y = y;
        _cursorPos.z = z;
        _cachedCursorTransform.localPosition = _cursorPos * 10;
        Game.Instance.PlayCursorMoveSe();
        return true;
    }

    private void TrySelectByMouseClick()
    {
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var stone = hit.collider.GetComponent<Stone>();
            if (stone != null && stone.CurrentState == Stone.State.None)
            {
                // クリックした駒の位置を取得
                var stonePos = hit.collider.transform.localPosition;
                var x = Mathf.RoundToInt(stonePos.x / 10f);
                var y = Mathf.RoundToInt(stonePos.y / 10f);
                var z = Mathf.RoundToInt(stonePos.z / 10f);

                // 範囲チェック
                if (x >= 0 && x < Game.XNUM && y >= 0 && y < Game.YNUM && z >= 0 && z < Game.ZNUM)
                {
                    if (Game.Instance.CalcTotalReverseCount(MyColor, x, y, z) > 0)
                    {
                        _desidedPos = new Vector3Int(x, y, z);
                        Game.Instance.Cursor.SetActive(false);
                        HideDots();
                    }
                }
            }
        }
    }

    private void ShowDots()
    {
        var availablePoints = GetAvailablePoints();
        var stones = Game.Instance.Stones;
        foreach (var availablePoint in availablePoints.Keys)
        {
            var x = availablePoint.Item1;
            var y = availablePoint.Item2;
            var z = availablePoint.Item3;
            stones[y][z][x].EnableDot();
        }
    }

    private void HideDots()
    {
        var stones = Game.Instance.Stones;
        for (var y = 0; y < Game.YNUM; y++)
        {
            for (var z = 0; z < Game.ZNUM; z++)
            {
                for (var x = 0; x < Game.XNUM; x++)
                {
                    if (stones[y][z][x].CurrentState == Stone.State.None)
                    {
                        stones[y][z][x].SetActive(false, MyColor);
                    }
                }
            }
        }
    }
}