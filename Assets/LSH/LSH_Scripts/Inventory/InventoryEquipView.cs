using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Image m_detailIcon;
    [SerializeField] private TextMeshProUGUI m_detailName;
    [SerializeField] private TextMeshProUGUI m_detailDesc;
    [SerializeField] private Button m_equipButton;
    [SerializeField] private Button m_detailCloseButton;
    [SerializeField] private Button m_detailBlocker; // 💡 새로 추가: 바깥 영역(배경) 감지용 버튼

    [Header("Common")]
    [SerializeField] private Button m_mainCloseButton;

    public event Action<int> OnSlotClicked;
    public event Action OnEquipClicked;
    public event Action OnCloseClicked;

    private void Awake()
    {
        m_equipButton.onClick.AddListener(() => OnEquipClicked?.Invoke());
        m_mainCloseButton.onClick.AddListener(() => OnCloseClicked?.Invoke());

        // 상세창 X 버튼과 뒷배경(블로커) 버튼 모두 HideDetail 함수를 실행하도록 연결
        m_detailCloseButton.onClick.AddListener(HideDetail);

        if (m_detailBlocker != null)
            m_detailBlocker.onClick.AddListener(HideDetail);
    }

    public void DrawSlots(List<ItemData> items)
    {
        foreach (Transform child in m_slotContainer) Destroy(child.gameObject);

        for (int i = 0; i < 8; i++)
        {
            InventorySlot slot = Instantiate(m_slotPrefab, m_slotContainer);
            int slotIndex = i;

            if (i < items.Count)
                slot.SetSlot(items[i].id, items[i].icon, (id) => OnSlotClicked?.Invoke(slotIndex));
            else
                slot.SetEmpty(m_emptySlotSprite);
        }
    }

    public void ShowDetail(ItemData item)
    {
        // 1. 블로커(배경)와 팝업 모두 켜기
        if (m_detailBlocker != null) m_detailBlocker.gameObject.SetActive(true);
        m_detailPanel.SetActive(true);

        m_detailName.text = item.name;
        m_detailIcon.sprite = item.icon;
        m_detailDesc.text = item.description;

        // 2. 화면 맨 앞으로 가져오기 (순서: 블로커가 먼저 깔리고, 그 위에 팝업이 올라감)
        if (m_detailBlocker != null) m_detailBlocker.transform.SetAsLastSibling();
        m_detailPanel.transform.SetAsLastSibling();

        // 3. 💡 마우스 로직 제거 및 화면 정중앙 강제 배치
        RectTransform detailRect = m_detailPanel.GetComponent<RectTransform>();
        detailRect.anchorMin = new Vector2(0.5f, 0.5f);
        detailRect.anchorMax = new Vector2(0.5f, 0.5f);
        detailRect.pivot = new Vector2(0.5f, 0.5f);
        detailRect.anchoredPosition = Vector2.zero; // 오차 없이 완벽한 정중앙 좌표 (0,0)
    }

    public void HideDetail()
    {
        // 팝업이 꺼질 때 뒷배경도 같이 꺼주기
        if (m_detailBlocker != null) m_detailBlocker.gameObject.SetActive(false);
        m_detailPanel.SetActive(false);
    }
}