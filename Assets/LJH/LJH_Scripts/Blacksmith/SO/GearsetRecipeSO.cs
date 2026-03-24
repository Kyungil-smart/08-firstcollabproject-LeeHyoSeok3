using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GearsetRecipe", menuName = "Blacksmith/Gearset Recipe")]
public class GearsetRecipeSO : ScriptableObject
{
    public string gearsetName;
    public List<GearsetMaterialData> requiredMaterials = new List<GearsetMaterialData>();
}