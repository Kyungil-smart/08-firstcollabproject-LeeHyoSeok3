using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GearsetRecipeDatabase", menuName = "Blacksmith/Gearset Recipe Database")]
public class GearsetRecipeDatabaseSO : ScriptableObject
{
    public List<GearsetRecipeSO> recipes = new List<GearsetRecipeSO>();

    public GearsetRecipeSO GetRecipeByName(string gearsetName)
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            if (recipes[i] != null && recipes[i].GetGearsetName() == gearsetName)
                return recipes[i];
        }

        return null;
    }
}