using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Game : SingletonMonoBehaviour<Game>
{
    public static readonly int XNUM = 4;
    public static readonly int ZNUM = 4;
    public static readonly int YNUM = 4;


    public enum GameMode
    {
        PlayerVsAI,
        PlayerVsPlayer
}



    public enum State
    {
        None,
        Initializing,
        BlackTurn,
        WhiteTurn,
        Result,
    }

    [SerializeField]
    private Stone _stonePrefab;

    [SerializeField]
    private Transform _stoneBase;

    [SerializeField]
    private SelfPlayer _selfPlayer;

    [SerializeField]
    private EnemyPlayer _enemyPlayer;

    [SerializeField]
    private LocalPlayer _localPlayerBlack;

    [SerializeField]
    private LocalPlayer _localPlayerWhite;

    [SerializeField]
    private GameObject _cursor;

    private BasePlayer _blackPlayer;
    private BasePlayer _whitePlayer;

    [SerializeField]
    private TextMeshPro _blackScoreText;

    [SerializeField]
    private TextMeshPro _whiteScoreText;

    [SerializeField]
    private TextMeshPro _resultText;

    [SerializeField]
    private AudioSource _seAudioSource;

    [SerializeField]
    private AudioClip _cursorMoveSe;

    [SerializeField]
    private AudioClip _stoneAppearSe;

    [SerializeField]
    private AudioClip _stoneReverseSe;


    public State CurrentState { get; private set; } = State.None;

    public GameObject Cursor { get { return _cursor; } }

    public int CurrentTurn {
        get
        {
            var turnCount = 0;
            for (var y = 0; y < YNUM; y++)
            {
                for (var z = 0; z < ZNUM; z++)
                {
                    for (var x = 0; x < XNUM; x++)
                    {
                        if (_stones[y][z][x].CurrentState != Stone.State.None)
                            turnCount++;
                    }
                }
            }
            return turnCount;
        }
    }

    private Stone[][][] _stones;
    public Stone[][][] Stones { get { return _stones; } }

    private bool _isInitialized = false;

    private GameMode _currentGameMode = GameMode.PlayerVsAI;

    public void SetupGameMode(GameMode mode)
    {
        _currentGameMode = mode;
        switch (mode)
        {
            case GameMode.PlayerVsAI:
                _blackPlayer = _selfPlayer;
                _whitePlayer = _enemyPlayer;
                if (_localPlayerBlack != null) _localPlayerBlack.gameObject.SetActive(false);
                if (_localPlayerWhite != null) _localPlayerWhite.gameObject.SetActive(false);
                if (_selfPlayer != null) _selfPlayer.gameObject.SetActive(true);
                if (_enemyPlayer != null) _enemyPlayer.gameObject.SetActive(true);
                break;
            case GameMode.PlayerVsPlayer:
                _blackPlayer = _localPlayerBlack;
                _whitePlayer = _localPlayerWhite;
                if (_selfPlayer != null) _selfPlayer.gameObject.SetActive(false);
                if (_enemyPlayer != null) _enemyPlayer.gameObject.SetActive(false);
                if (_localPlayerBlack != null) {
                    _localPlayerBlack.AssignedPlayerNumber = LocalPlayer.PlayerNumber.Player1;
                    _localPlayerBlack.gameObject.SetActive(true);
                }
                if (_localPlayerWhite != null) {
                    _localPlayerWhite.AssignedPlayerNumber = LocalPlayer.PlayerNumber.Player2;
                    _localPlayerWhite.gameObject.SetActive(true);
                }
                break;
        }
    }

    private void Start()
    {
        _stones = new Stone[YNUM][][];
        for (var y = 0; y < YNUM; y++)
        {
            _stones[y] = new Stone[ZNUM][];
            for (var z = 0; z < ZNUM; z++)
            {
                _stones[y][z] = new Stone[XNUM];
                for (var x = 0; x < XNUM; x++)
                {
                    var stone = Instantiate(_stonePrefab, _stoneBase);
                    var t = stone.transform;
                    t.localPosition = new Vector3(x * 10, y * 10, z * 10);
                    t.localRotation = Quaternion.identity;
                    t.localScale = Vector3.one;
                    _stones[y][z][x] = stone;
                }
            }
        }

        // メニューで選択されたゲームモードを反映
        SetupGameMode(GameModeData.SelectedMode);

        CurrentState = State.Initializing;
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case State.Initializing:
                if (!_isInitialized)
                {
                    for (var y = 0; y < YNUM; y++)
                    {
                        for (var z = 0; z < ZNUM; z++)
                        {
                            for (var x = 0; x < XNUM; x++)
                            {
                                _stones[y][z][x].SetActive(false, Stone.Color.Black);
                            }
                        }
                    }

                    // 初期配置（中央に2*2*2で白４黒４の配置）
                    var centerY = YNUM / 2 - 1;
                    var centerZ = ZNUM / 2 - 1;
                    var centerX = XNUM / 2 - 1;
                    
                    // Z-1層
                    _stones[centerY][centerZ][centerX].SetActive(true, Stone.Color.White);
                    _stones[centerY][centerZ][centerX + 1].SetActive(true, Stone.Color.Black);
                    _stones[centerY][centerZ + 1][centerX].SetActive(true, Stone.Color.Black);
                    _stones[centerY][centerZ + 1][centerX + 1].SetActive(true, Stone.Color.White);
                    
                    // Z層
                    _stones[centerY + 1][centerZ][centerX].SetActive(true, Stone.Color.Black);
                    _stones[centerY + 1][centerZ][centerX + 1].SetActive(true, Stone.Color.White);
                    _stones[centerY + 1][centerZ + 1][centerX].SetActive(true, Stone.Color.White);
                    _stones[centerY + 1][centerZ + 1][centerX + 1].SetActive(true, Stone.Color.Black);
                    UpdateScore();
                    _resultText.gameObject.SetActive(false);
                    _cursor.SetActive(false);

                    _isInitialized = true;
                }

                if (!IsAnimating())
                {
                    CurrentState = State.BlackTurn;
                }
                break;

            case State.BlackTurn:
                {
                    if (IsAnimating())
                    {
                        break;
                    }
                    if (_blackPlayer != null && _blackPlayer.TryGetSelected(out var x, out var y, out var z))
                    {
                        _stones[y][z][x].SetActive(true, Stone.Color.Black);
                        Reverse(Stone.Color.Black, x, y, z);
                        UpdateScore();
                        if (IsGameFinished())
                        {
                            CurrentState = State.Result;
                        }
                        else
                        {
                            if (_whitePlayer != null && _whitePlayer.CanPut())
                            {
                                CurrentState = State.WhiteTurn;
                            }
                            else if (_blackPlayer != null && !_blackPlayer.CanPut())
                            {
                                CurrentState = State.Result;
                            }
                        }
                    }
                }
                break;
            case State.WhiteTurn:
                {
                    if (IsAnimating())
                    {
                        break;
                    }
                    if (_whitePlayer != null && _whitePlayer.TryGetSelected(out var x, out var y, out var z))
                    {
                        _stones[y][z][x].SetActive(true, Stone.Color.White);
                        Reverse(Stone.Color.White, x, y, z);
                        UpdateScore();
                        if (IsGameFinished())
                        {
                            CurrentState = State.Result;
                        }
                        else
                        {
                            if (_blackPlayer != null && _blackPlayer.CanPut())
                            {
                                CurrentState = State.BlackTurn;
                            }
                            else if (_whitePlayer != null && !_whitePlayer.CanPut())
                            {
                                CurrentState = State.Result;
                            }
                        }
                    }
                }
                break;

            case State.Result:
                {
                    if (!_resultText.gameObject.activeSelf)
                    {
                        int blackScore;
                        int whiteScore;
                        CalcScore(out blackScore, out whiteScore);
                        _resultText.text = blackScore > whiteScore ? "You Win"
                            : blackScore < whiteScore ? "You Lose"
                            : "Draw";
                        _resultText.gameObject.SetActive(true);
                    }

                    var kb = Keyboard.current;
                    if (kb.enterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)
                    {
                        _isInitialized = false;
                        CurrentState = State.Initializing;
                    }
                }
                break;

            case State.None:
            default:
                break;
        }
    }

    private void UpdateScore()
    {
        int blackScore;
        int whiteScore;
        CalcScore(out blackScore, out whiteScore);
        _blackScoreText.text = blackScore.ToString();
        _whiteScoreText.text = whiteScore.ToString();
    }

    private void CalcScore(out int blackScore, out int whiteScore)
    {
        blackScore = 0;
        whiteScore = 0;

        for (var y = 0; y < YNUM; y++)
        {
            for (var z = 0; z < ZNUM; z++)
            {
                for (var x = 0; x < XNUM; x++)
                {
                    if (_stones[y][z][x].CurrentState != Stone.State.None)
                    {
                        switch (_stones[y][z][x].CurrentColor)
                        {
                            case Stone.Color.Black:
                                blackScore++;
                                break;
                            case Stone.Color.White:
                                whiteScore++;
                                break;
                        }
                    }
                }
            }
        }
    }

    private void Reverse(Stone.Color color, int putX, int putY, int putZ)
    {
        for (var dirY = -1; dirY <= 1; dirY++)
        {
            for (var dirZ = -1; dirZ <= 1; dirZ++)
            {
                for (var dirX = -1; dirX <= 1; dirX++)
                {
                    if (dirX == 0 && dirY == 0 && dirZ == 0)
                        continue;

                    var reverseCount = CalcReverseCount(color, putX, putY, putZ, dirX, dirY, dirZ);

                    for (var i = 1; i <= reverseCount; i++)
                    {
                        _stones[putY + dirY * i][putZ + dirZ * i][putX + dirX * i].Reverse();
                    }
                }
            }
        }
    }

    private int CalcReverseCount(Stone.Color color, int putX, int putY, int putZ, int dirX, int dirY, int dirZ)
    {
        var x = putX;
        var y = putY;
        var z = putZ;
        var reverseCount = 0;
        for (var i = 0; i < 4; i++)
        {
            x += dirX;
            y += dirY;
            z += dirZ;
            if (x < 0 || XNUM <= x || y < 0 || YNUM <= y || z < 0 || ZNUM <= z)
            {
                reverseCount = 0;
                break;
            }

            var stone = _stones[y][z][x];
            if (stone.CurrentState == Stone.State.None)
            {
                reverseCount = 0;
                break;
            }
            else
            {
                if (stone.CurrentColor != color)
                {
                    reverseCount++;
                }
                else
                {
                    break;
                }
            }
        }

        return reverseCount;
    }

    public int CalcTotalReverseCount(Stone.Color color, int putX, int putY, int putZ)
    {
        if (_stones[putY][putZ][putX].CurrentState != Stone.State.None)
            return 0;

        var totalReverseCount = 0;
        for (var dirY = -1; dirY <= 1; dirY++)
        {
            for (var dirZ = -1; dirZ <= 1; dirZ++)
            {
                for (var dirX = -1; dirX <= 1; dirX++)
                {
                    if (dirX == 0 && dirY == 0 && dirZ == 0)
                        continue;

                    totalReverseCount += CalcReverseCount(color, putX, putY, putZ, dirX, dirY, dirZ);
                }
            }
        }

        return totalReverseCount;
    }

    private bool IsGameFinished()
    {
        for (var y = 0; y < YNUM; y++)
        {
            for (var z = 0; z < ZNUM; z++)
            {
                for (var x = 0; x < XNUM; x++)
                {
                    if (_stones[y][z][x].CurrentState == Stone.State.None)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private bool IsAnimating()
    {
        for (var y = 0; y < Game.YNUM; y++)
        {
            for (var z = 0; z < Game.ZNUM; z++)
            {
                for (var x = 0; x < Game.XNUM; x++)
                {
                    switch (_stones[y][z][x].CurrentState)
                    {
                        case Stone.State.Appearing:
                        case Stone.State.Reversing:
                            return true;
                    }
                }
            }
        }
        return false;
    }

    public void PlayCursorMoveSe()
    {
        _seAudioSource.PlayOneShot(_cursorMoveSe);
    }

    public void PlayStoneAppearSe()
    {
        _seAudioSource.PlayOneShot(_stoneAppearSe);
    }

    public void PlayStoneReverseSe()
    {
        _seAudioSource.PlayOneShot(_stoneReverseSe);
    }
}