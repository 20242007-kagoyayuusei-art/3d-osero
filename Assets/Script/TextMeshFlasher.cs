using UnityEngine;
using TMPro;

public class TextMeshFlasher : MonoBehaviour
{
    public float flashSpeed = 2f;
    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Color c = text.color;
        c.a = Mathf.PingPong(Time.time * flashSpeed, 1f);
        text.color = c;
    }
}
