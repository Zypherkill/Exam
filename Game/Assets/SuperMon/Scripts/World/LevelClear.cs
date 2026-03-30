using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class LevelClear : MonoBehaviour
{
    [Tooltip("Scene name to load when the trigger is activated. If empty, loads next build index.")]
    public string sceneToLoad = "";

#if UNITY_EDITOR
    [Tooltip("Editor-only: Scene asset to load while playing in the Editor without adding to Build Settings")]
    public SceneAsset sceneAsset;
#endif

    [Tooltip("Tag of the object that can trigger the level clear (e.g. Player)")]
    public string triggerTag = "Player";

    [Tooltip("Delay in seconds before loading the target scene")]
    public float delay = 0f;

    [Tooltip("Victory sound to play when level is cleared")]
    public AudioClip victorySFX;

    bool triggered = false;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogWarning("LevelClear: No AudioSource found!");
        if (victorySFX == null)
            Debug.LogWarning("LevelClear: No victorySFX assigned!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.CompareTag(triggerTag))
        {
            triggered = true;

            // Play victory sound
            if (audioSource != null && victorySFX != null)
                audioSource.PlayOneShot(victorySFX);

            // Calculate delay including sound length if using PlayOneShot
            float totalDelay = delay;
            if (audioSource != null && victorySFX != null && delay <= 0f)
                totalDelay = victorySFX.length; // Wait for sound to finish

            if (totalDelay <= 0f) LoadTarget(); else Invoke(nameof(LoadTarget), totalDelay);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag(triggerTag))
        {
            triggered = true;

            // Play victory sound
            if (audioSource != null && victorySFX != null)
                audioSource.PlayOneShot(victorySFX);

            // Calculate delay including sound length if using PlayOneShot
            float totalDelay = delay;
            if (audioSource != null && victorySFX != null && delay <= 0f)
                totalDelay = victorySFX.length; // Wait for sound to finish

            if (totalDelay <= 0f) LoadTarget(); else Invoke(nameof(LoadTarget), totalDelay);
        }
    }

    void LoadTarget()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            string path = AssetDatabase.GetAssetPath(sceneAsset);
            if (!string.IsNullOrEmpty(path))
            {
                var parameters = new LoadSceneParameters(LoadSceneMode.Single);
                EditorSceneManager.LoadSceneInPlayMode(path, parameters);
                return;
            }
        }
#endif

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
            return;
        }

        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            Debug.LogWarning("LevelClear: no next scene in Build Settings.");
    }
}
