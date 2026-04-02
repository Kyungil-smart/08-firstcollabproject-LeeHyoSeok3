using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class InventoryEquipmentPopup : MonoBehaviour
{
    [Header("Left Panel - Slots")]
    [SerializeField] private GameObject slotContainer;
    [SerializeField] private Button closeButton;
    [SerializeField] private InventorySlot slotPrefab;
    [SerializeField] private Sprite emptySlotSprite;

    [Header("Right Panel - Detail Popup")]
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private Image detailBackgroundImage;
    [SerializeField] private Image detailIcon;            // 💡 장비 아이콘
    [SerializeField] private TextMeshProUGUI detailName;

    [SerializeField] private Image detailTraitIcon;       // 💡 특성 아이콘
    [SerializeField] private TextMeshProUGUI detailTraitName;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unlockButton;

    [Header("Tooltip (마우스오버 팝업)")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipName;
    [SerializeField] private TextMeshProUGUI tooltipDesc;

    [Header("Data")]
    [SerializeField] private DataManager dummyDataManager;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private const int TOTAL_SLOTS = 8;
    private Color originalDetailBgColor = Color.white;
    private int currentSelectedItemID = -1;

    void Start()
    {
        closeButton.onClick.AddListener(ClosePopup);
        if (equipButton != null) equipButton.onClick.AddListener(() => OnItemEquipped(currentSelectedItemID));
        if (unlockButton != null) unlockButton.onClick.AddListener(() => OnUnlockClicked(currentSelectedItemID));

        if (detailBackgroundImage != null) originalDetailBgColor = detailBackgroundImage.color;

        InitializeSlots();

        // 💡 듀얼 툴팁 세팅: 장비 아이콘과 특성 아이콘에 각각 다른 이벤트를 붙여줍니다.
        if (detailIcon != null) SetupTooltipTrigger(detailIcon.gameObject, true);       // true = 장비용
        if (detailTraitIcon != null) SetupTooltipTrigger(detailTraitIcon.gameObject, false); // false = 특성용

        if (detailPanel != null) detailPanel.SetActive(false);
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    // 💡 툴팁 이벤트 동적 생성 (장비인지 특성인지 구분해서 띄워줍니다)
    private void SetupTooltipTrigger(GameObject targetObj, bool isEquipment)
    {
        if (targetObj.GetComponent<EventTrigger>() == null)
            targetObj.AddComponent<EventTrigger>();

        EventTrigger trigger = targetObj.GetComponent<EventTrigger>();
        trigger.triggers.Clear(); // 중복 방지

        EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener((data) => {
            if (tooltipPanel == null || currentSelectedItemID == -1) return;

            ItemData item = dummyDataManager.GetItemByID(currentSelectedItemID);
            if (item == null || !item.isUnlocked) return; // 미해금 상태면 툴팁 차단

            // 💡 마우스가 올라간 대상에 따라 툴팁 내용을 다르게 세팅!
            if (isEquipment)
            {
                if (tooltipName != null) tooltipName.text = item.name;
                if (tooltipDesc != null) tooltipDesc.text = item.description;
            }
            else
            {
                if (tooltipName != null) tooltipName.text = item.traitName;
                if (tooltipDesc != null) tooltipDesc.text = item.traitDescription;
            }

            tooltipPanel.SetActive(true);
        });
        trigger.triggers.Add(enter);

        EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener((data) => { if (tooltipPanel != null) tooltipPanel.SetActive(false); });
        trigger.triggers.Add(exit);
    }

    private void InitializeSlots()
    {
        // ... (기존과 동일)
        for (int i = 0; i < TOTAL_SLOTS; i++)
        {
            InventorySlot newSlot = Instantiate(slotPrefab, slotContainer.transform);
            newSlot.SetEmpty(emptySlotSprite);
            slots.Add(newSlot);
        }
    }

    public void OpenPopup() { gameObject.SetActive(true); RefreshSlots(); }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
        if (detailPanel != null) detailPanel.SetActive(false);
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    public void RefreshSlots()
    {
        // ... (기존과 동일)
        List<ItemData> inventoryItems = dummyDataManager.GetInventoryItems();
        for (int i = 0; i < TOTAL_SLOTS; i++)
        {
            if (i < inventoryItems.Count)
                slots[i].SetSlot(inventoryItems[i].id, inventoryItems[i].icon, inventoryItems[i].isUnlocked, (itemID) => SelectSlot(itemID));
            else
                slots[i].SetEmpty(emptySlotSprite);
        }
    }

    private void SelectSlot(int itemID)
    {
        currentSelectedItemID = itemID;
        ItemData selectedItem = dummyDataManager.GetItemByID(itemID);
        bool isEquipped = (dummyDataManager.GetEquippedItemID() == itemID);

        if (isEquipped)
        {
            if (detailPanel != null) detailPanel.SetActive(false);
            if (tooltipPanel != null) tooltipPanel.SetActive(false);
            return;
        }

        if (detailPanel != null) detailPanel.SetActive(true);

        // 📝 미해금 장비 처리 
        if (!selectedItem.isUnlocked)
        {
            if (detailBackgroundImage != null) detailBackgroundImage.color = new Color(0.6f, 0.6f, 0.6f, 1f);

            // 아이콘과 이름 숨기기
            if (detailName != null) detailName.gameObject.SetActive(false);
            if (detailIcon != null) detailIcon.gameObject.SetActive(false);
            if (detailTraitIcon != null) detailTraitIcon.gameObject.SetActive(false);

            // 💡 특성 이름 텍스트 자리를 빌려서 해금 메시지 띄우기 (문구 수정됨)
            if (detailTraitName != null)
            {
                detailTraitName.gameObject.SetActive(true);
                detailTraitName.text = "<align=center><color=#55FF55>던전 해금 시\n착용 가능</color></align>";
            }

            if (equipButton != null) equipButton.gameObject.SetActive(false);
            if (unlockButton != null) unlockButton.gameObject.SetActive(true);
            if (tooltipPanel != null) tooltipPanel.SetActive(false);
        }
        // 📝 해금된 장비 처리
        else
        {
            if (detailBackgroundImage != null) detailBackgroundImage.color = originalDetailBgColor;

            if (detailName != null) { detailName.gameObject.SetActive(true); detailName.text = selectedItem.name; }
            if (detailIcon != null) { detailIcon.gameObject.SetActive(true); detailIcon.sprite = selectedItem.icon; }

            if (detailTraitIcon != null)
            {
                detailTraitIcon.gameObject.SetActive(true);
                detailTraitIcon.sprite = selectedItem.traitIcon; // 💡 여기서 이미지를 바꿉니다
            }
            if (detailTraitName != null)
            {
                detailTraitName.gameObject.SetActive(true);
                detailTraitName.text = selectedItem.traitName;
            }

            if (equipButton != null) equipButton.gameObject.SetActive(true);
            if (unlockButton != null) unlockButton.gameObject.SetActive(false);
        }
    }

    private void OnUnlockClicked(int itemID)
    {
        if (itemID == -1) return;
        dummyDataManager.UnlockItem(itemID);
        RefreshSlots();
        SelectSlot(itemID);
    }

    private void OnItemEquipped(int itemID)
    {
        if (itemID == -1) return;
        dummyDataManager.EquipItemToParty(itemID);
        if (detailPanel != null) detailPanel.SetActive(false);
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        RefreshSlots();
    }
}