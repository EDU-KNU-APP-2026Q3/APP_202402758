using TMPro;
using UnityEngine;

public sealed class StarlightSceneUI : MonoBehaviour
{
    public TMP_Text roundScore;

    void Start()
    {
        if (roundScore != null && ScoreManager.Instance != null)
            roundScore.text = ScoreManager.Instance.CurrentScore.ToString("N0");
    }

    public void StartRound() => GameManager.Instance.RestartGame();
    public void GoToMenu() => GameManager.Instance.QuitToMainMenu();
}
