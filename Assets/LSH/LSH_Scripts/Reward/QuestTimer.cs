using System;
using UnityEngine;

public class QuestTimer
{
    private float m_timeRemaining;
    private bool m_isComplete;
    private bool m_isPaused;

    // 기획서 기준 소수점을 제외한 정수 형태로 출력하기 위한 프로퍼티
    public int RemainingSeconds
    {
        get { return Mathf.CeilToInt(m_timeRemaining); }
    }

    public bool IsComplete
    {
        get { return m_isComplete; }
    }

    public QuestTimer(float duration)
    {
        m_timeRemaining = duration;
        m_isComplete = false;
        m_isPaused = false;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (m_isComplete || m_isPaused)
        {
            return;
        }

        m_timeRemaining -= deltaTime;

        if (m_timeRemaining <= 0f)
        {
            CompleteQuest();
        }
    }

    public void ApplyOfflineTime(float offlineSeconds)
    {
        if (m_isComplete)
        {
            return;
        }

        // 퀘스트 타이머 남은 시간에서 오프라인 진행 시간을 차감합니다.
        m_timeRemaining -= offlineSeconds;

        // 계산 결과가 0 이하이면 음수가 아닌 0초로 판정합니다.
        if (m_timeRemaining <= 0f)
        {
            m_timeRemaining = 0f;
            m_isComplete = true;
            m_isPaused = true;
        }
        else
        {
            // 아니라면 산출된 값에 따라 남은 시간을 갱신 후 일시정지를 유지합니다.
            m_isPaused = true;
        }
    }

    public void ResumeTimer()
    {
        m_isPaused = false;
    }

    private void CompleteQuest()
    {
        m_timeRemaining = 0f;
        m_isComplete = true;
        m_isPaused = false;
    }
}