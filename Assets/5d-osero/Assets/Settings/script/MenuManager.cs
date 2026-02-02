using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnPlayerVsAIButtonPressed()
    {
        // ゲームモードを PlayerVsAI に設定
        GameModeData.SelectedMode = Game.GameMode.PlayerVsAI;
        
        // ゲームシーンに遷移
        SceneManager.LoadScene("SampleScene");
    }

    public void OnPlayerVsPlayerButtonPressed()
    {
        // ゲームモードを PlayerVsPlayer に設定
        GameModeData.SelectedMode = Game.GameMode.PlayerVsPlayer;
        
        // ゲームシーンに遷移
        SceneManager.LoadScene("SampleScene");
    }
}
