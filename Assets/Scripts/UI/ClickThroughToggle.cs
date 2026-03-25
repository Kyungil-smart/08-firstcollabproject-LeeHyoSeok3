using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 클릭 통과 모드 토글 버튼 컨트롤러.
///
/// 동작 규칙:
/// - 클릭 통과 모드(활성화) 중 토글 버튼 1회 클릭 → 클릭 통과 비활성화
/// - 이후 마우스가 위젯 영역 밖으로 나가면 → 다시 클릭 통과 활성화
/// - 2회 이상 연속 클릭해도 1회로 판정
/// - 클릭 통과 비활성화 상태에서 토글 버튼 클릭 → 클릭 통과 활성화
/// </summary>
public class ClickThroughToggle : MonoBehaviour, IPointerClickHandler
{
    [Header("위젯 영역 RectTransform (마우스 이탈 감지용)")]
    [SerializeField] private RectTransform widgetArea;

    [Header("토글 버튼 시각 피드백 (선택)")]
    [SerializeField] private UnityEngine.UI.Image toggleButtonImage;
    [SerializeField] private Color clickThroughActiveColor = Color.green;
    [SerializeField] private Color clickThroughInactiveColor = Color.gray;

    private bool _pendingReactivate = false; // 클릭 통과 재활성화 대기 상태
    private bool _buttonClickedThisFrame = false;
    private Camera _uiCamera;

    private void Start()
    {
        _uiCamera = Camera.main;
        UpdateButtonVisual();
    }

    private void Update()
    {
        _buttonClickedThisFrame = false;

        // 클릭 통과 재활성화 대기 중 → 마우스가 위젯 영역 밖으로 나갔는지 확인
        if (_pendingReactivate && !IsMouseOverWidgetArea())
        {
            WindowSystemManager.Instance.SetClickThrough(true);
            _pendingReactivate = false;
            UpdateButtonVisual();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_buttonClickedThisFrame) return; // 같은 프레임 내 중복 클릭 무시
        _buttonClickedThisFrame = true;

        bool currentlyClickThrough = WindowSystemManager.Instance.IsClickThrough;

        if (currentlyClickThrough)
        {
            // 클릭 통과 모드 중 → 비활성화, 재활성화 대기
            WindowSystemManager.Instance.SetClickThrough(false);
            _pendingReactivate = true;
        }
        else
        {
            // 인터랙션 모드 중 → 클릭 통과 활성화
            WindowSystemManager.Instance.SetClickThrough(true);
            _pendingReactivate = false;
        }

        UpdateButtonVisual();
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
}
