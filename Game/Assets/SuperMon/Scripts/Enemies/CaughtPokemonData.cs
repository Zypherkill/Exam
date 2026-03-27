using System;

[System.Serializable]
public class CaughtPokemonData
{
    public PokemonData.PokemonType type;
    public int level;
    public int experience;
    public DateTime caughtTime;

    public CaughtPokemonData(PokemonData source)
    {
        if (source != null)
        {
            type = source.Type;
            level = source.Level;
            experience = source.Experience;
            caughtTime = DateTime.Now;
        }
    }

    public string GetPokemonName()
    {
        return type.ToString();
    }
}
