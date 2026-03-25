using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Level Select")]
    [Tooltip("Assign the level select UI panel (optional). Buttons can call ShowLevelSelect/HideLevelSelect.")]
    public GameObject levelSelectPanel;

    public void PlayGame()
    {
        LoadLevel(1); // default start scene (build index 1)
    }

    public void QuitGame()
    {
        Application.Quit(); // Works only in build
    }

    // Load a level by build index
    public void LoadLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    // Load by scene name if you prefer
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Show/hide the level select UI panel
    public void ShowLevelSelect()
    {
        Debug.Log("MainMenu: ShowLevelSelect called");
        if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
        else Debug.LogWarning("MainMenu: levelSelectPanel is not assigned in the Inspector.");
    }

    public void HideLevelSelect()
    {
        Debug.Log("MainMenu: HideLevelSelect called");
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        else Debug.LogWarning("MainMenu: levelSelectPanel is not assigned in the Inspector.");
    }
}