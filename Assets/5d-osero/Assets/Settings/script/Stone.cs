using System;
using TMPro;
using UnityEngine;

public class Stone : MonoBehaviour
{
    static readonly float AppeareSeconds = 0.5f;
    static readonly float ReverseSeconds = 0.5f;

    public enum Color
    {
        Black,
        White,
    }

    public enum State
    {
        None,
        Appearing,
        Reversing,
        Fix,
    }

    [SerializeField]
    private GameObject _black;

    [SerializeField]
    private GameObject _white;

    [SerializeField]
    private GameObject _dot;

    public Color CurrentColor { get; private set; } = Color.Black;
    public State CurrentState { get; private set; } = State.None;

    private DateTime _stateChangedAt = DateTime.MinValue;
    private float ElapsedSecondsSinceStateChange { get { return (float)(DateTime.UtcNow - _stateChangedAt).TotalSeconds; } }

    private Vector3 _baseLocalPosition;


    private void Start()
    {
        _baseLocalPosition = transform.localPosition;
        // Start()では何もしない - SetActive()で完全に制御する
    }

    private void HideAll()
    {
        CurrentState = State.None;
        if (_black != null) _black.SetActive(false);
        if (_white != null) _white.SetActive(false);
        if (_dot != null) _dot.SetActive(false);
        gameObject.SetActive(true);  // 親は表示したままにしておく
    }

    public void SetActive(bool value, Color color)
    {
        if (!value)
        {
            HideAll();
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        
        CurrentColor = color;
        CurrentState = State.Appearing;

        if (_black != null) _black.SetActive(color == Color.Black);
        if (_white != null) _white.SetActive(color == Color.White);
        if (_dot != null) _dot.SetActive(false);

        transform.localRotation = Quaternion.identity;

        _stateChangedAt = DateTime.UtcNow;
        Game.Instance.PlayStoneAppearSe();
    }


    public void EnableDot()
    {
        gameObject.SetActive(true);
        HideAll();
        if (_dot != null) _dot.SetActive(true);
    }
    public void Reverse()
    {
        if (CurrentState == State.None)
        {
            Debug.LogError("Invalid Stone State");
            return;
        }

        switch (CurrentColor)
        {
            case Color.Black:
                CurrentColor = Color.White;
                if (this._black != null) this._black.SetActive(false);
                if (this._white != null) this._white.SetActive(true);
                break;
            case Color.White:
                CurrentColor = Color.Black;
                if (this._black != null) this._black.SetActive(true);
                if (this._white != null) this._white.SetActive(false);
                break;
        }
        this.CurrentState = State.Reversing;
        this._stateChangedAt = DateTime.UtcNow;
        Game.Instance.PlayStoneReverseSe();
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case State.Appearing:
{
    var startPos = _baseLocalPosition;
    startPos.y = 3f;

    var endPos = _baseLocalPosition;

    var t = Mathf.Clamp01(ElapsedSecondsSinceStateChange / AppeareSeconds);
    t = 1 - t * t * t * t;

    transform.localPosition = Vector3.Lerp(startPos, endPos, t);

    if (AppeareSeconds < ElapsedSecondsSinceStateChange)
    {
        transform.localPosition = endPos;
        CurrentState = State.Fix;
    }
}
break;

            case State.Reversing:
{
    // 瞬間的に色を切り替え、位置を確実にリセット
    transform.localPosition = _baseLocalPosition;
    transform.localRotation = Quaternion.identity;
    CurrentState = State.Fix;
}
break;

            case State.None:
            case State.Fix:
                break;
        }
    }
}