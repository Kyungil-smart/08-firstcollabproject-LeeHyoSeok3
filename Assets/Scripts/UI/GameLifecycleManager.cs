using UnityEngine;
using System;
using UnityEngine.UI;
using DesignPattern;
using System.Collections;

/// <summary>
/// 게임 시작 / 종료 시 타이머 시스템과의 연계를 담당합니다.
///
/// 게임 시작:
/// - 타이머 시스템 기획서 4p 자동생산 예외처리 2항,
///   5p 퀘스트 예외처리 1항, 7~9p 오프라인 보상 처리 완료 후
///   메인화면 UI 상호작용 활성화
///
/// 게임 종료:
/// - 오프라인 시스템 모든 기능 정지
/// - 자동 생산 시스템 예외처리 4p 2항 반영
/// </summary>
public class GameLifecycleManager : Singleton<GameLifecycleManager>
{
    [SerializeField] private Button quitButton; // 게임 종료 버튼 (UI에 배치된 버튼 연결)

    // // 최소화 화면 전환 시각 저장 키 (오프라인 보상 연계) -> LastInactiveTimeUtc로 통합(OfflineRewardManager에서 관리)
    // private const string MINIMIZED_TIME_KEY = "MinimizedTimestamp";
    // private const string LAST_QUIT_TIME_KEY = "LastQuitTimestamp";

    [Header("메인화면 UI 그룹 (시작 시 비활성화 → 처리 완료 후 활성화)")]
    [SerializeField] private GameObject mainScreenUIGroup;

    [Header("Managers")]
    [SerializeField] private OfflineRewardManager offlineRewardManager;

    protected override void Awake()
    {
        base.Awake();
    }
    
    private void Start()
    {
        if (quitButton != null) quitButton.onClick.AddListener(OnClickQuitButton);

        StartCoroutine(CoStartGameFlow());
    }

    private IEnumerator CoStartGameFlow()
    {
        // 모든 Start()가 끝난 뒤 실행되도록 한 프레임 대기
        yield return null;

        OnGameStart();
    }

    // ─── 게임 시작 ────────────────────────────────────────────────────

    private void OnGameStart()
    {
        // 메인화면 UI 비활성화 (오프라인 보상 처리 전)
        if (mainScreenUIGroup) mainScreenUIGroup.SetActive(false);

        if (offlineRewardManager != null)
        {
            offlineRewardManager.ProcessOfflineReward(OnOfflineRewardProcessComplete);
        }
        else
        {
            OnOfflineRewardProcessComplete();
        }

        // TODO: 타이머 시스템 담당자와 연계 필요
        // 아래 순서대로 처리 후 OnOfflineRewardProcessComplete() 호출
        // 1. 자동 생산 시스템 예외처리 (타이머 기획서 4p 2항)
        // 2. 퀘스트 시스템 예외처리 (타이머 기획서 5p 1항)
        // 3. 오프라인 보상 처리 (타이머 기획서 7~9p)
    }

    /// <summary>오프라인 보상 처리 완료 후 호출 (타이머 시스템에서 호출)</summary>
    public void OnOfflineRewardProcessComplete()
    {
        if (mainScreenUIGroup) mainScreenUIGroup.SetActive(true);
    }

    // ─── 게임 종료 ────────────────────────────────────────────────────

    /// <summary>
    /// 유저가 직접 "종료 버튼"을 눌렀을 때 호출됨
    /// </summary>
    private void OnClickQuitButton()
    {
        SaveInactiveTime();
        Application.Quit();
    }

    /// <summary>
    /// 앱 포커스를 잃었을 때 호출됨
    /// Alt+Tab, 다른 창 클릭 등
    /// hasFocus == false → 비활성 시작
    /// </summary>
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveInactiveTime();
        }
    }

    /// <summary>
    /// 게임이 완전히 종료되는 경우 호출
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveInactiveTime();
    }

    /// <summary>
    /// 오프라인 보상 계산 기준이 되는 비활성 시작 시각 저장
    /// 최소화, 포커스 잃음, 종료 등 모든 경우 공통 처리
    /// </summary>
    private void SaveInactiveTime()
    {
        OfflineRewardManager.SaveLastInactiveTimeUtc();
    }


    // ─── 최소화 화면 전환 시각 저장 ──────────────────────────────────

    // /// <summary>최소화 화면으로 전환 시 로컬 시각 저장 (오프라인 보상 연계)</summary>
    // public void SaveMinimizedTime()
    // {
    //     PlayerPrefs.SetString(MINIMIZED_TIME_KEY, DateTime.Now.ToString("o"));
    //     PlayerPrefs.Save();
    // }

    // /// <summary>저장된 최소화 전환 시각 반환</summary>
    // public DateTime GetMinimizedTime()
    // {
    //     string saved = PlayerPrefs.GetString(MINIMIZED_TIME_KEY, string.Empty);
    //     if (string.IsNullOrEmpty(saved)) return DateTime.MinValue;
    //     return DateTime.Parse(saved);
    // }

    // ─── 게임 종료 ────────────────────────────────────────────────────

    // private void OnGameEnd()
    // {
    //     // 종료 시각 저장
    //     PlayerPrefs.SetString(LAST_QUIT_TIME_KEY, DateTime.Now.ToString("o"));
    //     PlayerPrefs.Save();

    //     // TODO: 타이머 시스템 담당자와 연계 필요
    //     // 1. 오프라인 시스템 모든 기능 정지
    //     // 2. 자동 생산 시스템 예외처리 (타이머 기획서 4p 2항) 반영
    // }

    // /// <summary>저장된 마지막 종료 시각 반환</summary>
    // public DateTime GetLastQuitTime()
    // {
    //     string saved = PlayerPrefs.GetString(LAST_QUIT_TIME_KEY, string.Empty);
    //     if (string.IsNullOrEmpty(saved)) return DateTime.MinValue;
    //     return DateTime.Parse(saved);
    // }
}