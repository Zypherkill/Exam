using UnityEngine;

public class PokemonData : MonoBehaviour
{
    public enum PokemonType
    {
        Pikachu,
        Squirtle
    }

    [SerializeField] private PokemonType pokemonType = PokemonType.Pikachu;

    public PokemonType Type => pokemonType;

    public string GetPokemonName()
    {
        return pokemonType.ToString();
    }

    public void CopyFrom(PokemonData source)
    {
        if (source == null) return;
        pokemonType = source.pokemonType;
    }
}