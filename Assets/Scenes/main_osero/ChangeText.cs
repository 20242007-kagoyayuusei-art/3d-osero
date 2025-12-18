using TMPro;
using UnityEngine;

public class ChangeText : MonoBehaviour
{
    public TMP_Text targetText;
    public string newText;

    public void SetText()
    {
        targetText.text = newText;
    }
}
