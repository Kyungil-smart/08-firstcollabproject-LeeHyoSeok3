using System;
using UnityEngine;

public class OfflineRewardManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject m_totalRewardPopup; // 총 보상 팝업 UI
    [SerializeField] private TMPro.TMP_Text m_totalRewardText; // 팝업 내 보상 텍스트

    // 최대 인정 오프라인 시간 (12시간 = 43200초)
    private const float MAX_OFFLINE_SECONDS = 43200f;

    // 테스트용 임시 타이머 객체들 (실제로는 GameManager 등에서 관리하는 것을 가져옵니다)
    private AutoProductionTimer m_autoTimer;
    private QuestTimer m_questTimer;

    private void Start()
    {
        // 팝업은 처음에 무조건 숨깁니다.
        m_totalRewardPopup.SetActive(false);

        // 테스트용 타이머 생성 (자동생산: 5초마다 10골드, 퀘스트: 2시간(7200초)짜리)
        m_autoTimer = new AutoProductionTimer(5f, 10);
        m_questTimer = new QuestTimer(7200f);

        // 게임 시작 시 오프라인 보상 계산 실행
        CalculateOfflineRewards();
    }

    private void CalculateOfflineRewards()
    {
        // 1. 저장된 종료 시간 불러오기 (저장된 게 없으면 현재 시간으로 세팅)
        string lastSaveTimeString = PlayerPrefs.GetString("LastLogoutTime", DateTime.UtcNow.ToBinary().ToString());
        DateTime lastLogoutTime = DateTime.FromBinary(Convert.ToInt64(lastSaveTimeString));

        // 2. 시간 차이 계산 (초 단위) 
        TimeSpan offlineSpan = DateTime.UtcNow - lastLogoutTime;
        float offlineSeconds = (float)offlineSpan.TotalSeconds;

        // 3. 12시간(43200초) 초과 시 12시간으로 고정
        if (offlineSeconds > MAX_OFFLINE_SECONDS)
        {
            offlineSeconds = MAX_OFFLINE_SECONDS;
        }

        // 방치 시간이 거의 없다면(1초 미만) 팝업을 띄우지 않고 바로 게임 진행
        if (offlineSeconds < 1f)
        {
            ResumeGame();
            return;
        }

        // 4. 자동 생산 보상 계산 (소수점 버림 처리) 
        int autoRewardGold = m_autoTimer.CalculateOfflineReward(offlineSeconds);

        // 5. 퀘스트 남은 시간 계산 및 완료 처리
        m_questTimer.ApplyOfflineTime(offlineSeconds);

        // 6. 모든 계산이 끝난 후 총 보상 팝업 출력 
        ShowTotalRewardPopup(autoRewardGold, offlineSeconds);
    }

    private void ShowTotalRewardPopup(int earnedGold, float offlineSeconds)
    {
        m_autoTimer.IsPaused = true;

        // 1. 방치한 총 오프라인 시간 출력 (예: 2시간 30분 15초)
        m_totalRewardText.text = $"방치 시간: {FormatTimeKorean(offlineSeconds)}\n\n";

        // 2. 자동 생산 골드 보상 출력
        m_totalRewardText.text += $"자동 생산 수익: {earnedGold} G\n";

        // 3. 퀘스트 남은 시간 출력 (이곳도 포맷팅 함수 적용)
        if (m_questTimer.IsComplete)
        {
            m_totalRewardText.text += "퀘스트 탐험대가 복귀했습니다!";
        }
        else
        {
            m_totalRewardText.text += $"퀘스트 남은 시간: {FormatTimeKorean(m_questTimer.RemainingSeconds)}";
        }

        m_totalRewardPopup.SetActive(true);
    }

    // ✨ 초(Seconds)를 "시간 분 초" 포맷의 문자열로 변환해주는 헬퍼 함수 ✨
    private string FormatTimeKorean(float totalSeconds)
    {
        // 1시간은 3600초. 전체 초를 3600으로 나누어 시간을 구합니다.
        int hours = Mathf.FloorToInt(totalSeconds / 3600f);

        // 전체 초에서 3600으로 나눈 나머지(시간을 빼고 남은 초)를 60으로 나누어 분을 구합니다.
        int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);

        // 전체 초에서 60으로 나눈 나머지(분을 빼고 남은 초)가 순수한 초가 됩니다.
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);

        // 시간이 있으면 "시간 분 초" 모두 출력
        if (hours > 0)
        {
            return $"{hours}시간 {minutes}분 {seconds}초";
        }
        // 시간이 없고 분만 있으면 "분 초" 출력
        else if (minutes > 0)
        {
            return $"{minutes}분 {seconds}초";
        }
        // 1분 미만이면 "초"만 출력
        else
        {
            return $"{seconds}초";
        }
    }

    // 팝업의 '확인' 버튼을 누르면 호출될 함수
    public void OnClickConfirmPopup()
    {
        m_totalRewardPopup.SetActive(false);
        ResumeGame();
    }

    private void ResumeGame()
    {
        // 타이머들 일시정지 해제 및 인게임 로직 다시 시작
        m_autoTimer.IsPaused = false;
        m_questTimer.ResumeTimer();
        Debug.Log("게임 진행!");
    }

    // 게임 종료 또는 창을 닫을 때 시간 저장 
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("LastLogoutTime", DateTime.UtcNow.ToBinary().ToString());
        PlayerPrefs.Save();
    }
}