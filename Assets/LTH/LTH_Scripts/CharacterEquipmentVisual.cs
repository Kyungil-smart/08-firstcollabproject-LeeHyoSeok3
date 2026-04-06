using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class CharacterEquipmentVisual : MonoBehaviour
{
    [SerializeField] private DataManager dataManager;
    [SerializeField] private CharacterId characterId;
    [Header("Primary Weapon")]
    [FormerlySerializedAs("weaponImage")]
    [SerializeField] private Image primaryWeaponImage;
    [FormerlySerializedAs("defaultWeaponSprite")]
    [SerializeField] private Sprite defaultPrimaryWeaponSprite;

    [Header("Secondary Weapon")]
    [SerializeField] private Image secondaryWeaponImage;
    [SerializeField] private Sprite defaultSecondaryWeaponSprite;

    private void OnEnable()
    {
        BindDataManagerIfNeeded();

        if (dataManager != null)
        {
            dataManager.OnEquippedItemChanged += HandleEquippedItemChanged;
        }

        ApplyCurrentEquipmentVisual();
    }

    private void OnDisable()
    {
        if (dataManager != null)
        {
            dataManager.OnEquippedItemChanged -= HandleEquippedItemChanged;
        }
    }

    private void HandleEquippedItemChanged(GearsetRecipeSO equippedRecipe)
    {
        ApplyEquipmentVisual(equippedRecipe);
    }

    private void ApplyCurrentEquipmentVisual()
    {
        if (dataManager == null)
        {
            ApplyDefaultVisual();
            return;
        }

        ApplyEquipmentVisual(dataManager.GetEquippedRecipe());
    }

    private void ApplyEquipmentVisual(GearsetRecipeSO equippedRecipe)
    {
        if (primaryWeaponImage == null && secondaryWeaponImage == null)
            return;

        if (equippedRecipe == null || equippedRecipe.characterWeaponSprites == null)
        {
            ApplyDefaultVisual();
            return;
        }

        for (int i = 0; i < equippedRecipe.characterWeaponSprites.Count; i++)
        {
            CharacterWeaponSpriteData weaponData = equippedRecipe.characterWeaponSprites[i];
            if (weaponData == null || weaponData.characterId != characterId)
                continue;

            ApplyWeaponSprites(weaponData);
            return;
        }

        ApplyDefaultVisual();
    }

    private void ApplyWeaponSprites(CharacterWeaponSpriteData weaponData)
    {
        if (primaryWeaponImage != null)
        {
            primaryWeaponImage.sprite = weaponData.primaryWeaponSprite != null
                ? weaponData.primaryWeaponSprite
                : defaultPrimaryWeaponSprite;
        }

        if (secondaryWeaponImage != null)
        {
            secondaryWeaponImage.sprite = weaponData.secondaryWeaponSprite != null
                ? weaponData.secondaryWeaponSprite
                : defaultSecondaryWeaponSprite;
        }
    }

    private void ApplyDefaultVisual()
    {
        if (primaryWeaponImage != null)
        {
            primaryWeaponImage.sprite = defaultPrimaryWeaponSprite;
        }

        if (secondaryWeaponImage != null)
        {
            secondaryWeaponImage.sprite = defaultSecondaryWeaponSprite;
        }
    }

    private void BindDataManagerIfNeeded()
    {
        if (dataManager == null)
        {
            dataManager = FindFirstObjectByType<DataManager>();
        }
    }
}