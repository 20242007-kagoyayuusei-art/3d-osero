using UnityEngine;
using UnityEngine.InputSystem;

public class LocalPlayer : BasePlayer
{
    public enum PlayerNumber
    {
        Player1,
        Player2
    }

    [SerializeField]
    private PlayerNumber _playerNumber = PlayerNumber.Player1;

    public override Stone.Color MyColor
    {
        get { return _playerNumber == PlayerNumber.Player1 ? Stone.Color.Black : Stone.Color.White; }
    }

    private Transform _cachedCursorTransform;
    private Vector3Int _cursorPos = new Vector3Int(0, 0, 0);
    private Vector3Int? _decidedPos = null;

    private int _processingPlayerTurn = 0;

    public override bool TryGetSelected(out int x, out int y, out int z)
    {
        if (_decidedPos.HasValue)
        {
            var pos = _decidedPos.Value;
            x = pos.x;
            y = pos.y;
            z = pos.z;
            _decidedPos = null;
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
        Game.State currentState = Game.Instance.CurrentState;
        bool isMyTurn = (_playerNumber == PlayerNumber.Player1 && currentState == Game.State.BlackTurn) ||
                        (_playerNumber == PlayerNumber.Player2 && currentState == Game.State.WhiteTurn);

        if (!isMyTurn)
            return;

        ExecTurn();
    }

    private void ExecTurn()
    {
        var currentTurn = Game.Instance.CurrentTurn;
        if (_processingPlayerTurn != currentTurn)
        {
            ShowDots();
            _processingPlayerTurn = currentTurn;
            _decidedPos = null;
            Game.Instance.Cursor.SetActive(true);
        }

        // マウスクリック処理
        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            TrySelectByMouseClick();
        }

        var keyboard = Keyboard.current;

        // Player1: Arrow Keys + Q/E
        // Player2: IJKL + U/O
        bool isLeftMove = _playerNumber == PlayerNumber.Player1 ? 
            (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame) :
            (keyboard.jKey.wasPressedThisFrame);

        bool isUpMove = _playerNumber == PlayerNumber.Player1 ?
            (keyboard.upArrowKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame) :
            (keyboard.iKey.wasPressedThisFrame);

        bool isRightMove = _playerNumber == PlayerNumber.Player1 ?
            (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame) :
            (keyboard.lKey.wasPressedThisFrame);

        bool isDownMove = _playerNumber == PlayerNumber.Player1 ?
            (keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame) :
            (keyboard.kKey.wasPressedThisFrame);

        bool isUpLayer = _playerNumber == PlayerNumber.Player1 ?
            (keyboard.qKey.wasPressedThisFrame) :
            (keyboard.uKey.wasPressedThisFrame);

        bool isDownLayer = _playerNumber == PlayerNumber.Player1 ?
            (keyboard.eKey.wasPressedThisFrame) :
            (keyboard.oKey.wasPressedThisFrame);

        if (isLeftMove)
        {
            TryCursorMove(-1, 0, 0);
        }
        else if (isUpMove)
        {
            TryCursorMove(0, 0, 1);
        }
        else if (isRightMove)
        {
            TryCursorMove(1, 0, 0);
        }
        else if (isDownMove)
        {
            TryCursorMove(0, 0, -1);
        }
        else if (isUpLayer)
        {
            TryCursorMove(0, -1, 0);
        }
        else if (isDownLayer)
        {
            TryCursorMove(0, 1, 0);
        }

        if (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
        {
            if (Game.Instance.CalcTotalReverseCount(MyColor, _cursorPos.x, _cursorPos.y, _cursorPos.z) > 0)
            {
                _decidedPos = _cursorPos;
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
                        _decidedPos = new Vector3Int(x, y, z);
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
