using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 클릭 통과 모드 토글 버튼 컨트롤러.
///
/// 동작 규칙:
/// - 초기 상태: 클릭 통과 ON (배경 투명, 바탕화면 작업 가능)
/// - 토글 버튼 클릭 → 클릭 통과 OFF (배경 불투명, 게임 조작 가능)
/// - 토글 버튼 다시 클릭 → 클릭 통과 ON (배경 투명)
/// - 마우스가 위젯 영역 밖으로 나가면 → 클릭 통과 ON (배경 투명)
/// - 2회 이상 연속 클릭해도 1회로 판정
/// </summary>

public class ClickThroughToggle : MonoBehaviour, IPointerClickHandler
{
    [Header("위젯 영역 RectTransform (마우스 이탈 감지용)")]
    [SerializeField] private RectTransform widgetArea;

    [Header("토글 버튼 시각 피드백 (선택)")]
    [SerializeField] private Image toggleButtonImage;
    [SerializeField] private Color clickThroughActiveColor = Color.green;
    [SerializeField] private Color clickThroughInactiveColor = Color.gray;

    // 추가
    [Header("배경 투명화 조정")]
    [SerializeField] private Image backgroundImage;    // BackGroundImage
    [SerializeField] private CanvasGroup CanvasGroup; // 투명화 할 CanvasGroup (최대화 화면 전체)

    [Header("클릭 통과 시 Raycast Target 끌 대상들")]
    [SerializeField] private List<Graphic> raycastTargets;
    //



    private bool _pendingReactivate = false; // 클릭 통과 재활성화 대기 상태
    private bool _buttonClickedThisFrame = false;
    private Camera _uiCamera;

    private void Start()
    {
        _uiCamera = Camera.main;
        UpdateButtonVisual();
        UpdateBackgroundVisual();
    }

    private void Update()
    {
        // // 클릭 통과 재활성화 대기 중 → 마우스가 위젯 영역 밖으로 나갔는지 확인
        // if (_pendingReactivate && !IsMouseOverWidgetArea())
        // {
        //     WindowSystemManager.Instance.SetClickThrough(true);
        //     _pendingReactivate = false;
        //     UpdateButtonVisual();
        //     UpdateBackgroundVisual();
        // }

        _buttonClickedThisFrame = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_buttonClickedThisFrame) return; // 같은 프레임 내 중복 클릭 무시
        _buttonClickedThisFrame = true;

        bool currentlyClickThrough = WindowSystemManager.Instance.IsClickThrough;

        if (currentlyClickThrough)
        {
            // 클릭 통과 ON → OFF (게임 조작 모드)
            WindowSystemManager.Instance.SetClickThrough(false);
            // _pendingReactivate = true;
        }
        else
        {
            // 클릭 통과 OFF → ON (바탕화면 모드)
            WindowSystemManager.Instance.SetClickThrough(true);
            // _pendingReactivate = false;
        }

        UpdateButtonVisual();
        UpdateBackgroundVisual();
    }

    private bool IsMouseOverWidgetArea()
    {
        if (widgetArea == null) return false;
        return RectTransformUtility.RectangleContainsScreenPoint(
            widgetArea,
            Input.mousePosition,
            _uiCamera
        );
    }

    private void UpdateButtonVisual()
    {
        if (toggleButtonImage == null) return;
        bool isClickThrough = WindowSystemManager.Instance != null && WindowSystemManager.Instance.IsClickThrough;
        toggleButtonImage.color = isClickThrough ? clickThroughActiveColor : clickThroughInactiveColor;
    }

    private void UpdateBackgroundVisual()
    {
        bool isClickThrough = WindowSystemManager.Instance != null && WindowSystemManager.Instance.IsClickThrough;

        // Raycast Target 일괄 제어
        foreach (var graphic in raycastTargets)
        {
            if (graphic != null)
                graphic.raycastTarget = !isClickThrough;
        }

        if (backgroundImage != null)
        {
            Color c = backgroundImage.color;
            c.a = isClickThrough ? 0.3f : 1f;
            backgroundImage.color = c;
        }

        if (CanvasGroup != null)
        {
            CanvasGroup.alpha = isClickThrough ? 0.3f : 1f;
            CanvasGroup.interactable = !isClickThrough;
            CanvasGroup.blocksRaycasts = !isClickThrough;
        }
    }
}