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
        Debug.Log($"SetRecipe 호출됨: {(recipe != null ? recipe.gearsetName : "NULL")}");
    }

    public void ClearData()
    {
        gearsetName = "";
        recipe = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("마우스 진입");
        if (tooltipUI == null || recipe == null)
            return;

        tooltipUI.ShowTooltip(recipe, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("마우스 나감");
        if (tooltipUI == null)
            return;

        tooltipUI.HideTooltip();
    }
}