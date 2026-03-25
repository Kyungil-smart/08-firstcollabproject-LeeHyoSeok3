using System;
using UnityEngine;
using TMPro;

public class QuestTimerManager : MonoBehaviour
{
    // 최대 오프라인 보상 인정 시간 (12시간 = 43200초)
    private const float MAX_OFFLINE_SECONDS = 43200f;

    [Header("UI Components")]
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private GameObject _rewardReadyUI; // 보상 획득 대기 UI (버튼)

    private float _remainingTime;
    private bool _isTimerActive;

    private void Start()
    {
        // 방치형 위젯 게임의 필수 설정: 포커스를 잃어도 게임 루프 유지
        Application.runInBackground = true;

        // 초기 UI 상태 세팅
        if (_rewardReadyUI != null)
        {
            _rewardReadyUI.SetActive(false);
        }
    }

    private void Update()
    {
        // 인게임 타이머 기본 동작
        if (!_isTimerActive) return;

        _remainingTime -= Time.deltaTime;

        if (_remainingTime <= 0f)
        {
            CompleteQuestOnline();
        }
        else
        {
            UpdateTimerUI();
        }
    }

    /// <summary>
    /// 퀘스트 타이머를 초기화하고 동작을 시작합니다.
    /// </summary>
    /// <param name="timerQuest">데이터 테이블에서 참조한 퀘스트 소요 시간(초)</param>
    public void StartQuest(float timerQuest)
    {
        _remainingTime = timerQuest;
        _isTimerActive = true;

        if (_rewardReadyUI != null) _rewardReadyUI.SetActive(false);

        UpdateTimerUI();
    }

    /// <summary>
    /// 게임 재접속 시 오프라인 경과 시간을 계산하여 퀘스트 타이머에 적용합니다.
    /// 기획서에 따라 최대 12시간까지만 적용되며, 0초 이하 도달 시 완료 상태로 대기합니다.
    /// </summary>
    /// <param name="lastLogoutTime">로컬에 저장되어 있던 마지막 게임 종료 시간</param>
    /// <returns>오프라인 적용 후 퀘스트가 완료 상태(0초)가 되었는지 여부를 반환 (true면 완료)</returns>
    public bool ApplyOfflineTime(DateTime lastLogoutTime)
    {
        TimeSpan offlineSpan = DateTime.Now - lastLogoutTime;
        float offlineSeconds = (float)offlineSpan.TotalSeconds;

        // 보상 책정 기준 시간 12시간(43200초) 초과 시 조정
        if (offlineSeconds > MAX_OFFLINE_SECONDS)
        {
            offlineSeconds = MAX_OFFLINE_SECONDS;
        }

        if (_isTimerActive)
        {
            _remainingTime -= offlineSeconds;

            if (_remainingTime <= 0f)
            {
                // 오프라인에서 퀘스트가 완료된 경우: 음수가 아닌 0초로 고정 후 일시정지 상태 대기
                _remainingTime = 0f;
                _isTimerActive = false;
                UpdateTimerUI();

                // 주의: 오프라인 보상은 자동 생산 완료 확인 후 '총 보상 팝업'으로 띄워야 하므로, 
                // 여기서는 UI를 즉시 띄우지 않고 상태만 true로 반환합니다.
                return true;
            }

            // 0초가 아니라면 갱신된 시간 유지
            UpdateTimerUI();
        }
        return false;
    }

    private void CompleteQuestOnline()
    {
        _remainingTime = 0f;
        _isTimerActive = false;
        UpdateTimerUI();

        // 인게임 중 완료 시 즉시 보상 획득 UI 출력
        if (_rewardReadyUI != null)
        {
            _rewardReadyUI.SetActive(true);
        }
    }

    private void UpdateTimerUI()
    {
        if (_timerText != null)
        {
            // 소수점을 제외한 정수 형태로 출력 (올림 처리하여 0.1초 남아도 1초로 보이게 함)
            _timerText.text = Mathf.CeilToInt(_remainingTime).ToString();
        }
    }
}