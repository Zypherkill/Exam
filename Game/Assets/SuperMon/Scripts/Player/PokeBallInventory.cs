using UnityEngine;
using System;

public class PokeBallInventory : MonoBehaviour
{
    [SerializeField] private int maxPokeBalls = 5;
    [SerializeField] private int startingPokeBalls = 5;

    private int currentPokeBalls;

    public static PokeBallInventory Instance { get; private set; }

    public event Action<int> OnPokeBallCountChanged;

    public int CurrentPokeBalls => currentPokeBalls;
    public int MaxPokeBalls => maxPokeBalls;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Load pokeballs from persistent storage, or use starting amount if first time
        if (PlayerPrefs.HasKey("PokeBallCount"))
        {
            currentPokeBalls = PlayerPrefs.GetInt("PokeBallCount");
            Debug.Log("Loaded pokeballs from PlayerPrefs: " + currentPokeBalls);
        }
        else
        {
            currentPokeBalls = startingPokeBalls;
            Debug.Log("Starting with pokeballs: " + startingPokeBalls);
        }

        OnPokeBallCountChanged?.Invoke(currentPokeBalls);
    }

    public bool UsePokeBall()
    {
        if (currentPokeBalls > 0)
        {
            currentPokeBalls--;
            PlayerPrefs.SetInt("PokeBallCount", currentPokeBalls);
            OnPokeBallCountChanged?.Invoke(currentPokeBalls);
            return true;
        }
        return false;
    }

    public void AddPokeBall(int amount = 1)
    {
        currentPokeBalls = Mathf.Min(currentPokeBalls + amount, maxPokeBalls);
        PlayerPrefs.SetInt("PokeBallCount", currentPokeBalls);
        OnPokeBallCountChanged?.Invoke(currentPokeBalls);
        Debug.Log("Picked up pokeball! Total: " + currentPokeBalls);
    }

    public void ResetInventory()
    {
        currentPokeBalls = startingPokeBalls;
        PlayerPrefs.SetInt("PokeBallCount", currentPokeBalls);
        OnPokeBallCountChanged?.Invoke(currentPokeBalls);
    }
}