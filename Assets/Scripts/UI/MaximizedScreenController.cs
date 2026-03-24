using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 최대화 화면(메인화면)의 최소화 버튼 드래그 컨트롤러.
/// UI 설명서 6p 기준:
/// 최소화 버튼을 클릭 후 우로 드래그 → 화면 넓이가 줄어들어
/// 일정 크기 이하가 되면 최소화 화면으로 전환.
/// </summary>
public class MaximizedScreenController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("최소화 버튼 RectTransform")]
    [SerializeField] private RectTransform minimizeButtonRect;

    [Header("검은 화면 오버레이")]
    [SerializeField] private GameObject blackScreenOverlay;

    [Header("최대화 화면 RectTransform")]
    [SerializeField] private RectTransform maximizedScreenRect;

    // 최소화 전환 임계 배율 (1.3배의 역수 = 약 0.77배 이하)
    // UI 설명서: "일정 크기 이하로 작아지면 최소화 화면으로 전환"
    // 창 전환 시스템 기획서 8p와 대칭: 최소화→최대화가 1.3배이므로
    // 최대화→최소화는 1/1.3 ≈ 0.769배 이하
    private const float SHRINK_THRESHOLD = 1f / 1.3f;

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
        if (!RectTransformUtility.RectangleContainsScreenPoint(
            minimizeButtonRect, eventData.position, eventData.pressEventCamera))
            return;

        _isDragging = true;
        _dragStartPos = eventData.position;
        _baseWidth = maximizedScreenRect != null ? maximizedScreenRect.rect.width : 1536f;

        if (blackScreenOverlay) blackScreenOverlay.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        // 우측으로 드래그한 거리만큼 넓이 감소
        float dragDistance = eventData.position.x - _dragStartPos.x;
        if (dragDistance < 0) dragDistance = 0; // 좌측 드래그 무시

        if (maximizedScreenRect != null)
        {
            float newWidth = Mathf.Max(_baseWidth - dragDistance, 0f);
            maximizedScreenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging) return;
        _isDragging = false;

        float dragDistance = eventData.position.x - _dragStartPos.x;
        if (dragDistance < 0) dragDistance = 0;

        float shrinkRatio = (_baseWidth - dragDistance) / _baseWidth;

        if (blackScreenOverlay) blackScreenOverlay.SetActive(false);

        // 최대화 화면 크기 원복
        if (maximizedScreenRect != null)
            maximizedScreenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _baseWidth);

        if (shrinkRatio <= SHRINK_THRESHOLD)
        {
            // 최소화 화면으로 전환
            ScreenStateManager.Instance?.GoToMinimized();
        }
        // 임계값 미달 시 최대화 화면 유지 (아무 것도 하지 않음)
    }
}
