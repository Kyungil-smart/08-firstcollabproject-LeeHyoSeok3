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
    // ─── Button 참조 ───
    private Button _button;
    private PartyEquipManager _partyEquipManager;

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
        
        // QuestPanel 프리팹에 Button 컴포넌트가 이미 붙어있으니까
        // 그걸 가져와서 클릭 시 OnQuestClicked 호출되도록 연결
        _button = GetComponent<Button>();
        if (_button != null)
        {
            _button.onClick.AddListener(OnQuestClicked);
            // ★ 추가: 요구 특성 충족 여부에 따라 버튼 활성화/비활성화
            UpdateQuestAvailability();
        }

        _partyEquipManager = PartyEquipManager.Instance;
        if (_partyEquipManager != null)
        {
            _partyEquipManager.OnAttributeChanged -= HandlePartyAttributeChanged;
            _partyEquipManager.OnAttributeChanged += HandlePartyAttributeChanged;
        }

    }

    // ─── 추가 3: 퀘스트 패널 클릭 시 호출되는 메서드 ───
    // ─── 퀘스트 패널 클릭 시 호출되는 메서드 ───
    private void OnQuestClicked()
    {
        // 이미 퀘스트 진행 중이면 출발 불가
        if (QuestManager.Instance != null && QuestManager.Instance.IsQuestActive)
        {
            Debug.Log("이미 퀘스트가 진행 중입니다!");
            return;
        }

        // ★ 추가: 요구 특성을 충족하지 못하면 출발 불가
        if (PartyEquipManager.Instance != null &&
            !PartyEquipManager.Instance.CanEnterDungeon(_data.requiredTraitKey))
        {
            Debug.Log($"요구 특성 불충족! 필요: {_data.requiredAttribute}, " +
                      $"현재: {PartyEquipManager.Instance.CurrentAttribute}");
            return;
        }

        // QuestManager에게 출발 요청
        QuestManager.Instance?.StartQuest(_data);
    }

    // 현재 언어에 맞게 텍스트를 갱신하는 메서드
    private void RefreshText()
    {
        if (_data == null) return;

        _dungeonName.text = LocalizationManager.Instance.GetText(_data.dungeonName);
        _description.text = LocalizationManager.Instance.GetText(_data.description);
        _requiredAttribute.text = LocalizationManager.Instance.GetText(_data.requiredAttribute);
        _rewardItem.text = LocalizationManager.Instance.GetText(_data.rewardItem);
        string minuteUnit = LocalizationManager.Instance.GetText("분");

        _timeRequired.text = $"{_data.timeRequired}{minuteUnit}";
    }

    /// <summary>
    /// 요구 특성 충족 여부에 따라 버튼 활성화/비활성화 + 텍스트 색상 변경
    /// 기획서 기준:
    ///   충족 → 버튼 활성화 + 기본 색상
    ///   불충족 → 버튼 비활성화 + 빨간색 (#FF0000)
    /// </summary>
    private void UpdateQuestAvailability()
    {
        if (_data == null) return;

        // PartyEquipManager가 없으면 기본적으로 활성화
        if (_partyEquipManager == null)
            return;

        bool canEnter = _partyEquipManager.CanEnterDungeon(_data.requiredTraitKey);

        // 버튼 활성화/비활성화
        if (_button != null)
        {
            _button.interactable = canEnter;
        }

        // 요구 특성 텍스트 색상 변경
        if (_requiredAttribute != null)
        {
            if (canEnter)
            {
                // 충족 → 기본 색상 (파란색)
                _requiredAttribute.color = Color.blue;
            }
            else
            {
                // 불충족 → 빨간색 (#FF0000)
                _requiredAttribute.color = new Color(1f, 0f, 0f, 1f);
            }
        }
    }

    private void HandlePartyAttributeChanged(string currentAttribute)
    {
        UpdateQuestAvailability();
    }

    // 오브젝트가 파괴될 때 이벤트 등록 해제 (메모리 누수 방지)
    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= RefreshText;
        }

        if (_partyEquipManager != null)
        {
            _partyEquipManager.OnAttributeChanged -= HandlePartyAttributeChanged;
        }

        // ★ 추가: 버튼 클릭 리스너도 해제 (등록과 짝 맞추기)
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnQuestClicked);
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
