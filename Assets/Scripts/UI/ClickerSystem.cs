using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

/// <summary>
/// 최소화 화면 내부 클릭 시 동작하는 재화 클리커 시스템.
///
/// 기능:
/// 1. 최소화 화면 내부 클릭 → 재화 획득
/// 2. 클릭 위치에 N초간 텍스트 팝업 출력 (최소화 프레임 내에서만)
/// 3. 보유 재화량 갱신 → HUD 텍스트 UI 업데이트
/// 4. 클릭 통과 모드 중에는 토글 버튼 1회 상호작용 후에만 동작
/// </summary>
public class ClickerSystem : MonoBehaviour, IPointerClickHandler
{
    [Header("재화 관련")]
    [SerializeField] private long currencyPerClick = 1;

    [Header("HUD 보유 재화 텍스트 UI")]
    [SerializeField] private TextMeshProUGUI currencyDisplayText;

    [Header("클릭 팝업 텍스트 (Prefab 또는 씬 내 오브젝트)")]
    [SerializeField] private TextMeshProUGUI clickPopupText;
    [SerializeField] private float popupDuration = 1.5f;

    [Header("최소화 화면 프레임 RectTransform (팝업 클리핑 기준)")]
    [SerializeField] private RectTransform minimizedFrameRect;

    // 보유 재화량 (외부에서 초기값 주입 가능)
    public long CurrentCurrency { get; private set; } = 0;

    private Coroutine _popupCoroutine;
    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        UpdateCurrencyDisplay();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭 통과 모드 중에는 동작 안 함
        if (WindowSystemManager.Instance != null && WindowSystemManager.Instance.IsClickThrough)
            return;

        AddCurrency(currencyPerClick);
        ShowClickPopup(eventData.position);
    }

    /// <summary>재화 추가</summary>
    public void AddCurrency(long amount)
    {
        CurrentCurrency += amount;
        UpdateCurrencyDisplay();
    }

    /// <summary>재화량 직접 설정 (저장 데이터 로드 시 사용)</summary>
    public void SetCurrency(long amount)
    {
        CurrentCurrency = amount;
        UpdateCurrencyDisplay();
    }

    private void UpdateCurrencyDisplay()
    {
        if (currencyDisplayText == null) return;
        currencyDisplayText.text = CurrentCurrency.ToString("N0");
    }

    private void ShowClickPopup(Vector2 screenPosition)
    {
        if (clickPopupText == null) return;

        // 팝업 텍스트 위치를 클릭 위치로 설정
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimizedFrameRect,
            screenPosition,
            _canvas?.worldCamera,
            out Vector2 localPoint
        );

        // 최소화 프레임 내부로 클리핑
        localPoint.x = Mathf.Clamp(localPoint.x, minimizedFrameRect.rect.xMin, minimizedFrameRect.rect.xMax);
        localPoint.y = Mathf.Clamp(localPoint.y, minimizedFrameRect.rect.yMin, minimizedFrameRect.rect.yMax);

        clickPopupText.rectTransform.anchoredPosition = localPoint;
        clickPopupText.text = $"+{currencyPerClick:N0}";

        if (_popupCoroutine != null) StopCoroutine(_popupCoroutine);
        _popupCoroutine = StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        clickPopupText.gameObject.SetActive(true);
        yield return new WaitForSeconds(popupDuration);
        clickPopupText.gameObject.SetActive(false);
    }
}
