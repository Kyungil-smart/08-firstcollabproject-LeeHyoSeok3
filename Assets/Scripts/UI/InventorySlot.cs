using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

// 3. 파티 슬롯 프리팹 스크립트
public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button slotButton;

    private int itemID;
    private Action<int> onClickAction; // 클릭 시 Item ID를 전달할 콜백

    public void SetSlot(int id, Sprite icon, Action<int> onClick)
    {
        itemID = id;
        iconImage.sprite = icon;
        iconImage.color = Color.white; // 비어 있지 않다면 불투명하게
        onClickAction = onClick;

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(OnClicked);
    }

    // 비어 있는 슬롯 설정
    public void SetEmpty(Sprite emptyIcon)
    {
        itemID = -1;
        iconImage.sprite = emptyIcon;
        iconImage.color = new Color(1, 1, 1, 0.5f); // 비어 있으면 투명하게
        slotButton.onClick.RemoveAllListeners();
    }

    private void OnClicked()
    {
        onClickAction?.Invoke(itemID);
    }
}