using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayThenLoad : MonoBehaviour
{
    [Tooltip("Scene name to load after the intro finishes")]
    public string nextScene;

    [Tooltip("Seconds to wait before loading next scene")]
    public float duration = 5f;

    void Start()
    {
        // Animations are expected to play automatically when the scene starts.
        StartCoroutine(WaitAndLoad());
    }

    IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(duration);

        if (!string.IsNullOrEmpty(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogWarning("PlayThenLoad: 'nextScene' is not set on " + gameObject.name);
        }
    }
}
