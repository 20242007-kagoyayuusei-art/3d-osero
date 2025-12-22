using UnityEngine;
using UnityEngine.UI; // ButtonやImageを扱うために必要

public class ButtonFlasher : MonoBehaviour
{
    private Image buttonImage;
    public float flashSpeed = 2.0f; // 点滅の速さ

    void Start()
    {
        // ボタン自身のImageコンポーネントを取得
        buttonImage = GetComponent<Image>();
    }

    void Update()
    {
        if (buttonImage != null)
        {
            // Mathf.PingPongで0〜1の間を往復する値を作る
            float alpha = Mathf.PingPong(Time.time * flashSpeed, 1.0f);

            // 色を維持したまま、透明度(a)だけを変化させる
            Color color = buttonImage.color;
            color.a = alpha;
            buttonImage.color = color;
        }
    }
}