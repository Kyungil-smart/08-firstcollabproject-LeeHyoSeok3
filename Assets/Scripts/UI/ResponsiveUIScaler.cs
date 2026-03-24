using UnityEngine;
using System.Collections;

/// <summary>
/// 유저 모니터의 화면 비율에 맞춰 게임 창 크기를 자동으로 조절합니다.
///
/// 각 화면(메인, 최소화, 월드맵)별로 기준 해상도 비율을 유지하면서
/// 현재 모니터 해상도에 비례하여 창 크기를 조절합니다.
/// UI 담당자의 기획서에서 각 화면별 해상도 비율을 반드시 확인 후 입력하세요.
/// </summary>
public class ResponsiveUIScaler : MonoBehaviour
{
    [System.Serializable]
    public struct ScreenSizeConfig
    {
        [Tooltip("기준 해상도 너비 (UI 기획서 참고)")]
        public float referenceWidth;
        [Tooltip("기준 해상도 높이 (UI 기획서 참고)")]
        public float referenceHeight;
        [Tooltip("모니터 너비 대비 창 너비 비율 (0~1)")]
        [Range(0.1f, 1f)]
        public float widthRatio;
    }

    [Header("화면별 크기 설정 (UI 설명서 가안 0.0.3 기준)")]
    // 최대화 화면(메인): FHD 기준 가로 80%, 세로 100px
    [SerializeField] private ScreenSizeConfig mainScreenConfig = new ScreenSizeConfig
    {
        referenceWidth = 1920f, referenceHeight = 400f, widthRatio = 0.8f
    };
    // 최소화 화면: FHD 기준 가로 16%, 세로 29.63% → 320×320 (1:1)
    [SerializeField] private ScreenSizeConfig minimizedConfig = new ScreenSizeConfig
    {
        referenceWidth = 320f, referenceHeight = 320f, widthRatio = 0.1667f
    };
    // 퀘스트(모험) 최대화 화면(월드맵): FHD 기준 가로 93.75%, 세로 29.63% → 1800×320
    [SerializeField] private ScreenSizeConfig worldMapConfig = new ScreenSizeConfig
    {
        referenceWidth = 1800f, referenceHeight = 320f, widthRatio = 0.9375f
    };

    private ScreenStateManager _stateManager;
    private Resolution _lastResolution;

    private void Start()
    {
        _stateManager = FindObjectOfType<ScreenStateManager>();
        _lastResolution = Screen.currentResolution;
        StartCoroutine(ApplyScaleNextFrame());
    }

    // UniWindowController 초기화 완료 후 적용되도록 한 프레임 대기
    private IEnumerator ApplyScaleNextFrame()
    {
        yield return null;
        ApplyScale();
    }

    private void Update()
    {
        // 모니터 해상도 변경 감지
        Resolution current = Screen.currentResolution;
        if (current.width != _lastResolution.width || current.height != _lastResolution.height)
        {
            _lastResolution = current;
            ApplyScale();
        }
    }

    /// <summary>현재 화면 상태에 맞는 창 크기를 적용합니다.</summary>
    public void ApplyScale()
    {
        if (WindowSystemManager.Instance == null) return;

        ScreenSizeConfig config = GetCurrentConfig();
        Resolution screen = Screen.currentResolution;

        float newWidth = screen.width * config.widthRatio;
        float aspectRatio = config.referenceHeight / config.referenceWidth;
        float newHeight = newWidth * aspectRatio;

        WindowSystemManager.Instance.SetWindowSize(new Vector2(newWidth, newHeight));
    }

    private ScreenSizeConfig GetCurrentConfig()
    {
        if (_stateManager == null) return mainScreenConfig;

        return _stateManager.CurrentState switch
        {
            ScreenStateManager.ScreenState.Minimized => minimizedConfig,
            ScreenStateManager.ScreenState.WorldMap => worldMapConfig,
            ScreenStateManager.ScreenState.WorldMapMinimized => minimizedConfig,
            _ => mainScreenConfig,
        };
    }
}
