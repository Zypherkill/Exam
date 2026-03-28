using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem instance { get; private set; }

    private int score = 0;
    private const string SCORE_KEY = "PlayerScore";

    // Delegate for score change events
    public delegate void ScoreChangeDelegate(int newScore);
    public static event ScoreChangeDelegate OnScoreChanged;

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Load score from PlayerPrefs
        if (PlayerPrefs.HasKey(SCORE_KEY))
        {
            score = PlayerPrefs.GetInt(SCORE_KEY);
            Debug.Log("Loaded score from PlayerPrefs: " + score);
        }
        else
        {
            score = 0;
            Debug.Log("No saved score, starting with: 0");
        }
    }

    public void AddPoints(int points)
    {
        score += points;
        PlayerPrefs.SetInt(SCORE_KEY, score);
        PlayerPrefs.Save();
        OnScoreChanged?.Invoke(score);
        Debug.Log($"Score updated: {score}");
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
        PlayerPrefs.DeleteKey(SCORE_KEY);
        PlayerPrefs.Save();
        OnScoreChanged?.Invoke(score);
        Debug.Log("Score reset to 0");
    }
}
