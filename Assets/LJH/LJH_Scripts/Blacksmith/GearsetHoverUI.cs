using UnityEngine;
using UnityEngine.EventSystems;

public class GearsetHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GearsetTooltipUI tooltipUI;
    [SerializeField] private GearsetInventory gearsetInventory;
    [SerializeField] private GearsetSlotUI slotUI;

    private void Awake()
    {
        if (slotUI == null)
            slotUI = GetComponentInParent<GearsetSlotUI>();
    }

    private GearsetRecipeSO GetRecipe()
    {
        if (slotUI == null)
            return null;

        return slotUI.GetRecipe();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("마우스 진입");

        if (tooltipUI == null)
        {
            Debug.LogError("tooltipUI가 null");
            return;
        }

        GearsetRecipeSO recipe = GetRecipe();
        if (recipe == null)
        {
            Debug.LogError("recipe가 null");
            return;
        }

        if (gearsetInventory != null && gearsetInventory.IsCrafted(recipe))
        {
            Debug.Log("이미 제작된 장비 → 툴팁 표시 안함");
            return;
        }

        Debug.Log($"툴팁 표시 시도: {recipe.gearsetName}");
        tooltipUI.ShowTooltip(recipe, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("마우스 나감");

        if (tooltipUI == null)
        {
            Debug.LogError("tooltipUI가 null");
            return;
        }

        tooltipUI.HideTooltip();
    }
}