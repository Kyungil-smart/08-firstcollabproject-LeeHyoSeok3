using UnityEngine;

/// <summary>
/// 창이 모니터 프레임을 이탈했을 때 자동으로 좌표를 보정합니다.
///
/// 이탈 기준: 최소화 버튼 UI 크기의 30%
/// 보정 방식: 이탈 방향(좌/우/상/하)에 따라 X 또는 Y 좌표를 이탈하지 않는 영역까지 이동
/// 화면 전환(최소화 드래그) 후 이탈 시: 지정된 초기 위치로 강제 이동
/// </summary>
public class ScreenBoundsHandler : MonoBehaviour
{
    [Header("최소화 버튼 UI RectTransform (이탈 기준 크기 계산용)")]
    [SerializeField] private RectTransform minimizeButtonRect;

    // UI 설명서 기준: 모든 화면 초기 위치 = 작업표시줄 위, 우하단
    // 작업표시줄 높이 = 44px (UI 설명서 2p)
    // 초기 위치는 Start()에서 모니터 해상도 기준으로 동적 계산됨
    private const float TASKBAR_HEIGHT = 44f;

    [Header("각 화면별 초기 위치 (런타임에 자동 계산, 필요 시 오버라이드 가능)")]
    [SerializeField] private bool useAutoInitialPosition = true;
    [SerializeField] private Vector2 mainScreenInitialPosition;
    [SerializeField] private Vector2 worldMapInitialPosition;

    // 이탈 기준: 최소화 버튼 크기의 30%
    private const float ESCAPE_THRESHOLD_RATIO = 0.3f;

    private void Start()
    {
        if (useAutoInitialPosition)
            CalculateInitialPositions();
    }

    /// <summary>모니터 해상도 기준으로 초기 위치(우하단, 작업표시줄 위)를 자동 계산합니다.</summary>
    private void CalculateInitialPositions()
    {
        Resolution screen = Screen.currentResolution;

        // 최대화 화면 (80% 너비, 320 높이)
        float mainW = screen.width * 0.8f;
        float mainH = 320f * (screen.height / 1080f);
        mainScreenInitialPosition = new Vector2(
            screen.width - mainW,
            screen.height - mainH - TASKBAR_HEIGHT
        );

        // 퀘스트 최대화 화면 (93.75% 너비, 320 높이)
        float worldW = screen.width * 0.9375f;
        float worldH = 320f * (screen.height / 1080f);
        worldMapInitialPosition = new Vector2(
            screen.width - worldW,
            screen.height - worldH - TASKBAR_HEIGHT
        );
    }

    /// <summary>
    /// 드래그 종료 후 화면 이탈 여부를 확인하고 자동 보정합니다.
    /// </summary>
    public void CheckAndCorrectBounds()
    {
        if (WindowSystemManager.Instance == null) return;

        Vector2 windowPos = WindowSystemManager.Instance.GetWindowPosition();
        Vector2 windowSize = WindowSystemManager.Instance.GetWindowSize();

        float escapeThreshold = GetEscapeThreshold();
        Resolution screen = Screen.currentResolution;

        float correctedX = windowPos.x;
        float correctedY = windowPos.y;
        bool needsCorrection = false;

        // 좌측 이탈
        if (windowPos.x + escapeThreshold < 0)
        {
            correctedX = 0;
            needsCorrection = true;
        }
        // 우측 이탈
        else if (windowPos.x + windowSize.x - escapeThreshold > screen.width)
        {
            correctedX = screen.width - windowSize.x;
            needsCorrection = true;
        }

        // 상단 이탈
        if (windowPos.y + escapeThreshold < 0)
        {
            correctedY = 0;
            needsCorrection = true;
        }
        // 하단 이탈
        else if (windowPos.y + windowSize.y - escapeThreshold > screen.height)
        {
            correctedY = screen.height - windowSize.y;
            needsCorrection = true;
        }

        if (needsCorrection)
        {
            WindowSystemManager.Instance.SetWindowPosition(new Vector2(correctedX, correctedY));
        }
    }

    /// <summary>
    /// 화면 전환(드래그) 후 이탈 시 지정된 초기 위치로 강제 이동합니다.
    /// </summary>
    public void ResetToInitialPosition(ScreenStateManager.ScreenState targetState)
    {
        if (WindowSystemManager.Instance == null) return;

        Vector2 targetPos = targetState == ScreenStateManager.ScreenState.WorldMap
            ? worldMapInitialPosition
            : mainScreenInitialPosition;

        if (IsWindowOutOfBounds())
        {
            WindowSystemManager.Instance.SetWindowPosition(targetPos);
        }
    }

    public bool IsWindowOutOfBounds()
    {
        if (WindowSystemManager.Instance == null) return false;

        Vector2 windowPos = WindowSystemManager.Instance.GetWindowPosition();
        Vector2 windowSize = WindowSystemManager.Instance.GetWindowSize();
        float escapeThreshold = GetEscapeThreshold();
        Resolution screen = Screen.currentResolution;

        return windowPos.x + escapeThreshold < 0
            || windowPos.x + windowSize.x - escapeThreshold > screen.width
            || windowPos.y + escapeThreshold < 0
            || windowPos.y + windowSize.y - escapeThreshold > screen.height;
    }

    private float GetEscapeThreshold()
    {
        if (minimizeButtonRect == null) return 20f; // 기본값
        float buttonWidth = minimizeButtonRect.rect.width;
        float buttonHeight = minimizeButtonRect.rect.height;
        // 짧은 변 기준 30%
        return Mathf.Min(buttonWidth, buttonHeight) * ESCAPE_THRESHOLD_RATIO;
    }
}
