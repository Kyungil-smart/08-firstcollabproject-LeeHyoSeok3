using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Gearset Recipe")]
public class GearsetRecipeSO : ScriptableObject
{
    [Header("Save")]
    public string saveId;

    [Header("Gear Info")]
    public string gearsetName;
    [TextArea] public string gearDescription;
    public Sprite gearIcon;

    [Header("Trait Info")]
    public string traitKey;
    public string traitName;
    [TextArea] public string traitDescription;
    public Sprite traitIcon;

    [Header("Recipe")]
    public List<MaterialRequirement> requirements = new List<MaterialRequirement>();
}

[System.Serializable]
public class MaterialRequirement
{
    public MaterialDataSO material;
    public int requiredCount;
}
