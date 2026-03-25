using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class InventoryEquipmentPopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject slotContainer; // GridLayoutGroup이 붙은 곳
    [SerializeField] private Button closeButton;
    [SerializeField] private InventorySlot slotPrefab;
    [SerializeField] private Sprite emptySlotSprite;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private const int TOTAL_SLOTS = 8; // 2x4 = 8

    // (임시) 데이터 매니저 예시
    [SerializeField] private DataManager dummyDataManager;

    void Start()
    {
        closeButton.onClick.AddListener(ClosePopup);
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        // 프리팹으로 8개 슬롯 미리 생성
        for (int i = 0; i < TOTAL_SLOTS; i++)
        {
             InventorySlot newSlot = Instantiate(slotPrefab, slotContainer.transform);
            newSlot.SetEmpty(emptySlotSprite);
            slots.Add(newSlot);
        }
    }

    public void OpenPopup()
    {
        gameObject.SetActive(true);
        RefreshSlots();
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    // 인벤토리 또는 파티 데이터를 바탕으로 슬롯 갱신
    public void RefreshSlots()
    {
        // (가정) Dummy 데이터에서 현재 장착 가능한 아이템 목록을 가져온다고 침
        List<ItemData> inventoryItems = dummyDataManager.GetInventoryItems();

        for (int i = 0; i < TOTAL_SLOTS; i++)
        {
            if (i < inventoryItems.Count)
            {
                // 데이터가 있는 경우 슬롯 설정, 클릭 시 detail popup 열기
                slots[i].SetSlot(inventoryItems[i].id, inventoryItems[i].icon, (itemID) =>
                {
                    ItemData selectedItem = dummyDataManager.GetItemByID(itemID);
                });
            }
            else
            {
                // 데이터가 없는 빈 슬롯
                slots[i].SetEmpty(emptySlotSprite);
            }
        }
    }

    // 상세 팝업에서 장착 버튼을 눌렀을 때 호출됨
    private void OnItemEquipped(int itemID)
    {
        Debug.Log($"장착 완료: {itemID}");

        // 데이터 매니저를 통해 실제 파티 데이터에 장착하는 로직 추가
        dummyDataManager.EquipItemToParty(itemID);

        // 팝업 모두 닫기
        ClosePopup();
    }
}
