// ============================================
// 파일명: SettingsPopupUI.cs
// 붙일 오브젝트: SettingsPopup
// 역할: 크기 보정 + 닫기 + 볼륨 + 단축키 + 언어 설정
// ============================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SettingsPopupUI : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    private RectTransform _rectTransform;

    // ─── 볼륨 관련 ─────────────────────────────
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    // ─── 단축키 관련 ────────────────────────────
    [SerializeField] private TMP_InputField _minimizeInput;
    [SerializeField] private TMP_InputField _clickThroughInput;

    // ─── 언어 관련 ──────────────────────────────
    // Inspector에서 한국 국기 버튼, 미국 국기 버튼을 드래그해서 연결
    [SerializeField] private Button _koreanButton;
    [SerializeField] private Button _englishButton;

    // ─── 뮤트 토글 관련 ─────────────────────────
    // Inspector에서 BGMIcon, SFXIcon 오브젝트를 드래그해서 연결
    [SerializeField] private Button _bgmMuteButton;
    [SerializeField] private Button _sfxMuteButton;

    // 뮤트/언뮤트 아이콘 스프라이트 (Inspector에서 드래그)
    [SerializeField] private Sprite _muteSprite;    // 음소거 아이콘
    [SerializeField] private Sprite _unmuteSprite;  // 소리 나는 아이콘

    // 뮤트 전 마지막 볼륨값을 저장해두는 변수
    // 뮤트 해제할 때 이전 볼륨으로 복원하기 위해 필요
    private float _lastBgmVolume = 1f;
    private float _lastSfxVolume = 1f;

    // 현재 뮤트 상태인지 추적하는 변수
    private bool _isBgmMuted = false;
    private bool _isSfxMuted = false;

    private void Awake()
    {
        // 뮤트 버튼 연결
        _bgmMuteButton.onClick.AddListener(ToggleBGMMute);
        _sfxMuteButton.onClick.AddListener(ToggleSFXMute);

        _rectTransform = GetComponent<RectTransform>();

        // 볼륨 슬라이더 연결
        bgmSlider.onValueChanged.AddListener(OnChangedBGM);
        sfxSlider.onValueChanged.AddListener(OnChangedSFX);

        // 닫기 버튼 연결
        _closeButton.onClick.AddListener(ClosePopup);

        // 단축키 InputField 연결
        _minimizeInput.onEndEdit.AddListener(OnMinimizeKeyChanged);
        _clickThroughInput.onEndEdit.AddListener(OnClickThroughKeyChanged);

        // 언어 버튼 연결
        _koreanButton.onClick.AddListener(OnKoreanButtonClicked);
        _englishButton.onClick.AddListener(OnEnglishButtonClicked);
    }

    private void OnEnable()
    {
        // 볼륨 슬라이더 동기화
        if (SoundManager.Instance != null)
        {
            bgmSlider.value = SoundManager.Instance.BGMVolume;
            sfxSlider.value = SoundManager.Instance.SFXVolume;
        }

        // 단축키 InputField 동기화
        if (HotkeyManager.Instance != null)
        {
            _minimizeInput.text = HotkeyManager.Instance.MinimizeKey == KeyCode.None
                ? ""
                : HotkeyManager.Instance.MinimizeKey.ToString();

            _clickThroughInput.text = HotkeyManager.Instance.ClickThroughKey == KeyCode.None
                ? ""
                : HotkeyManager.Instance.ClickThroughKey.ToString();
        }

        // 언어 버튼 상태 동기화
        UpdateLanguageButtons();

        StartCoroutine(FixSizeAndPosition());
    }

    private IEnumerator FixSizeAndPosition()
    {
        yield return null;
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 650f);
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1072f);
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    // ─── 볼륨 조절 ─────────────────────────────

    private void OnChangedBGM(float vol)
    {
        SoundManager.Instance.ControlBGMVolume(vol);

        // 슬라이더가 0이면 뮤트 아이콘, 아니면 언뮤트 아이콘
        _isBgmMuted = vol <= 0f;
        _bgmMuteButton.GetComponent<Image>().sprite = _isBgmMuted ? _muteSprite : _unmuteSprite;
    }

    private void OnChangedSFX(float vol)
    {
        SoundManager.Instance.ControlSFXVolume(vol);

        _isSfxMuted = vol <= 0f;
        _sfxMuteButton.GetComponent<Image>().sprite = _isSfxMuted ? _muteSprite : _unmuteSprite;
    }

    // ─── 뮤트 토글 ─────────────────────────────

    // BGM 뮤트 버튼 클릭 시 호출
    private void ToggleBGMMute()
    {
        _isBgmMuted = !_isBgmMuted;

        if (_isBgmMuted)
        {
            // 뮤트 ON: 현재 볼륨 저장 → 볼륨 0으로 → 슬라이더도 0으로
            _lastBgmVolume = bgmSlider.value;
            SoundManager.Instance.ControlBGMVolume(0f);
            bgmSlider.value = 0f;
        }
        else
        {
            // 뮤트 OFF: 저장해둔 볼륨으로 복원
            SoundManager.Instance.ControlBGMVolume(_lastBgmVolume);
            bgmSlider.value = _lastBgmVolume;
        }

        // 아이콘 이미지 변경
        _bgmMuteButton.GetComponent<Image>().sprite = _isBgmMuted ? _muteSprite : _unmuteSprite;
    }

    // SFX 뮤트 버튼 클릭 시 호출
    private void ToggleSFXMute()
    {
        _isSfxMuted = !_isSfxMuted;

        if (_isSfxMuted)
        {
            _lastSfxVolume = sfxSlider.value;
            SoundManager.Instance.ControlSFXVolume(0f);
            sfxSlider.value = 0f;
        }
        else
        {
            SoundManager.Instance.ControlSFXVolume(_lastSfxVolume);
            sfxSlider.value = _lastSfxVolume;
        }

        _sfxMuteButton.GetComponent<Image>().sprite = _isSfxMuted ? _muteSprite : _unmuteSprite;
    }

    // ─── 단축키 설정 ────────────────────────────

    private void OnMinimizeKeyChanged(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            HotkeyManager.Instance.SetHotKey(HotkeyType.Minimize, KeyCode.None);
            _minimizeInput.text = "";
            return;
        }

        if (TryParseHotkeyInput(input, out KeyCode result))
        {
            if (result == KeyCode.Escape)
            {
                HotkeyManager.Instance.SetHotKey(HotkeyType.Minimize, KeyCode.None);
                _minimizeInput.text = "";
                return;
            }

            HotkeyManager.Instance.SetHotKey(HotkeyType.Minimize, result);
            _minimizeInput.text = result.ToString();

            _clickThroughInput.text = HotkeyManager.Instance.ClickThroughKey == KeyCode.None
                ? ""
                : HotkeyManager.Instance.ClickThroughKey.ToString();
        }
        else
        {
            _minimizeInput.text = HotkeyManager.Instance.MinimizeKey == KeyCode.None
                ? ""
                : HotkeyManager.Instance.MinimizeKey.ToString();
        }
    }

    private void OnClickThroughKeyChanged(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            HotkeyManager.Instance.SetHotKey(HotkeyType.ClickThrough, KeyCode.None);
            _clickThroughInput.text = "";
            return;
        }

        if (TryParseHotkeyInput(input, out KeyCode result))
        {
            if (result == KeyCode.Escape)
            {
                HotkeyManager.Instance.SetHotKey(HotkeyType.ClickThrough, KeyCode.None);
                _clickThroughInput.text = "";
                return;
            }

            HotkeyManager.Instance.SetHotKey(HotkeyType.ClickThrough, result);
            _clickThroughInput.text = result.ToString();

            _minimizeInput.text = HotkeyManager.Instance.MinimizeKey == KeyCode.None
                ? ""
                : HotkeyManager.Instance.MinimizeKey.ToString();
        }
        else
        {
            _clickThroughInput.text = HotkeyManager.Instance.ClickThroughKey == KeyCode.None
                ? ""
                : HotkeyManager.Instance.ClickThroughKey.ToString();
        }
    }

    private bool TryParseHotkeyInput(string input, out KeyCode result)
    {
        string trimmedInput = input.Trim();

        if (trimmedInput.Length == 1 && char.IsDigit(trimmedInput[0]))
        {
            return System.Enum.TryParse($"Alpha{trimmedInput}", true, out result);
        }

        return System.Enum.TryParse(trimmedInput, true, out result);
    }

    // ─── 언어 설정 ──────────────────────────────

    private void OnKoreanButtonClicked()
    {
        LocalizationManager.Instance.SetLanguage(Language.Korean);
        ForceRefreshLocalizedUI();
        UpdateLanguageButtons();
    }

    private void OnEnglishButtonClicked()
    {
        LocalizationManager.Instance.SetLanguage(Language.English);
        ForceRefreshLocalizedUI();
        UpdateLanguageButtons();
    }

    private void ForceRefreshLocalizedUI()
    {
        // LocalizedText 붙은 텍스트들 즉시 갱신
        LocalizedText[] localizedTexts = FindObjectsByType<LocalizedText>(FindObjectsSortMode.None);
        foreach (var text in localizedTexts)
        {
            text.SendMessage("UpdateText", SendMessageOptions.DontRequireReceiver);
        }

        // 장비 슬롯 즉시 갱신
        GearsetSlotUI[] gearSlots = FindObjectsByType<GearsetSlotUI>(FindObjectsSortMode.None);
        foreach (var slot in gearSlots)
        {
            slot.RefreshSlotInfo();
            slot.RefreshState();
        }
    }

    // 현재 언어에 따라 버튼 활성화/비활성화 갱신
    // 기획서: 현재 선택된 언어의 버튼은 비활성화, 다른 언어 버튼은 활성화
    private void UpdateLanguageButtons()
    {
        if (LocalizationManager.Instance == null) return;

        bool isKorean = LocalizationManager.Instance.CurrentLanguage == Language.Korean;

        // interactable = false면 버튼이 회색으로 바뀌고 클릭 불가
        _koreanButton.interactable = !isKorean;
        _englishButton.interactable = isKorean;
    }

    // ─── 팝업 열기/닫기 ─────────────────────────

    public void OpenSettings()
    {
        if (gameObject.activeSelf) return;
        PopupManager.Instance.OpenSettings();
    }

    private void ClosePopup()
    {
        PopupManager.Instance.ClosePopup(_rectTransform);
    }
}