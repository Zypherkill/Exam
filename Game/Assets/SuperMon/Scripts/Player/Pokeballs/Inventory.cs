using UnityEngine;
using System;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxPokeBalls = 5;

    private int currentPokeBalls;
    private List<CaughtPokemonData> caughtPokemon = new List<CaughtPokemonData>();

    public static Inventory Instance { get; private set; }

    public event Action<int> OnPokeBallCountChanged;
    public event Action<List<CaughtPokemonData>> OnCaughtPokemonChanged;

    public int CurrentPokeBalls => currentPokeBalls;
    public int MaxPokeBalls => maxPokeBalls;
    public List<CaughtPokemonData> CaughtPokemon => caughtPokemon;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Ladda pokebollar från sparad lagring
        if (PlayerPrefs.HasKey("PokeBallCount"))
        {
            currentPokeBalls = PlayerPrefs.GetInt("PokeBallCount");
            Debug.Log("Laddade pokebollar från PlayerPrefs: " + currentPokeBalls);
        }
        else
        {
            currentPokeBalls = 0;
            Debug.Log("Startar utan pokebollar: 0");
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
        Debug.Log("Plockade upp en pokeboll! Totalt: " + currentPokeBalls);
    }

    public void CatchPokemon(PokemonData pokemonData)
    {
        if (pokemonData != null)
        {
            CaughtPokemonData caught = new CaughtPokemonData(pokemonData);
            caughtPokemon.Add(caught);
            Debug.Log("✓ Fångade: " + caught.GetPokemonName() + " | Total fångade: " + caughtPokemon.Count);
            OnCaughtPokemonChanged?.Invoke(caughtPokemon);
        }
    }

    public void ResetInventory()
    {
        currentPokeBalls = maxPokeBalls;
        caughtPokemon.Clear();
        PlayerPrefs.SetInt("PokeBallCount", currentPokeBalls);
        OnPokeBallCountChanged?.Invoke(currentPokeBalls);
        OnCaughtPokemonChanged?.Invoke(caughtPokemon);
    }
}