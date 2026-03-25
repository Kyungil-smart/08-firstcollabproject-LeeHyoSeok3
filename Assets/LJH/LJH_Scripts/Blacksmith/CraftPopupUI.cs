using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GearsetCraftPopupUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gearsetNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private MaterialInventoryUI inventoryUI;

    private GearsetRecipeSO currentRecipe;
    private MaterialInventory currentInventory;
    private GearsetSlotUI currentSlot;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(GearsetRecipeSO recipe, MaterialInventory inventory, GearsetSlotUI slot)
    {
        if (recipe == null)
        {
            Debug.LogWarning("GearsetCraftPopupUI: recipe가 null입니다.");
            return;
        }

        currentRecipe = recipe;
        currentInventory = inventory;
        currentSlot = slot;

        gearsetNameText.text = recipe.gearsetName;
        descriptionText.text = recipe.description;

        gameObject.SetActive(true);
    }

    public void OnClickCraft()
    {
        Debug.Log("제작 버튼 클릭됨");

        if (currentRecipe == null || currentInventory == null || currentSlot == null)
            return;

        if (!currentInventory.CanCraft(currentRecipe))
            return;

        currentInventory.Consume(currentRecipe);
        currentSlot.MarkCrafted();
        
        if (inventoryUI != null)
        {
            Debug.Log("UI 갱신 호출");
            inventoryUI.RefreshUI();
        }
        else
        {
            Debug.LogError("inventoryUI 연결 안됨");
        }

        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}