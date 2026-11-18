using TMPro;
using UnityEngine;

public class ScoreBoardUI : MonoBehaviour
{
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] TMP_Text[] rankTexts = new TMP_Text[3];
    [SerializeField] TMP_Text emptyMessage;

    void OnEnable()
    {
        var manager = ScoreManager.Instance;
        int count = manager == null ? 0 : manager.TopScores.Count;
        bool hasRecords = count != 0;

        if (emptyMessage != null)
        {
            highScoreText.text = hasRecords ? manager.HighScore.ToString("N0") : "—";
            emptyMessage.gameObject.SetActive(!hasRecords);
        }
        else
        {
            highScoreText.text = hasRecords
                ? $"최고 기록 {manager.HighScore}"
                : "아직 기록이 없습니다";
        }

        for (int i = 0; i < rankTexts.Length; i++)
        {
            string value = i < count ? manager.TopScores[i].ToString("N0") : "—";
            rankTexts[i].text = emptyMessage != null ? value : $"{i + 1}위  {value}";
        }
    }
}
