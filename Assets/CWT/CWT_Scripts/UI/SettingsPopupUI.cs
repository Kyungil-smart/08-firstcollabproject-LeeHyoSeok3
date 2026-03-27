// ============================================
// 파일명: SettingsPopupUI.cs
// 붙일 오브젝트: SettingsPopup
// 역할: PopupManager가 크기를 덮어씌우는 걸 보정 + 닫기 버튼 연결
// ============================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsPopupUI : MonoBehaviour
{
    // Inspector에서 CloseButton 오브젝트를 드래그해서 연결할 변수
    [SerializeField] private Button _closeButton;

    // 크기 조절용 RectTransform (자기 자신의 것)
    private RectTransform _rectTransform;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;


    private void Awake()
    {
        // 자기 자신에 붙어있는 RectTransform 컴포넌트를 가져와서 저장해둠
        _rectTransform = GetComponent<RectTransform>();

        // bgm과sfx 볼륨 조절
        bgmSlider.onValueChanged.AddListener(OnChangedBGM);
        sfxSlider.onValueChanged.AddListener(OnChangedSFX);

        // 닫기 버튼을 클릭하면 ClosePopup 메서드가 실행되도록 연결
        _closeButton.onClick.AddListener(ClosePopup);
    }

    private void OnEnable()
    {
        // 팝업 열릴 때 슬라이더를 현재 볼륨값에 맞춰줌
        bgmSlider.value = SoundManager.Instance.BGMVolume;
        sfxSlider.value = SoundManager.Instance.SFXVolume;

        SoundManager.Instance.BGMShot("blacksmith");

        // 바로 보정하면 PopupManager의 CenterOnMonitor가 뒤에서 덮어씌움
        StartCoroutine(FixSizeAndPosition());
    }

    // 외부에서 설정 팝업을 열 때 이 메서드를 호출
    // 이미 열려있으면 중복 실행을 막아줌
    public void OpenSettings()
    {
        // 이미 활성화 상태면 아무것도 안 함
        if (gameObject.activeSelf) return;

        // PopupManager를 통해 정상적으로 열기
        PopupManager.Instance.OpenSettings();
    }

    // 코루틴: 1프레임 기다린 뒤 크기와 위치를 강제 보정하는 메서드
    // IEnumerator = 코루틴 메서드의 리턴 타입 (yield를 쓰려면)
    private IEnumerator FixSizeAndPosition()
    {
        // yield return null = "1프레임 기다려"라는 뜻
        // 이 한 줄 덕분에 PopupManager의 작업이 다 끝난 뒤에 아래 코드가 실행되게 한다.
        yield return null;

        // PopupManager가 바꿔버린 크기를 원래 원하는 크기로 다시 세팅
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 650f);
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1072f);

        // 위치를 앵커 기준 정중앙으로 보정
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    private void OnChangedBGM(float vol)
    {
        SoundManager.Instance.ControlBGMVolume(vol);
    }

    private void OnChangedSFX(float vol)
    {
        SoundManager.Instance.ControlSFXVolume(vol);
    }

    // 닫기 버튼 눌렀을 때 호출되는 메서드
    private void ClosePopup()
    {
        PopupManager.Instance.ClosePopup(_rectTransform);
    }
}