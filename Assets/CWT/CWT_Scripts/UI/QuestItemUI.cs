using TMPro;
using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트를 쓰려면 필요

public class QuestItemUI : MonoBehaviour
{
    // ─── 텍스트 ───
    [SerializeField] private TMP_Text _dungeonName;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _requiredAttribute;
    [SerializeField] private TMP_Text _rewardItem;
    [SerializeField] private TMP_Text _timeRequired;

    // ─── 아이콘 이미지 ───
    // Inspector에서 각각의 Image 오브젝트를 드래그해서 연결
    [SerializeField] private Image _dungeonIcon;      // 던전 아이콘 (LocationImage)
    [SerializeField] private Image _attributeIcon;    // 특성 아이콘 (Attribute 안의 Image)
    [SerializeField] private Image _materialIcon;     // 재료 아이콘 (Ingredient 안의 Image)

    // 원본 데이터를 저장 (언어 변경 시 다시 텍스트를 갱신하기 위해)
    private DungeonData _data;

    public void SetData(DungeonData data)
    {
        _data = data;

        // 아이콘 설정
        SetIcon(_dungeonIcon, data.dungeonIcon);
        SetIcon(_attributeIcon, data.attributeIcon);
        SetIcon(_materialIcon, data.materialIcon);

        // 텍스트 설정 (현재 언어에 맞게)
        RefreshText();

        // 언어 변경 이벤트에 등록
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += RefreshText;
        }
    }

    // 현재 언어에 맞게 텍스트를 갱신하는 메서드
    private void RefreshText()
    {
        if (_data == null) return;

        _dungeonName.text = LocalizationManager.Instance.GetText(_data.dungeonName);
        _description.text = LocalizationManager.Instance.GetText(_data.description);
        _requiredAttribute.text = LocalizationManager.Instance.GetText(_data.requiredAttribute);
        _rewardItem.text = LocalizationManager.Instance.GetText(_data.rewardItem);
        _timeRequired.text = _data.timeRequired; // 시간은 번역 필요 없음
    }

    // 오브젝트가 파괴될 때 이벤트 등록 해제 (메모리 누수 방지)
    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= RefreshText;
        }
    }

    // 스프라이트가 있으면 이미지에 넣고, 없으면 이미지를 숨기는 메서드
    private void SetIcon(Image iconImage, Sprite sprite)
    {
        if (iconImage == null) return;

        if (sprite != null)
        {
            iconImage.sprite = sprite;
            iconImage.enabled = true;
        }
        else
        {
            // 스프라이트가 없으면 이미지를 숨김 (평원은 요구특성이 없으니까)
            iconImage.enabled = false;
        }
    }
}