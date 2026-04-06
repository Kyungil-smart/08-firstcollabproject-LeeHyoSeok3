using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GearsetCraftPopupUI : MonoBehaviour
{
    [Header("Gear UI")]
    [SerializeField] private TextMeshProUGUI gearsetNameText;
    [SerializeField] private Image gearIconImage;

    [Header("Trait UI")]
    [SerializeField] private TextMeshProUGUI traitNameText;
    [SerializeField] private Image traitIconImage;

    [Header("Tooltip Hover")]
    [SerializeField] private TooltipHoverUI gearIconHover;
    [SerializeField] private TooltipHoverUI traitIconHover;

    [SerializeField] private MaterialInventoryUI inventoryUI;
    [SerializeField] private DataManager dataManager; // 제작된 아이템이 파티 장착 시스템과 연동

    private GearsetRecipeSO currentRecipe;
    private MaterialInventory currentInventory;
    private GearsetSlotUI currentSlot;

    private Coroutine tooltipCheckCoroutine;

    private void Start()
    {
        gameObject.SetActive(false);

        if (dataManager == null) dataManager = FindFirstObjectByType<DataManager>();
    }

    public void Show(GearsetRecipeSO recipe, MaterialInventory inventory, GearsetSlotUI slot)
    {
        if (recipe == null)
        {
            Debug.LogWarning("recipe null");
            return;
        }

        currentRecipe = recipe;
        currentInventory = inventory;
        currentSlot = slot;

        if (gearsetNameText != null)
            gearsetNameText.text = recipe.gearsetName;

        if (gearIconImage != null)
            gearIconImage.sprite = recipe.gearIcon;

        if (traitNameText != null)
            traitNameText.text = recipe.traitName;

        if (traitIconImage != null)
            traitIconImage.sprite = recipe.traitIcon;
        
        if (gearIconHover != null)
            gearIconHover.SetTooltip(recipe.gearDescription);

        if (traitIconHover != null)
            traitIconHover.SetTooltip(recipe.traitDescription);

        gameObject.SetActive(true);

        Canvas.ForceUpdateCanvases();

        if (tooltipCheckCoroutine != null)
            StopCoroutine(tooltipCheckCoroutine);

        tooltipCheckCoroutine = StartCoroutine(CheckTooltipRepeatedly());
    }

    private IEnumerator CheckTooltipRepeatedly()
    {
        // UI가 안정화될 시간을 잠깐 줌
        for (int i = 0; i < 5; i++)
        {
            yield return null;
            Canvas.ForceUpdateCanvases();

            if (gearIconHover != null)
                gearIconHover.CheckAndShowTooltip();

            if (traitIconHover != null)
                traitIconHover.CheckAndShowTooltip();
        }

        tooltipCheckCoroutine = null;
    }

    public void OnClickCraft()
    {
        if (currentRecipe == null || currentInventory == null || currentSlot == null)
            return;

        if (!currentInventory.CanCraft(currentRecipe))
            return;

        currentInventory.Consume(currentRecipe);
        currentSlot.MarkCrafted();

        SoundManager.Instance?.OneShot("CraftExecute");

        if (dataManager != null)
            dataManager.MarkItemCraftedBySaveId(currentRecipe.saveId);

        if (inventoryUI != null)
            inventoryUI.RefreshUI();

        Hide();
    }

    public void Hide()
    {
        if (tooltipCheckCoroutine != null)
        {
            StopCoroutine(tooltipCheckCoroutine);
            tooltipCheckCoroutine = null;
        }

        gameObject.SetActive(false);
    }
}
