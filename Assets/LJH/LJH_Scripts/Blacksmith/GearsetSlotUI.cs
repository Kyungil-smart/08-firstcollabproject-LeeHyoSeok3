using UnityEngine;
using UnityEngine.UI;

public class GearsetSlotUI : MonoBehaviour
{
    [SerializeField] private GearsetRecipeSO recipe;
    [SerializeField] private GearsetCraftPopupUI popupUI;
    [SerializeField] private MaterialInventory materialInventory;
    [SerializeField] private GearsetInventory gearsetInventory;
    [SerializeField] private Button slotButton;
    [SerializeField] private Image slotImage;

    [Header("제작 상태 색상")]
    [SerializeField] private Color craftedColor = Color.white;
    [SerializeField] private Color notCraftedColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    private void Start()
    {
        RefreshState();
    }

    public void OnClickSlot()
    {
        Debug.Log("슬롯 클릭됨");

        if (recipe == null)
        {
            Debug.LogWarning("recipe가 연결되지 않았습니다.");
            return;
        }

        if (gearsetInventory != null && gearsetInventory.IsCrafted(recipe))
        {
            Debug.Log("이미 제작 완료된 장비입니다.");
            return;
        }

        if (materialInventory == null)
        {
            Debug.LogWarning("materialInventory가 연결되지 않았습니다.");
            return;
        }

        if (popupUI == null)
        {
            Debug.LogWarning("popupUI가 연결되지 않았습니다.");
            return;
        }

        if (!materialInventory.CanCraft(recipe))
        {
            Debug.Log("재료 부족 -> 팝업 열리지 않음");
            return;
        }

        popupUI.Show(recipe, materialInventory, this);
    }

    public void MarkCrafted()
    {
        if (gearsetInventory != null)
        {
            gearsetInventory.AddCrafted(recipe);
        }

        RefreshState();
    }

    public void RefreshState()
    {
        bool isCrafted = gearsetInventory != null && gearsetInventory.IsCrafted(recipe);

        if (slotImage != null)
        {
            slotImage.color = isCrafted ? craftedColor : notCraftedColor;
        }

        if (slotButton != null)
        {
            slotButton.interactable = true;
        }
    }

    public bool IsCrafted()
    {
        return gearsetInventory != null && gearsetInventory.IsCrafted(recipe);
    }
}