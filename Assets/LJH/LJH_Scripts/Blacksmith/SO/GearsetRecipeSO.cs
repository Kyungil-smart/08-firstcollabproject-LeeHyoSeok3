using System.Collections.Generic;
using UnityEngine;

public enum CharacterId
{
    Elf,
    Cat,
    Dwarf,
    Human
}

[System.Serializable]
public class CharacterWeaponSpriteData
{
    public CharacterId characterId;
    public Sprite weaponSprite;
}

[CreateAssetMenu(menuName = "Game/Gearset Recipe")]
public class GearsetRecipeSO : ScriptableObject
{
    [Header("Save")]
    public string saveId;

    [Header("Gear Info")]
    public string gearsetNameKey;
    [TextArea] public string gearDescriptionKey;
    public Sprite gearIcon;

    [Header("Trait Info")]
    public string traitKey;
    public string traitNameKey;
    [TextArea] public string traitDescriptionKey;
    public Sprite traitIcon;

    [Header("Character Weapon Visuals")]
    public List<CharacterWeaponSpriteData> characterWeaponSprites = new List<CharacterWeaponSpriteData>();

    [Header("Recipe")]
    public List<MaterialRequirement> requirements = new List<MaterialRequirement>();

    public string GetGearsetName()
    {
        return GetLocalizedText(gearsetNameKey);
    }

    public string GetGearDescription()
    {
        return GetLocalizedText(gearDescriptionKey);
    }

    public string GetTraitName()
    {
        return GetLocalizedText(traitNameKey);
    }

    public string GetTraitDescription()
    {
        return GetLocalizedText(traitDescriptionKey);
    }

    private string GetLocalizedText(string key)
    {
        if (string.IsNullOrEmpty(key))
            return string.Empty;

        if (LocalizationManager.Instance == null)
            return key;

        return LocalizationManager.Instance.GetText(key);
    }
}

[System.Serializable]
public class MaterialRequirement
{
    public MaterialDataSO material;
    public int requiredCount;
}