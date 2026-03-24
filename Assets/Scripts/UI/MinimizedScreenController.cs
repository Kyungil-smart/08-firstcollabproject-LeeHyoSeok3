using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 최소화 화면 컨트롤러.
///
/// 기능:
/// 1. 최소화 버튼 위에서 좌측으로 드래그 → 넓이 1.3배 초과 시 메인화면 전환
/// 2. 드래그 중 검은 화면 오버레이 출력 (백그라운드 영향 없음)
/// 3. 1.3배 미만으로 드래그 종료 시 최소화 화면 유지
/// 4. 화면 전환 후 모니터 이탈 시 초기 위치로 강제 이동
/// </summary>
public class MinimizedScreenController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("최소화 버튼 (드래그 시작점)")]
    [SerializeField] private RectTransform minimizeButtonRect;

    [Header("검은 화면 오버레이 (맨 앞 레이어)")]
    [SerializeField] private GameObject blackScreenOverlay;

    [Header("현재 최소화 화면 RectTransform")]
    [SerializeField] private RectTransform minimizedScreenRect;

    // 메인화면 전환 임계 배율 (기획서: 1.3배)
    private const float EXPAND_THRESHOLD = 1.3f;

    private bool _isDragging = false;
    private Vector2 _dragStartPos;
    private float _baseWidth;
    private ScreenBoundsHandler _boundsHandler;

    private void Start()
    {
        _boundsHandler = FindObjectOfType<ScreenBoundsHandler>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 최소화 버튼 위에서만 드래그 시작
        if (!RectTransformUtility.RectangleContainsScreenPoint(minimizeButtonRect, eventData.position, eventData.pressEventCamera))
            return;

        _isDragging = true;
        _dragStartPos = eventData.position;
        _baseWidth = minimizedScreenRect != null ? minimizedScreenRect.rect.width : 100f;

        if (blackScreenOverlay) blackScreenOverlay.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        // 좌측으로 드래그한 거리
        float dragDistance = _dragStartPos.x - eventData.position.x;
        if (dragDistance < 0) dragDistance = 0; // 우측 드래그 무시

        // 최소화 화면 넓이 증가 시각화
        if (minimizedScreenRect != null)
        {
            float newWidth = _baseWidth + dragDistance;
            minimizedScreenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging) return;
        _isDragging = false;

        float dragDistance = _dragStartPos.x - eventData.position.x;
        if (dragDistance < 0) dragDistance = 0;

        float expandedWidth = _baseWidth + dragDistance;
        float expandRatio = expandedWidth / _baseWidth;

        if (blackScreenOverlay) blackScreenOverlay.SetActive(false);

        // 최소화 화면 크기 원복
        if (minimizedScreenRect != null)
            minimizedScreenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _baseWidth);

        if (expandRatio >= EXPAND_THRESHOLD)
        {
            // 메인화면으로 전환
            ScreenStateManager.Instance?.GoToMain();

            // 전환 후 모니터 이탈 시 초기 위치로 강제 이동
            _boundsHandler?.ResetToInitialPosition(ScreenStateManager.ScreenState.Main);
        }
        // 1.3배 미만이면 최소화 화면 유지 (아무 것도 하지 않음)
    }
}
