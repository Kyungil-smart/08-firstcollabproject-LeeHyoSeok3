using UnityEngine;
using UnityEngine.UI;
using Kirurobo;

/// <summary>
/// 위젯 창 시스템의 핵심 관리자.
/// UniWindowController를 통해 투명 배경, 항상 최상위, 클릭 통과 모드를 제어합니다.
/// 반드시 빌드 후 Windows 환경에서 테스트하세요.
/// </summary>
public class WindowSystemManager : MonoBehaviour
{
    public static WindowSystemManager Instance { get; private set; }

    [Header("UniWindowController 참조")]
    [SerializeField] private UniWindowController uniWinController;

    // 클릭 통과 모드 상태
    //public bool IsClickThrough => uniWinController != null && uniWinController.isClickThrough;

    private bool _isClickThrough = false;
    public bool IsClickThrough => _isClickThrough;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (uniWinController == null)
            uniWinController = FindObjectOfType<UniWindowController>();

        if (uniWinController == null)
        {
            Debug.LogError("[WindowSystemManager] UniWindowController를 찾을 수 없습니다.");
            return;
        }

        InitializeWindow();
    }

    private void InitializeWindow()
    {
        // 투명 배경 활성화
        uniWinController.isTransparent = true;
        // 항상 최상위
        uniWinController.isTopmost = true;
        // 초기 클릭 통과 모드: 비활성화 (인터랙션 모드로 시작)
       // SetClickThrough(false);
    }

    /// <summary>클릭 통과 모드 전환</summary>
    public void SetClickThrough(bool enable)
    {
        if (uniWinController == null) return;
        _isClickThrough = enable;
        //uniWinController.isClickThrough = enable;
    }

    /// <summary>창 위치 반환 (스크린 좌표)</summary>
    public Vector2 GetWindowPosition()
    {
        if (uniWinController == null) return Vector2.zero;
        return uniWinController.windowPosition;
    }

    /// <summary>창 위치 설정 (스크린 좌표)</summary>
    public void SetWindowPosition(Vector2 position)
    {
        if (uniWinController == null) return;
        uniWinController.windowPosition = position;
    }

    /// <summary>창 크기 반환</summary>
    public Vector2 GetWindowSize()
    {
        if (uniWinController == null) return Vector2.zero;
        return uniWinController.windowSize;
    }

    /// <summary>창 크기 설정</summary>
    public void SetWindowSize(Vector2 size)
    {
        if (uniWinController == null) return;
        uniWinController.windowSize = size;
    }
}
