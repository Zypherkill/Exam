using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public string scorePrefix = "Score: ";

    private void Start()
    {
        // Subscribe to score change events
        if (ScoreSystem.instance != null)
        {
            ScoreSystem.OnScoreChanged += UpdateScoreDisplay;
            // Update display with initial score
            UpdateScoreDisplay(ScoreSystem.instance.GetScore());
        }
        else
        {
            Debug.LogWarning("ScoreSystem instance not found!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        ScoreSystem.OnScoreChanged -= UpdateScoreDisplay;
    }

    private void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = scorePrefix + newScore.ToString();
        }
    }
}
