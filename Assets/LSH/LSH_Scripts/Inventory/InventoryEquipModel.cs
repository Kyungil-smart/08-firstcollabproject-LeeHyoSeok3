using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;

public class InventoryEquipModel
{
    // 데이터 상태 보관
    private List<ItemData> m_inventoryItems = new List<ItemData>();
    private ItemData m_selectedItem;

    // 데이터가 변경되었음을 외부에 알리는 이벤트 (Controller가 구독함)
    public event Action<List<ItemData>> OnInventoryUpdated;
    public event Action<ItemData> OnItemSelected;

    // 인벤토리 목록 업데이트
    public void UpdateInventory(List<ItemData> items)
    {
        m_inventoryItems = items;
        OnInventoryUpdated?.Invoke(m_inventoryItems); // 리스트 갱신 알림
    }

    // 아이템 선택 처리
    public void SelectItem(int index)
    {
        if (index >= 0 && index < m_inventoryItems.Count)
        {
            m_selectedItem = m_inventoryItems[index];
            OnItemSelected?.Invoke(m_selectedItem); // 선택된 아이템 변경 알림
        }
    }

    // 현재 선택된 아이템 반환
    public ItemData GetSelectedItem()
    {
        return m_selectedItem;
    }
}