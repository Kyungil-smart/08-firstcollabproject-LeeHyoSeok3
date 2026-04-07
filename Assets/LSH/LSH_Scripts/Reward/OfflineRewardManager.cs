using System;
using UnityEngine;

public class OfflineRewardManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private OfflineRewardPopupUI m_offlineRewardPopupUI;
    [SerializeField] private RectTransform m_rewardPopup;

    [Header("System References")]
    [SerializeField] private AnvilGoldController m_anvilGoldController; // 지동 생산 시스템 참조

    // 최대 인정 오프라인 시간 (12시간 = 43200초)
    private const float MAX_OFFLINE_SECONDS = 43200f;
    private const string LAST_INACTIVE_TIME_KEY = "LastInactiveTimeUtc";

    // ---------------------Test-------------------------------------
    // private AutoProductionTimer m_autoTimer; // 테스트용(자동 생산)
    // private QuestTimer m_questTimer; // 테스트용(퀘스트)
    // --------------------------------------------------------------

    private Action m_onProcessComplete; // 보상 계산 완료 후 호출할 콜백
    private int m_pendingAutoRewardGold;      // 팝업에 표시한 자동 생산 보상 저장
    private float m_pendingOfflineSeconds;    // 팝업에 표시한 오프라인 시간 저장

    private void Awake()
    {
        // m_questTimer = new QuestTimer(7200f);
    }

    private void Start()
    {
        // 팝업은 처음에 무조건 숨깁니다.
        // if (m_totalRewardPopup != null) m_totalRewardPopup.SetActive(false);

        // 테스트용 타이머 생성 (자동생산: 5초마다 10골드, 퀘스트: 2시간(7200초)짜리)
        // m_autoTimer = new AutoProductionTimer(5f, 10);

        // m_questTimer = new QuestTimer(7200f);
    }

    public void ProcessOfflineReward(Action onComplete)
    {
        m_onProcessComplete = onComplete;
        CalculateOfflineRewards();
    }

    private void CalculateOfflineRewards()
    {
        // // 1. 저장된 종료 시간 불러오기 (저장된 게 없으면 현재 시간으로 세팅)
        // string lastSaveTimeString = PlayerPrefs.GetString("LastLogoutTime", DateTime.UtcNow.ToBinary().ToString());
        // DateTime lastLogoutTime = DateTime.FromBinary(Convert.ToInt64(lastSaveTimeString));

        // // 2. 시간 차이 계산 (초 단위) 
        // TimeSpan offlineSpan = DateTime.UtcNow - lastLogoutTime;
        // float offlineSeconds = (float)offlineSpan.TotalSeconds;

        // // 3. 12시간(43200초) 초과 시 12시간으로 고정
        // if (offlineSeconds > MAX_OFFLINE_SECONDS)
        // {
        //     offlineSeconds = MAX_OFFLINE_SECONDS;
        // }

        DateTime lastInactiveTime = LoadLastInactiveTimeUtc();
        Debug.Log($"[OfflineReward] lastInactiveTime = {lastInactiveTime}");

        // 첫 실행이거나 저장값이 없으면 바로 완료
        if (lastInactiveTime == DateTime.MinValue)
        {
            Debug.Log("[OfflineReward] 저장값 없음 -> 팝업 생략");
            CompleteProcess();
            return;
        }

        float offlineSeconds = (float)(DateTime.UtcNow - lastInactiveTime).TotalSeconds;
        offlineSeconds = Mathf.Clamp(offlineSeconds, 0f, MAX_OFFLINE_SECONDS);

        // 방치 시간이 거의 없다면(1초 미만) 팝업을 띄우지 않고 바로 게임 진행
        if (offlineSeconds < 1f)
        {
            Debug.Log("[OfflineReward] 1초 미만 -> 팝업 생략");
            ResumeGame();
            CompleteProcess();
            return;
        }

        // 4. 자동 생산 보상 계산 (소수점 버림 처리) 
        // int autoRewardGold = m_autoTimer.CalculateOfflineReward(offlineSeconds);
        //int autoRewardGold = CalculateAutoProductionOfflineReward(offlineSeconds);
        m_pendingOfflineSeconds = offlineSeconds;
        m_pendingAutoRewardGold = CalculateAutoProductionOfflineReward(offlineSeconds);
        Debug.Log($"[OfflineReward] 지급 예정 골드: {m_pendingAutoRewardGold}");

        if (m_pendingAutoRewardGold <= 0)
        {
            Debug.Log("[OfflineReward] 자동 생산 보상 없음 -> 팝업 생략");
            ResumeGame();
            CompleteProcess();
            return;
        }

        // 5. 퀘스트 남은 시간 계산 및 완료 처리
        //if (m_questTimer != null) m_questTimer.ApplyOfflineTime(offlineSeconds);

        // 6. 모든 계산이 끝난 후 총 보상 팝업 출력 
        ShowTotalRewardPopup(m_pendingAutoRewardGold, m_pendingOfflineSeconds);
    }

    private int CalculateAutoProductionOfflineReward(float offlineSeconds)
    {
        if (m_anvilGoldController == null) return 0;

        int goldPerSecond = m_anvilGoldController.CurrentGoldPerSecond;

        // 자동 생산 업그레이드가 없으면 보상 없음
        if (goldPerSecond <= 0) return 0;

        float currentAutoTimer = m_anvilGoldController.CurrentAutoTimer;

        // 기존 누적 시간 + 오프라인 시간
        float totalAutoTime = currentAutoTimer + offlineSeconds;

        // 현재 구조는 1초마다 보상 지급
        int rewardTicks = Mathf.FloorToInt(totalAutoTime / 1f);

        // 남은 자투리 시간 복원
        float remainAutoTime = totalAutoTime % 1f;
        m_anvilGoldController.SetAutoTimer(remainAutoTime);

        return rewardTicks * goldPerSecond;
    }

    private void ShowTotalRewardPopup(int earnedGold, float offlineSeconds)
    {
        // m_autoTimer.IsPaused = true;

        // if (m_totalRewardText == null || m_totalRewardPopup == null)
        // {
        //     CompleteProcess();
        //     return;
        // }

        // // 1. 방치한 총 오프라인 시간 출력 (예: 2시간 30분 15초)
        // m_totalRewardText.text = $"방치 시간: {FormatTimeKorean(offlineSeconds)}\n\n";

        // // 2. 자동 생산 골드 보상 출력
        // m_totalRewardText.text += $"자동 생산 수익: {earnedGold} G\n";

        // // 3. 퀘스트 남은 시간 출력 (이곳도 포맷팅 함수 적용)
        // if (m_questTimer.IsComplete)
        // {
        //     m_totalRewardText.text += "퀘스트 탐험대가 복귀했습니다!";
        // }
        // else
        // {
        //     m_totalRewardText.text += $"퀘스트 남은 시간: {FormatTimeKorean(m_questTimer.RemainingSeconds)}";
        // }

        // m_totalRewardPopup.SetActive(true);

        Debug.Log("[OfflineReward] ShowTotalRewardPopup 진입");

        Debug.Log($"[OfflineReward] PopupManager.Instance null 여부: {PopupManager.Instance == null}");
        Debug.Log($"[OfflineReward] m_offlineRewardPopupUI null 여부: {m_offlineRewardPopupUI == null}");
        Debug.Log($"[OfflineReward] m_rewardPopup null 여부: {m_rewardPopup == null}");

        if (PopupManager.Instance == null || m_offlineRewardPopupUI == null)
        {
            Debug.LogWarning("오프라인 보상 팝업을 표시할 수 없습니다.");
            OnClickConfirmPopup();
            return;
        }

        PopupManager.Instance.OpenOfflineRewardPopup();
        m_offlineRewardPopupUI.Setup(BuildPendingRewardMessage, OnClickConfirmPopup);

        Debug.Log("[OfflineReward] 팝업 표시 완료");
    }

    private string BuildPendingRewardMessage()
    {
        return BuildRewardMessage(m_pendingAutoRewardGold, m_pendingOfflineSeconds);
    }

    private string BuildRewardMessage(int earnedGold, float offlineSeconds)
    {
        string formattedTime = FormatOfflineTime(offlineSeconds);
        string formattedGold = GoldManager.FormatGold(earnedGold);

        string awayTemplate = GetLocalizedText("OFFLINE_REWARD_AWAY_FOR", "{0}동안");
        string earnedTemplate = GetLocalizedText("OFFLINE_REWARD_EARNED_GOLD", "{0}원을 벌었어요!");

        return string.Format(awayTemplate, formattedTime) + "\n" + string.Format(earnedTemplate, formattedGold);
    }

    private string FormatOfflineTime(float totalSeconds)
    {
        int hours = Mathf.FloorToInt(totalSeconds / 3600f);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);

        string hourUnit = GetLocalizedText("TIME_HOUR_UNIT", "시간");
        string minuteUnit = GetLocalizedText("TIME_MINUTE_UNIT", "분");
        string secondUnit = GetLocalizedText("TIME_SECOND_UNIT", "초");

        if (hours > 0)
        {
            return $"{hours}{hourUnit} {minutes}{minuteUnit} {seconds}{secondUnit}";
        }

        if (minutes > 0)
        {
            return $"{minutes}{minuteUnit} {seconds}{secondUnit}";
        }

        return $"{seconds}{secondUnit}";
    }

    private string GetLocalizedText(string key, string fallback)
    {
        if (LocalizationManager.Instance == null)
            return fallback;

        string localized = LocalizationManager.Instance.GetText(key);
        return string.IsNullOrEmpty(localized) || localized == key ? fallback : localized;
    }

    // 팝업의 '확인' 버튼을 누르면 호출될 함수
    public void OnClickConfirmPopup()
    {
        ApplyRewards();

        if (PopupManager.Instance != null && m_rewardPopup != null)
        {
            PopupManager.Instance.ClosePopup(m_rewardPopup);
        }

        ResumeGame();
        CompleteProcess();
    }

    private void ApplyRewards()
    {
        if (GoldManager.Instance == null) return;

        if (m_pendingAutoRewardGold > 0)
        {
            GoldManager.Instance.AddGold(m_pendingAutoRewardGold);
        }
    }

    private void CompleteProcess()
    {
        m_onProcessComplete?.Invoke();
        m_onProcessComplete = null;
    }

    public static void SaveLastInactiveTimeUtc()
    {
        string now = DateTime.UtcNow.ToString("o");
        PlayerPrefs.SetString(LAST_INACTIVE_TIME_KEY, now);
        PlayerPrefs.Save();

        Debug.Log($"[OfflineReward] 저장된 비활성 시각: {now}");
    }

    private DateTime LoadLastInactiveTimeUtc()
    {
        string saved = PlayerPrefs.GetString(LAST_INACTIVE_TIME_KEY, string.Empty);
        Debug.Log($"[OfflineReward] 불러온 저장값: {saved}");

        if (string.IsNullOrEmpty(saved))
            return DateTime.MinValue;

        if (DateTime.TryParse(saved, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime result))
            return result;

        return DateTime.MinValue;
    }

    private void ResumeGame()
    {
        // 타이머들 일시정지 해제 및 인게임 로직 다시 시작
        // m_autoTimer.IsPaused = false;
        //   m_questTimer.ResumeTimer();
        Debug.Log("[OfflineReward] 게임 진행 재개");
    }

    // // 게임 종료 또는 창을 닫을 때 시간 저장 
    // private void OnApplicationQuit()
    // {
    //     PlayerPrefs.SetString("LastLogoutTime", DateTime.UtcNow.ToBinary().ToString());
    //     PlayerPrefs.Save();
    // }
}