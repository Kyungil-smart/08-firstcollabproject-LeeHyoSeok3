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
    [SerializeField] private Image m_detailIcon;             // 장비 아이콘
    [SerializeField] private TextMeshProUGUI m_detailName;

    [SerializeField] private Image m_detailTraitIcon;        // 특성 아이콘
    [SerializeField] private TextMeshProUGUI m_detailTraitName;  // 특성 이름 텍스트 (미해금 시 안내 문구로 활용)

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
    private ItemData m_currentDetailItem; // 툴팁용 데이터 임시 저장

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

        // 💡 [듀얼 툴팁 연결] 장비(true)와 특성(false)에 각각 이벤트를 붙여줍니다.
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
            if (!m_currentDetailItem.isUnlocked) return; // 미해금 장비는 툴팁 안 띄움

            // 💡 [듀얼 툴팁 로직] 마우스가 올라간 대상에 맞춰 내용을 다르게 표시!
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

        m_detailPanel.SetActive(true);
        if (m_detailBlocker != null) m_detailBlocker.gameObject.SetActive(true);

        // 📝 미해금 장비 처리
        if (!item.isUnlocked)
        {
            if (m_detailBackgroundImage != null) m_detailBackgroundImage.color = new Color(0.6f, 0.6f, 0.6f, 1f);

            // 아이콘 및 이름 숨기기
            if (m_detailName != null) m_detailName.gameObject.SetActive(false);
            if (m_detailIcon != null) m_detailIcon.gameObject.SetActive(false);
            if (m_detailTraitIcon != null) m_detailTraitIcon.gameObject.SetActive(false);

            // 💡 [미해금 텍스트 수정] 특성 이름 텍스트 자리를 활용하여 해금 메시지 노출
            if (m_detailTraitName != null)
            {
                m_detailTraitName.gameObject.SetActive(true);
                m_detailTraitName.text = "<align=center><color=#55FF55>던전 해금 시\n착용 가능</color></align>";
            }

            if (m_equipButton != null) m_equipButton.gameObject.SetActive(false);
            if (m_unlockButton != null) m_unlockButton.gameObject.SetActive(true);
            if (m_tooltipPanel != null) m_tooltipPanel.SetActive(false);
        }
        // 📝 해금된 장비 처리
        else
        {
            if (m_detailBackgroundImage != null) m_detailBackgroundImage.color = m_originalDetailColor;

            if (m_detailName != null) { m_detailName.gameObject.SetActive(true); m_detailName.text = item.name; }
            if (m_detailIcon != null) { m_detailIcon.gameObject.SetActive(true); m_detailIcon.sprite = item.icon; }

            if (m_detailTraitIcon != null)
            {
                m_detailTraitIcon.gameObject.SetActive(true);
                m_detailTraitIcon.sprite = item.traitIcon;
            }
            if (m_detailTraitName != null)
            {
                m_detailTraitName.gameObject.SetActive(true);
                m_detailTraitName.text = item.traitName;
            }

            if (m_equipButton != null) m_equipButton.gameObject.SetActive(true);
            if (m_unlockButton != null) m_unlockButton.gameObject.SetActive(false);
        }
    }

    public void HideDetail()
    {
        m_currentViewedItemID = -1;
        m_currentDetailItem = null;
        if (m_detailBlocker != null) m_detailBlocker.gameObject.SetActive(false);
        if (m_detailPanel != null) m_detailPanel.SetActive(false);
        if (m_tooltipPanel != null) m_tooltipPanel.SetActive(false);
    }
}