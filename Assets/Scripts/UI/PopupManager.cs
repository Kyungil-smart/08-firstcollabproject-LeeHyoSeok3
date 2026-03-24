using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 팝업 UI 관리자.
/// UI 설명서 2p 기준: 모든 팝업 UI는 사용자 모니터 중앙에 출력.
///
/// 지원 팝업 목록 (UI 설명서 기준):
/// - 퀘스트 보드 팝업 (730×500, FHD 38%×46%)
/// - 대장간(제작소) 팝업 (1300×750, FHD 67.7%×69.4%)
/// - 파티 장비 장착 팝업 (770×320, FHD 40.1%×29.63%)
/// - 환경설정 팝업 (400×400, FHD 20.8%×37%)
/// - 던전 보상재료 획득 팝업
/// </summary>
public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [Header("팝업 프리팹 또는 씬 내 오브젝트")]
    [SerializeField] private RectTransform questBoardPopup;
    [SerializeField] private RectTransform blacksmithPopup;
    [SerializeField] private RectTransform partyEquipPopup;
    [SerializeField] private RectTransform settingsPopup;
    [SerializeField] private RectTransform rewardPopup;

    [Header("팝업을 배치할 Canvas")]
    [SerializeField] private Canvas popupCanvas;

    private readonly List<RectTransform> _activePopups = new List<RectTransform>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ─── 팝업 열기 ───────────────────────────────────────────────────

    public void OpenQuestBoard()       => OpenPopup(questBoardPopup);
    public void OpenBlacksmith()       => OpenPopup(blacksmithPopup);
    public void OpenPartyEquip()       => OpenPopup(partyEquipPopup);
    public void OpenSettings()         => OpenPopup(settingsPopup);
    public void OpenRewardPopup()      => OpenPopup(rewardPopup);

    /// <summary>팝업을 모니터 중앙에 배치하고 활성화합니다.</summary>
    public void OpenPopup(RectTransform popup)
    {
        if (popup == null) return;

        popup.gameObject.SetActive(true);
        CenterOnMonitor(popup);

        if (!_activePopups.Contains(popup))
            _activePopups.Add(popup);
    }

    /// <summary>팝업을 닫습니다.</summary>
    public void ClosePopup(RectTransform popup)
    {
        if (popup == null) return;
        popup.gameObject.SetActive(false);
        _activePopups.Remove(popup);
    }

    public void CloseAllPopups()
    {
        foreach (var popup in _activePopups)
            if (popup != null) popup.gameObject.SetActive(false);
        _activePopups.Clear();
    }

    // ─── 모니터 중앙 배치 ─────────────────────────────────────────────

    /// <summary>
    /// 팝업을 유저 모니터 화면의 중앙에 배치합니다.
    /// 게임 창 위치와 무관하게 모니터 절대 중앙 기준입니다.
    /// </summary>
    private void CenterOnMonitor(RectTransform popup)
    {
        if (popupCanvas == null) return;

        Resolution screen = Screen.currentResolution;
        Vector2 monitorCenter = new Vector2(screen.width * 0.5f, screen.height * 0.5f);

        // 게임 창의 현재 위치를 고려하여 Canvas 로컬 좌표로 변환
        Vector2 gameWindowPos = Vector2.zero;
        if (WindowSystemManager.Instance != null)
            gameWindowPos = WindowSystemManager.Instance.GetWindowPosition();

        // 모니터 중앙을 게임 창 기준 스크린 좌표로 변환
        Vector2 screenPosInWindow = monitorCenter - gameWindowPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            popupCanvas.GetComponent<RectTransform>(),
            screenPosInWindow,
            popupCanvas.worldCamera,
            out Vector2 localPoint
        );

        popup.anchoredPosition = localPoint;
    }

    // ─── 팝업 크기 반응형 스케일링 ────────────────────────────────────

    /// <summary>
    /// FHD 기준 비율로 팝업 크기를 현재 해상도에 맞게 조절합니다.
    /// UI 설명서 각 팝업의 FHD% 수치 사용.
    /// </summary>
    public void ScalePopupToScreen(RectTransform popup, float widthRatio, float heightRatio)
    {
        Resolution screen = Screen.currentResolution;
        float w = screen.width * widthRatio;
        float h = screen.height * heightRatio;
        popup.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        popup.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }

    private void Start()
    {
        // 팝업 초기 크기 설정 (UI 설명서 FHD% 기준)
        if (questBoardPopup)  ScalePopupToScreen(questBoardPopup,  0.38f,  0.46f);
        if (blacksmithPopup)  ScalePopupToScreen(blacksmithPopup,  0.677f, 0.694f);
        if (partyEquipPopup)  ScalePopupToScreen(partyEquipPopup,  0.401f, 0.2963f);
        if (settingsPopup)    ScalePopupToScreen(settingsPopup,    0.208f, 0.37f);

        // 모든 팝업 초기 비활성화
        CloseAllPopups();
        if (questBoardPopup)  questBoardPopup.gameObject.SetActive(false);
        if (blacksmithPopup)  blacksmithPopup.gameObject.SetActive(false);
        if (partyEquipPopup)  partyEquipPopup.gameObject.SetActive(false);
        if (settingsPopup)    settingsPopup.gameObject.SetActive(false);
        if (rewardPopup)      rewardPopup.gameObject.SetActive(false);
    }
}
