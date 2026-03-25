using UnityEngine;
using UnityEngine.UI;

public class GearsetSlotUI : MonoBehaviour
{
    [SerializeField] private GearsetRecipeSO recipe;
    [SerializeField] private GearsetCraftPopupUI popupUI;
    [SerializeField] private MaterialInventory materialInventory;
    [SerializeField] private Button slotButton;
    [SerializeField] private GameObject focusObject;
    
    private bool isCrafted;

    private void Start()
    {
        if (focusObject != null)
            focusObject.SetActive(false);

        RefreshState();
    }

    public void OnClickSlot()
    {
        Debug.Log("슬롯 클릭됨");

        if (isCrafted)
        {
            Debug.Log("이미 제작 완료된 장비");
            return;
        }
        
        if (recipe == null)
        {
            Debug.LogWarning("recipe가 연결되지 않았습니다.");
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

        if (isCrafted)
        {
            Debug.Log("이미 제작 완료된 장비입니다.");
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
        isCrafted = true;

        if (focusObject != null)
            focusObject.SetActive(true);

        RefreshState();
    }

    public void RefreshState()
    {
        if (focusObject != null)
            focusObject.SetActive(isCrafted);

        if (slotButton != null)
            slotButton.interactable = true;
    }

    public bool IsCrafted()
    {
        return isCrafted;
    }
    
    
}