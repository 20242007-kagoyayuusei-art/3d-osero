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
    SetActive(false, Color.Black);
    _baseLocalPosition = transform.localPosition;
    
    }

    public void SetActive(bool value, Color color)
    {
        if (value)
        {
            this.CurrentColor = color;
            this.CurrentState = State.Appearing;
            
            // 色に応じて正しいオブジェクトだけを有効化
            switch (color)
            {
                case Color.Black:
                    this._black.SetActive(true);
                    this._white.SetActive(false);
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Color.White:
                    this._black.SetActive(false);
                    this._white.SetActive(true);
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
            }
            
            this._dot.SetActive(false);
            this._stateChangedAt = DateTime.UtcNow;
            Game.Instance.PlayStoneAppearSe();
        }
        else
        {
            this.CurrentState = State.None;
        }

        gameObject.SetActive(value);
    }

    public void EnableDot()
    {
        this._black.SetActive(false);
        this._white.SetActive(false);
        this._dot.SetActive(true);
        gameObject.SetActive(true);
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
                this._black.SetActive(false);
                this._white.SetActive(true);
                break;
            case Color.White:
                CurrentColor = Color.Black;
                this._black.SetActive(true);
                this._white.SetActive(false);
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