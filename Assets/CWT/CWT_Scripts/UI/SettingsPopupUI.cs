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

    private void Awake()
    {
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
            SoundManager.Instance.BGMShot("blacksmith");
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
    }

    private void OnChangedSFX(float vol)
    {
        SoundManager.Instance.ControlSFXVolume(vol);
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

        if (System.Enum.TryParse(input.ToUpper(), out KeyCode result))
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

        if (System.Enum.TryParse(input.ToUpper(), out KeyCode result))
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

    // ─── 언어 설정 ──────────────────────────────

    private void OnKoreanButtonClicked()
    {
        LocalizationManager.Instance.SetLanguage(Language.Korean);
        UpdateLanguageButtons();
    }

    private void OnEnglishButtonClicked()
    {
        LocalizationManager.Instance.SetLanguage(Language.English);
        UpdateLanguageButtons();
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