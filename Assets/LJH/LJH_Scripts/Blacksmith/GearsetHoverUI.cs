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
        if (tooltipUI == null)
            return;

        GearsetRecipeSO recipe = GetRecipe();
        if (recipe == null)
            return;

        if (slotUI != null && slotUI.IsCrafted())
            return;

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