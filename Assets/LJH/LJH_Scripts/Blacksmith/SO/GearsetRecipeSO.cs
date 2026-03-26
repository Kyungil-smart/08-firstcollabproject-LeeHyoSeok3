using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Gearset Recipe")]
public class GearsetRecipeSO : ScriptableObject
{
    public string gearsetName;
    [TextArea] public string description;

    public List<MaterialRequirement> requirements = new List<MaterialRequirement>();
}

[System.Serializable]
public class MaterialRequirement
{
    public MaterialDataSO material;
    public int requiredCount;
}

