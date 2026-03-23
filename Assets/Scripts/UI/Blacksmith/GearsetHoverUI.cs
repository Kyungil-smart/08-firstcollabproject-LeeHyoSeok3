using UnityEngine;
using UnityEngine.EventSystems;

public class GearsetHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GearsetTooltipUI tooltipUI;

    private string gearsetName;
    private GearsetRecipeSO recipe;

    public void SetData(string newGearsetName, GearsetRecipeSO newRecipe)
    {
        gearsetName = newGearsetName;
        recipe = newRecipe;
    }

    public void ClearData()
    {
        gearsetName = "";
        recipe = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipUI == null || recipe == null)
            return;

        tooltipUI.ShowTooltip(recipe, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipUI == null)
            return;

        tooltipUI.HideTooltip();
    }
}