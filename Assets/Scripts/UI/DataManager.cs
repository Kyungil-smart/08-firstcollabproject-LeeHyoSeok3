using UnityEngine;
using System.Collections.Generic;

// 2. 데이터 관리 매니저 (더미 데이터 예시)
public class DataManager : MonoBehaviour
{
    private Dictionary<int, ItemData> itemDatabase = new Dictionary<int, ItemData>();
    private List<ItemData> inventory = new List<ItemData>();
    private int partyEquippedItemID = -1; // 현재 파티에 장착된 아이템 ID

    void Awake()
    {
        LoadDummyData();
    }

    private void LoadDummyData()
    {
        // (가정) CSV에서 읽어온 원본 데이터 로드
        ItemData item1 = new ItemData { id = 1, name = "녹슨 대검", description = "초보용 전사의 무기. 공격력 +5.", icon = Resources.Load<Sprite>("Icons/Item_01") };
        ItemData item2 = new ItemData { id = 2, name = "수련의 지팡이", description = "마법의 힘이 깃든 나무 지팡이. 마력 +7.", icon = Resources.Load<Sprite>("Icons/Item_02") };
        ItemData item3 = new ItemData { id = 3, name = "가죽 갑옷", description = "가볍고 질긴 가죽 갑옷. 방어력 +3.", icon = Resources.Load<Sprite>("Icons/Item_03") };

        itemDatabase.Add(item1.id, item1);
        itemDatabase.Add(item2.id, item2);
        itemDatabase.Add(item3.id, item3);

        // (가정) 인벤토리에 이 아이템들을 가지고 있다고 침
        inventory.Add(item1);
        inventory.Add(item2);
        inventory.Add(item3);
    }

    // 팝업에서 호출할 메서드들
    public List<ItemData> GetInventoryItems()
    {
        return inventory;
    }

    public ItemData GetItemByID(int id)
    {
        if (itemDatabase.TryGetValue(id, out ItemData data))
            return data;
        return default;
    }

    public void EquipItemToParty(int itemID)
    {
        Debug.Log($"[DataManager] 파티에 아이템 장착 시도: ID {itemID}");

        if (itemDatabase.ContainsKey(itemID))
        {
            partyEquippedItemID = itemID;
            Debug.Log($"장착 성공! 현재 파티 아이템: {itemDatabase[itemID].name}");

            // (참고) 이전 커밋에서 이야기한 CSV 데이터를 이용해서 실제 파티 스탯에 반영하는 논리를 여기에 구현
            // 예: System_Data.csv의 파티 공격력/방어력을 업데이트
        }
    }
}