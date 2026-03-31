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
        if (m_detailBlocker != null) m_detailBlocker.gameObject.SetActive(true);
        m_detailPanel.SetActive(true);

        if (m_detailBlocker != null) m_detailBlocker.transform.SetAsLastSibling();
        m_detailPanel.transform.SetAsLastSibling();

        RectTransform detailRect = m_detailPanel.GetComponent<RectTransform>();
        detailRect.anchorMin = new Vector2(0.5f, 0.5f);
        detailRect.anchorMax = new Vector2(0.5f, 0.5f);
        detailRect.pivot = new Vector2(0.5f, 0.5f);
        detailRect.anchoredPosition = Vector2.zero;

        // 💡 해금 여부에 따른 UI 분기 처리 (기획서 6페이지 반영)
        if (item.isUnlocked)
        {
            // 1. 해금된 장비: 모든 UI 요소를 켜고 정상적인 데이터를 보여줍니다.
            m_detailName.gameObject.SetActive(true);
            m_detailIcon.gameObject.SetActive(true);
            m_equipButton.gameObject.SetActive(true);

            m_detailName.text = item.name;
            m_detailIcon.sprite = item.icon;
            m_detailDesc.text = item.description;
        }
        else
        {
            // 2. 미해금 장비 (잠금 상태): 이름, 아이콘, 장착 버튼을 숨깁니다.
            m_detailName.gameObject.SetActive(false);
            m_detailIcon.gameObject.SetActive(false);
            m_equipButton.gameObject.SetActive(false);

            // TextMeshPro의 Rich Text 태그(<align>)를 사용하여 텍스트를 팝업 정중앙에 배치합니다.
            m_detailDesc.text = "<align=center>퀘스트 해금시\n착용가능</align>";
        }
    }

    public void HideDetail()
    {
        // 팝업이 꺼질 때 뒷배경도 같이 꺼주기
        if (m_detailBlocker != null) m_detailBlocker.gameObject.SetActive(false);
        m_detailPanel.SetActive(false);
    }
}