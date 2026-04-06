using UnityEngine;
using UnityEngine.UI;

public class CharacterEquipmentVisual : MonoBehaviour
{
    [SerializeField] private DataManager dataManager;
    [SerializeField] private CharacterId characterId;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Sprite defaultWeaponSprite;

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
        if (weaponImage == null)
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

            weaponImage.sprite = weaponData.weaponSprite != null ? weaponData.weaponSprite : defaultWeaponSprite;
            return;
        }

        ApplyDefaultVisual();
    }

    private void ApplyDefaultVisual()
    {
        if (weaponImage != null)
        {
            weaponImage.sprite = defaultWeaponSprite;
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
