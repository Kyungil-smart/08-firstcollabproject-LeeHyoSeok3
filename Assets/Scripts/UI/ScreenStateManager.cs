using UnityEngine;
using System;

/// <summary>
/// 게임 화면 상태를 관리합니다.
/// 메인화면 / 최소화 / 월드맵 / 월드맵최소화 간 전환을 담당합니다.
/// </summary>
public class ScreenStateManager : MonoBehaviour
{
    public static ScreenStateManager Instance { get; private set; }

    public enum ScreenState
    {
        Main,
        Minimized,
        WorldMap,
        WorldMapMinimized
    }

    public ScreenState CurrentState { get; private set; } = ScreenState.Main;

    // 화면 전환 이벤트 (다른 시스템이 구독하여 연계 동작)
    public event Action<ScreenState, ScreenState> OnScreenStateChanged; // (이전 상태, 새 상태)

    [Header("각 화면별 루트 GameObject")]
    [SerializeField] private GameObject mainScreenRoot;
    [SerializeField] private GameObject minimizedScreenRoot;
    [SerializeField] private GameObject worldMapRoot;
    [SerializeField] private GameObject worldMapMinimizedRoot;

    private ResponsiveUIScaler _uiScaler;
    private ScreenBoundsHandler _boundsHandler;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (AdventureManager.Instance != null)
        {
            AdventureManager.Instance.OnAdventureStarted += HandleAdventureStarted;
            AdventureManager.Instance.OnAdventureCompleted += HandleAdventureCompleted;
        }
    }

    private void OnDisable()
    {
        if (AdventureManager.Instance != null)
        {
            AdventureManager.Instance.OnAdventureStarted -= HandleAdventureStarted;
            AdventureManager.Instance.OnAdventureCompleted -= HandleAdventureCompleted;
        }
    }

    private void Start()
    {
        _uiScaler = FindObjectOfType<ResponsiveUIScaler>();
        _boundsHandler = FindObjectOfType<ScreenBoundsHandler>();
        ForceState(ScreenState.Main);
    }

    /// <summary>지정된 화면 상태로 전환합니다.</summary>
    public void TransitionTo(ScreenState newState)
    {
        ScreenState previousState = CurrentState;
        if (previousState == newState)
            return;

        CurrentState = newState;

        UpdateScreenVisibility();
        _uiScaler?.ApplyScale();

        OnScreenStateChanged?.Invoke(previousState, newState);
    }

    public void GoToMain() => TransitionTo(ScreenState.Main);
    public void GoToMinimized() => TransitionTo(ScreenState.Minimized);
    public void GoToWorldMap() => TransitionTo(ScreenState.WorldMap);
    public void GoToWorldMapMinimized() => TransitionTo(ScreenState.WorldMapMinimized);

    private void ForceState(ScreenState newState)
    {
        ScreenState previousState = CurrentState;
        CurrentState = newState;

        UpdateScreenVisibility();
        _uiScaler?.ApplyScale();

        OnScreenStateChanged?.Invoke(previousState, newState);
    }

    // 모험 중 최소화 창 대비 화면 전환 처리
    private void HandleAdventureStarted()
    {
        if (CurrentState == ScreenState.Minimized)
        {
            GoToWorldMapMinimized();
        }
        else
        {
            GoToWorldMap();
        }
    }

    private void HandleAdventureCompleted()
    {
        if (CurrentState == ScreenState.WorldMapMinimized)
        {
            GoToMinimized();
        }
        else
        {
            GoToMain();
        }
    }

    private static bool IsMinimizedState(ScreenState state)
    {
        return state == ScreenState.Minimized || state == ScreenState.WorldMapMinimized;
    }

    private void UpdateScreenVisibility()
    {
        if (mainScreenRoot) mainScreenRoot.SetActive(CurrentState == ScreenState.Main);
        if (minimizedScreenRoot) minimizedScreenRoot.SetActive(CurrentState == ScreenState.Minimized);
        if (worldMapRoot) worldMapRoot.SetActive(CurrentState == ScreenState.WorldMap);
        if (worldMapMinimizedRoot) worldMapMinimizedRoot.SetActive(CurrentState == ScreenState.WorldMapMinimized);
    }
}
