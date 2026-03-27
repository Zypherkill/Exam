using System;

[System.Serializable]
public class CaughtPokemonData
{
    public PokemonData.PokemonType type;
    public DateTime caughtTime;

    public CaughtPokemonData(PokemonData source)
    {
        if (source != null)
        {
            type = source.Type;
            caughtTime = DateTime.Now;
        }
    }

    public string GetPokemonName()
    {
        return type.ToString();
    }
}
