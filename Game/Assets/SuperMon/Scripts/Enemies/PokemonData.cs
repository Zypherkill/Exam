using UnityEngine;

public class PokemonData : MonoBehaviour
{
    public enum PokemonType
    {
        Pikachu,
        Squirtle
    }

    [SerializeField] private PokemonType pokemonType = PokemonType.Pikachu;
    [SerializeField] private int level = 1;
    [SerializeField] private int experience = 0;

    public PokemonType Type => pokemonType;
    public int Level => level;
    public int Experience => experience;

    public string GetPokemonName()
    {
        return pokemonType.ToString();
    }

    public void SetLevel(int newLevel)
    {
        level = Mathf.Max(1, newLevel);
    }

    public void CopyFrom(PokemonData source)
    {
        if (source == null) return;
        pokemonType = source.pokemonType;
        level = source.level;
        experience = source.experience;
    }
}