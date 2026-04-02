using UnityEngine;
using UnityEngine.UI;
using System;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image m_iconImage;
    [SerializeField] private Button m_button;

    // 💡 비활성화(잠금) 상태일 때 덮어씌울 어두운 회색
    private Color m_lockedColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    private Color m_unlockedColor = Color.white;

    // 💡 매개변수에 bool isUnlocked가 추가되었습니다!
    public void SetSlot(int id, Sprite icon, bool isUnlocked, Action<int> onClicked)
    {
        m_iconImage.sprite = icon;

        // 해금 상태에 따라 아이콘의 색상을 원래 색(하얀색 필터) 또는 어두운 회색으로 변경합니다.
        m_iconImage.color = isUnlocked ? m_unlockedColor : m_lockedColor;

        m_button.onClick.RemoveAllListeners();
        m_button.onClick.AddListener(() => onClicked?.Invoke(id));
    }

    public void SetEmpty(Sprite emptySprite)
    {
        m_iconImage.sprite = emptySprite;
        m_iconImage.color = Color.white; // 빈 슬롯은 기본 색상 유지
        m_button.onClick.RemoveAllListeners();
    }
}