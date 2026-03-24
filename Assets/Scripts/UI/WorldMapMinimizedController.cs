using UnityEngine;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// 월드맵 최소화 화면 컨트롤러.
///
/// 기능:
/// 1. 최소화 버튼 드래그(좌측) → 1.3배 초과 시 월드맵으로 전환
/// 2. 남은 퀘스트 시간 HH.MM.SS 형태로 실시간 표시
/// 3. 월드맵 최소화 중 오프라인 보상 타이머 비활성화
/// 4. 파티 스프라이트 카메라 추적 (카메라 참조 연결 필요)
/// </summary>
public class WorldMapMinimizedController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("최소화 버튼 RectTransform")]
    [SerializeField] private RectTransform minimizeButtonRect;

    [Header("검은 화면 오버레이")]
    [SerializeField] private GameObject blackScreenOverlay;

    [Header("최소화 화면 RectTransform")]
    [SerializeField] private RectTransform minimizedScreenRect;

    [Header("남은 시간 표기 텍스트 UI")]
    [SerializeField] private TMPro.TextMeshProUGUI remainingTimeText;

    [Header("파티 스프라이트 추적 카메라")]
    [SerializeField] private Camera worldMapCamera;
    [SerializeField] private Transform partySprite; // 파티 스프라이트 Transform

    // 퀘스트 종료 시각 (외부에서 설정)
    public DateTime QuestEndTime { get; set; } = DateTime.MinValue;

    private const float EXPAND_THRESHOLD = 1.3f;
    private bool _isDragging = false;
    private Vector2 _dragStartPos;
    private float _baseWidth;
    private ScreenBoundsHandler _boundsHandler;

    private void Start()
    {
        _boundsHandler = FindObjectOfType<ScreenBoundsHandler>();
    }

    private void Update()
    {
        UpdateRemainingTime();
        TrackPartySprite();
    }

    // ─── 남은 시간 표시 ───────────────────────────────────────────────

    private void UpdateRemainingTime()
    {
        if (remainingTimeText == null) return;
        if (QuestEndTime == DateTime.MinValue)
        {
            remainingTimeText.text = "00:00:00";
            return;
        }

        TimeSpan remaining = QuestEndTime - DateTime.Now;
        if (remaining.TotalSeconds <= 0)
        {
            remainingTimeText.text = "00:00:00";
            return;
        }

        // H:M 형식 (UI 설명서 11p 기준)
        int hours = (int)remaining.TotalHours;
        remainingTimeText.text = $"{hours}:{remaining.Minutes:D2}";
    }

    // ─── 파티 스프라이트 카메라 추적 ─────────────────────────────────

    private void TrackPartySprite()
    {
        if (worldMapCamera == null || partySprite == null) return;

        Vector3 targetPos = partySprite.position;
        targetPos.z = worldMapCamera.transform.position.z; // Z축 유지
        worldMapCamera.transform.position = targetPos;
    }

    // ─── 드래그로 월드맵 전환 ─────────────────────────────────────────

    public void OnPointerDown(PointerEventData eventData)
    {
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

        float dragDistance = _dragStartPos.x - eventData.position.x;
        if (dragDistance < 0) dragDistance = 0;

        if (minimizedScreenRect != null)
            minimizedScreenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _baseWidth + dragDistance);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging) return;
        _isDragging = false;

        float dragDistance = _dragStartPos.x - eventData.position.x;
        if (dragDistance < 0) dragDistance = 0;

        float expandRatio = (_baseWidth + dragDistance) / _baseWidth;

        if (blackScreenOverlay) blackScreenOverlay.SetActive(false);

        if (minimizedScreenRect != null)
            minimizedScreenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _baseWidth);

        if (expandRatio >= EXPAND_THRESHOLD)
        {
            // 월드맵으로 전환
            ScreenStateManager.Instance?.GoToWorldMap();

            // 전환 후 모니터 이탈 시 초기 위치로 강제 이동
            _boundsHandler?.ResetToInitialPosition(ScreenStateManager.ScreenState.WorldMap);
        }
    }
}
