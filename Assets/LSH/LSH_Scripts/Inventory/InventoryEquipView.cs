using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections.Generic;

public class InventoryEquipView : MonoBehaviour
{
    [Header("Left Panel - Slots")]
    [SerializeField] private Transform m_slotContainer;
    [SerializeField] private InventorySlot m_slotPrefab;
    [SerializeField] private Sprite m_emptySlotSprite;

    [Header("Right Panel - Detail")]
    [SerializeField] private GameObject m_detailPanel;
    [SerializeField] private Image m_detailBackgroundImage;

    // 💡 [NEW] UI 스위칭용 그룹 변수
    [Header("UI Groups")]
    [SerializeField] private GameObject m_unlockedUIGroup; // 해금 시 켜지는 그룹 (아이콘, 이름, 특성 등 몽땅)
    [SerializeField] private GameObject m_lockedUIGroup;   // 잠금 시 켜지는 그룹 (정중앙 안내 문구)

    [Header("Unlocked UI Elements")]
    [SerializeField] private Image m_detailIcon;
    [SerializeField] private TextMeshProUGUI m_detailName;
    [SerializeField] private Image m_detailTraitIcon;
    [SerializeField] private TextMeshProUGUI m_detailTraitName;

    [Header("Buttons")]
    [SerializeField] private Button m_equipButton;
    [SerializeField] private Button m_unlockButton;
    [SerializeField] private Button m_detailBlocker;

    [Header("Tooltip UI")]
    [SerializeField] private GameObject m_tooltipPanel;
    [SerializeField] private TextMeshProUGUI m_tooltipName;
    [SerializeField] private TextMeshProUGUI m_tooltipDesc;

    [Header("Common")]
    [SerializeField] private Button m_mainCloseButton;
    [SerializeField] private Button m_detailCloseButton;

    public event Action<int> OnSlotClicked;
    public event Action OnEquipClicked;
    public event Action OnCloseClicked;
    public event Action<int> OnUnlockClicked;

    private int m_currentViewedItemID = -1;
    private Color m_originalDetailColor;
    private ItemData m_currentDetailItem;

    private void Awake()
    {
        if (m_equipButton != null) m_equipButton.onClick.AddListener(() => OnEquipClicked?.Invoke());
        if (m_mainCloseButton != null) m_mainCloseButton.onClick.AddListener(() => OnCloseClicked?.Invoke());

        if (m_unlockButton != null)
        {
            m_unlockButton.onClick.AddListener(() => {
                if (m_currentViewedItemID != -1) OnUnlockClicked?.Invoke(m_currentViewedItemID);
            });
        }

        if (m_detailCloseButton != null) m_detailCloseButton.onClick.AddListener(HideDetail);
        if (m_detailBlocker != null) m_detailBlocker.onClick.AddListener(HideDetail);

        if (m_detailBackgroundImage != null) m_originalDetailColor = m_detailBackgroundImage.color;

        if (m_detailIcon != null) SetupTooltipTrigger(m_detailIcon.gameObject, true);
        if (m_detailTraitIcon != null) SetupTooltipTrigger(m_detailTraitIcon.gameObject, false);

        if (m_tooltipPanel != null) m_tooltipPanel.SetActive(false);
        HideDetail();
    }

    private void SetupTooltipTrigger(GameObject targetObj, bool isEquipment)
    {
        if (targetObj.GetComponent<EventTrigger>() == null) targetObj.AddComponent<EventTrigger>();
        EventTrigger trigger = targetObj.GetComponent<EventTrigger>();
        trigger.triggers.Clear();

        EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener((data) => {
            if (m_tooltipPanel == null || m_currentDetailItem == null) return;
            if (!m_currentDetailItem.isUnlocked) return;

            if (isEquipment)
            {
                if (m_tooltipName != null) m_tooltipName.text = m_currentDetailItem.name;
                if (m_tooltipDesc != null) m_tooltipDesc.text = m_currentDetailItem.description;
            }
            else
            {
                if (m_tooltipName != null) m_tooltipName.text = m_currentDetailItem.traitName;
                if (m_tooltipDesc != null) m_tooltipDesc.text = m_currentDetailItem.traitDescription;
            }
            m_tooltipPanel.SetActive(true);
        });
        trigger.triggers.Add(enter);

        EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener((data) => { if (m_tooltipPanel != null) m_tooltipPanel.SetActive(false); });
        trigger.triggers.Add(exit);
    }

    public void DrawSlots(List<ItemData> items)
    {
        foreach (Transform child in m_slotContainer) Destroy(child.gameObject);

        for (int i = 0; i < 8; i++)
        {
            InventorySlot slot = Instantiate(m_slotPrefab, m_slotContainer);
            int slotIndex = i;

            if (i < items.Count)
                slot.SetSlot(items[i].id, items[i].icon, items[i].isUnlocked, (id) => OnSlotClicked?.Invoke(slotIndex));
            else
                slot.SetEmpty(m_emptySlotSprite);
        }
    }

    public void ShowDetail(ItemData item, bool isEquipped)
    {
        m_currentViewedItemID = item.id;
        m_currentDetailItem = item;

        if (isEquipped) { HideDetail(); return; }

        bool canUnlock = item.isCrafted && !item.isUnlocked;

        if (!item.isCrafted)
        {
            HideDetail();
            return;
        }

        if (m_detailPanel != null) m_detailPanel.SetActive(true);
        if (m_detailBlocker != null) m_detailBlocker.gameObject.SetActive(true);

        if (canUnlock)
        {
            if (m_detailBackgroundImage != null) m_detailBackgroundImage.color = new Color(0.6f, 0.6f, 0.6f, 1f);

            if (m_unlockedUIGroup != null) m_unlockedUIGroup.SetActive(false);
            if (m_lockedUIGroup != null) m_lockedUIGroup.SetActive(true);

            if (m_equipButton != null) m_equipButton.gameObject.SetActive(false);
            if (m_unlockButton != null) m_unlockButton.gameObject.SetActive(true);
            if (m_detailTraitName != null) m_detailTraitName.text = "해금 후 착용 가능";
            if (m_tooltipPanel != null) m_tooltipPanel.SetActive(false);
        }
        else
        {
            if (m_detailBackgroundImage != null) m_detailBackgroundImage.color = m_originalDetailColor;

            if (m_unlockedUIGroup != null) m_unlockedUIGroup.SetActive(true);
            if (m_lockedUIGroup != null) m_lockedUIGroup.SetActive(false);

            if (m_detailName != null) m_detailName.text = item.name;
            if (m_detailIcon != null) m_detailIcon.sprite = item.icon;
            if (m_detailTraitIcon != null) m_detailTraitIcon.sprite = item.traitIcon;
            if (m_detailTraitName != null) m_detailTraitName.text = item.traitName;

            if (m_equipButton != null) m_equipButton.gameObject.SetActive(true);
            if (m_unlockButton != null) m_unlockButton.gameObject.SetActive(false);
        }
    }

    public void HideDetail()
    {
        m_currentViewedItemID = -1;
        m_currentDetailItem = null;
        if (m_unlockedUIGroup != null) m_unlockedUIGroup.SetActive(false);
        if (m_lockedUIGroup != null) m_lockedUIGroup.SetActive(false);
        if (m_equipButton != null) m_equipButton.gameObject.SetActive(false);
        if (m_unlockButton != null) m_unlockButton.gameObject.SetActive(false);
        if (m_detailBlocker != null) m_detailBlocker.gameObject.SetActive(false);
        if (m_detailPanel != null) m_detailPanel.SetActive(false);
        if (m_tooltipPanel != null) m_tooltipPanel.SetActive(false);
    }
}