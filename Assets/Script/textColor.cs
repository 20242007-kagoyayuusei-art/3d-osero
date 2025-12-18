using UnityEngine;
using UnityEngine.UI;

public class textColor : MonoBehaviour
{
    // Unityのインスペクター画面から好きな色を選べるようになります
    public Color targetColor = Color.red;

    void Start()
    {
        Text myText = GetComponent<Text>();
        if (myText != null)
        {
            myText.color = targetColor;
        }
    }
}