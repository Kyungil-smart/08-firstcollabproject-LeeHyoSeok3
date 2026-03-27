using UnityEngine;

public class AutoProductionTimer
{
    private float m_maxTime;      // Timer_object 컬럼 참조값 (예: 5초)
    private float m_currentTime;  // 현재 타이머 시간
    private int m_rewardAmount;   // 1회 완료 시 얻는 재화량

    public bool IsPaused { get; set; } // 오프라인 정산 중 일시정지를 위한 속성

    // 생성자: 오브젝트 구입 시 초기값 설정
    public AutoProductionTimer(float maxTime, int rewardAmount)
    {
        m_maxTime = maxTime;
        m_rewardAmount = rewardAmount;
        m_currentTime = 0f;
        IsPaused = false;
    }

    // 인게임 매 프레임 업데이트
    public int UpdateTimer(float deltaTime)
    {
        if (IsPaused) return 0;

        m_currentTime += deltaTime;

        // 목표 시간에 도달했을 때 (타이머 업)
        if (m_currentTime >= m_maxTime)
        {
            m_currentTime = 0f; // 0초로 초기화
            return m_rewardAmount; // 획득할 보상량 반환
        }

        return 0; // 아직 도달하지 않음
    }

    // 오프라인 방치 시간 한 번에 정산
    public int CalculateOfflineReward(float offlineSeconds)
    {
        // 보상 책정 기준 시간을 타이머 최대값으로 나누기 
        float totalRewardsFloat = offlineSeconds / m_maxTime;

        // 소수점이 포함될 경우 버림 처리 후 정수값만 산출 
        int rewardCount = Mathf.FloorToInt(totalRewardsFloat);

        // 남은 자투리 시간을 현재 시간에 반영 (예: 5초 타이머인데 12초 지났으면, 2번 보상받고 2초부터 다시 시작)
        m_currentTime = offlineSeconds % m_maxTime;

        return rewardCount * m_rewardAmount; // 최종 획득 재화량
    }
}