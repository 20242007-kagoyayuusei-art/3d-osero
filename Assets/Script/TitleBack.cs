
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleBack : MonoBehaviour
{
    public void BackBtn()
    {
        SceneManager.LoadScene("title");
    }
}
