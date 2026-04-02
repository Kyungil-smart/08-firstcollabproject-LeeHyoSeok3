using UnityEngine;
using System.Collections.Generic;

public class InventoryEquipController : MonoBehaviour
{
    [SerializeField] private InventoryEquipView m_view;
    [SerializeField] private DataManager m_dataManager;

    // 💡 View에서 마지막으로 클릭한 아이템의 ID를 기억해둘 컨트롤러의 '뇌(기억장치)' 입니다.
    private int m_currentSelectedItemID = -1;

    private void Start()
    {
        // View의 버튼 이벤트 구독
        m_view.OnSlotClicked += HandleSlotClicked;
        m_view.OnEquipClicked += HandleEquipClicked;
        m_view.OnCloseClicked += HandleCloseClicked;
        m_view.OnUnlockClicked += HandleUnlockClicked;

        // 시작 시 인벤토리 목록 갱신
        RefreshUI();
    }

    private void RefreshUI()
    {
        List<ItemData> items = m_dataManager.GetInventoryItems();
        m_view.DrawSlots(items);
    }

    private void HandleSlotClicked(int slotIndex)
    {
        List<ItemData> items = m_dataManager.GetInventoryItems();
        if (slotIndex >= 0 && slotIndex < items.Count)
        {
            ItemData clickedItem = items[slotIndex];

            // 💡 나중에 '장착' 버튼을 누를 때를 대비해서, 방금 클릭한 아이템의 ID를 기억해 둡니다!
            m_currentSelectedItemID = clickedItem.id;

            bool isEquipped = (m_dataManager.GetEquippedItemID() == clickedItem.id);

            // View에게 선택된 아이템 정보를 넘겨주며 그려달라고 요청
            m_view.ShowDetail(clickedItem, isEquipped);
        }
    }

    private void HandleEquipClicked()
    {
        // 💡 기억해둔 ID가 제대로 있는지(선택된 상태인지) 확인하고 장착을 실행합니다!
        if (m_currentSelectedItemID != -1)
        {
            Debug.Log($"[Controller] 아이템 장착 명령 전달: ID {m_currentSelectedItemID}");

            // 1. DataManager에 실제 파티 장착 요청
            m_dataManager.EquipItemToParty(m_currentSelectedItemID);

            // 2. 기획서 사양: 장착 후에는 상세 팝업창 닫기
            m_view.HideDetail();

            // 3. UI 새로고침 (장착 중인 아이템 표시가 달라져야 하므로)
            RefreshUI();
        }
    }

    private void HandleUnlockClicked(int itemID)
    {
        // 1. 모델 해금 처리
        m_dataManager.UnlockItem(itemID);

        // 2. UI 새로고침 (회색 -> 밝은 색으로 갱신)
        RefreshUI();

        // 3. 해금된 아이템 상세 정보 정상적으로 다시 보여주기
        ItemData unlockedItem = m_dataManager.GetItemByID(itemID);
        bool isEquipped = (m_dataManager.GetEquippedItemID() == itemID);
        m_view.ShowDetail(unlockedItem, isEquipped);
    }

    private void HandleCloseClicked()
    {
        // 팝업 닫기
        gameObject.SetActive(false);
    }
}