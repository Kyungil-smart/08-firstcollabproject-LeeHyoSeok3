// ============================================
// 파일명: QuestManager.cs
// 붙일 오브젝트: 빈 오브젝트 (씬에 하나만 존재)
// 역할: 현재 진행 중인 퀘스트 정보를 관리하고,
//       퀘스트 출발/완료 처리를 담당
// ============================================

using UnityEngine;
using System;
using DesignPattern;

public class QuestManager : Singleton<QuestManager>
{
    // 현재 진행 중인 퀘스트 데이터 (null이면 퀘스트 미진행)
    private DungeonData _currentQuest = null;

    //-------------------- 수정 --------------------------
    // 퀘스트 시작 시간(월드맵 재진입 시 현재 진행률 계산하기 위해 필요)
    private DateTime _questStartTime = DateTime.MinValue;
    public DateTime QuestStartTime => _questStartTime;

    // 퀘스트 종료 시각 (현재 시간 + 소요 시간)
    private DateTime _questEndTime = DateTime.MinValue;
    public DateTime QuestEndTime => _questEndTime;

    // -------------------- 수정 --------------------------

    // ★ 추가: 마지막으로 완료한 퀘스트 데이터 (팀장 보상 시스템에서 가져감)
    private DungeonData _completedQuest = null;
    public bool HasCompletedQuest => _completedQuest != null;

    // 프로퍼티 추가
    public DungeonData CompletedQuest => _completedQuest;


    // 퀘스트 진행 중인지 확인하는 프로퍼티
    public bool IsQuestActive => _currentQuest != null;

    // 현재 퀘스트 데이터를 외부에서 읽을 수 있는 프로퍼티
    public DungeonData CurrentQuest => _currentQuest;

    /// <summary>
    /// 저장된 퀘스트가 활성 상태인지 확인하는 프로퍼티
    /// </summary>
    public bool IsQuestSavedActive => _currentQuest != null;
    public string CurrentQuestDungeonName => _currentQuest != null ? _currentQuest.dungeonName : string.Empty;
    public string CompletedQuestDungeonName => _completedQuest != null ? _completedQuest.dungeonName : string.Empty;

    /// <summary>
    /// 퀘스트 출발 처리
    /// QuestItemUI에서 패널 클릭 시 호출됨
    /// </summary>
    public void StartQuest(DungeonData questData)
    {
        if (HasCompletedQuest)
        {
            Debug.Log("[QuestManager] 미수령 보상이 있어 새 퀘스트를 시작할 수 없습니다.");
            PopupManager.Instance?.OpenWarningPopup();
            return;
        }

        // 이미 퀘스트 진행 중이면 출발 불가
        if (IsQuestActive)
        {
            Debug.Log("이미 퀘스트가 진행 중입니다!");
            return;
        }

        // 1. 현재 퀘스트 데이터 저장
        _currentQuest = questData;

        // 2. 소요 시간 문자열을 분(minute)으로 변환해서 종료 시각 계산
        int minutes = ParseTimeToMinutes(questData.timeRequired);
        _questStartTime = DateTime.Now; // 시작 시간 추가
        _questEndTime = DateTime.Now.AddMinutes(minutes);

        Debug.Log($"[QuestManager] 퀘스트 시작\n" +
          $"- 던전: {questData.dungeonName}\n" +
          $"- 시작 시각: {_questStartTime}\n" +
          $"- 종료 시각: {_questEndTime}\n" +
          $"- 총 소요 시간(분): {minutes}");

        // 3. 퀘스트 보드 팝업 닫기
        PopupManager.Instance.CloseAllPopups();

        // 4. 출발 효과음 재생
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OneShot("AdventureStart");
        }

        // 5. 모험 화면(WorldMap)으로 전환
        // if (ScreenStateManager.Instance != null)
        // {
        //     ScreenStateManager.Instance.GoToWorldMap();
        // }

        // ★ 추가: AdventureManager에게 모험 시작 요청
        if (AdventureManager.Instance != null)
        {
            AdventureManager.Instance.StartAdventure(minutes);
        }

        // 6. 월드맵 최소화 컨트롤러에 종료 시각 전달 (남은 시간 표시용)
        WorldMapMinimizedController worldMapMini = FindObjectOfType<WorldMapMinimizedController>();
        if (worldMapMini != null)
        {
            worldMapMini.QuestEndTime = _questEndTime;
        }
    }

    /// <summary>
    /// 퀘스트 완료 처리 (나중에 보상 시스템 연동 시 사용)
    /// </summary>
    public void CompleteQuest()
    {
        if (!IsQuestActive) return;

        Debug.Log($"퀘스트 완료! 던전: {_currentQuest.dungeonName}");

        // ★ 완료된 퀘스트 데이터를 보관 (팀장 보상 시스템에서 가져감)
        _completedQuest = _currentQuest;

        Debug.Log($"[QuestManager] HasCompletedQuest = {HasCompletedQuest}");

        // 진행 중인 퀘스트 초기화
        _currentQuest = null;
        _questStartTime = DateTime.MinValue; // 시작 시간 추가
        _questEndTime = DateTime.MinValue;

        Debug.Log($"[QuestManager] 초기화 완료\n" +
              $"- StartTime: {_questStartTime}\n" +
              $"- EndTime: {_questEndTime}");
    }

    /// <summary>
    /// 보상 수령 완료 시 호출 (팀장 자루 시스템에서 호출)
    /// 완료된 퀘스트 데이터를 초기화
    /// </summary>
    public void ClearCompletedQuest()
    {
        Debug.Log($"보상 수령 완료! 던전: {_completedQuest?.dungeonName}");
        _completedQuest = null;

        Debug.Log($"[QuestManager] HasCompletedQuest = {HasCompletedQuest}");
    }

    /// <summary>
    /// 퀘스트 남은 시간 확인 (0 이하면 완료)
    /// </summary>
    public TimeSpan GetRemainingTime()
    {
        if (!IsQuestActive) return TimeSpan.Zero;

        TimeSpan remaining = _questEndTime - DateTime.Now;
        if (remaining < TimeSpan.Zero) // 예외처리 : 남은 시간이 음수인 경우(이미 종료된 경우) 0으로 반환
        {
            return TimeSpan.Zero;
        }
        return remaining;
        // return _questEndTime - DateTime.Now;
    }

    /// <summary>
    /// 게임 재진입 시 오프라인으로 진행된 퀘스트 시간을 반영하는 함수
    /// </summary>
    /// <param name="currentQuest"></param>
    /// <param name="questStartTime"></param>
    /// <param name="questEndTime"></param>
    /// <param name="completedQuest"></param>
    public void LoadQuestState(
    DungeonData currentQuest,
    DateTime questStartTime,
    DateTime questEndTime,
    DungeonData completedQuest)
    {
        _currentQuest = currentQuest;
        _questStartTime = questStartTime;
        _questEndTime = questEndTime;
        _completedQuest = completedQuest;

        Debug.Log($"[QuestManager] LoadQuestState 완료\n" +
                  $"- CurrentQuest: {_currentQuest?.dungeonName}\n" +
                  $"- StartTime: {_questStartTime}\n" +
                  $"- EndTime: {_questEndTime}\n" +
                  $"- CompletedQuest: {_completedQuest?.dungeonName}");
    }

    public bool ApplyOfflineQuestProgress()
    {
        if (!IsQuestActive) return false;
        if (_questEndTime == DateTime.MinValue) return false;

        if (DateTime.Now >= _questEndTime)
        {
            Debug.Log("[QuestManager] 오프라인 복귀 시 퀘스트 완료 처리");
            CompleteQuest();
            return true;
        }

        return false;
    }

    private void Update()
    {
        // // 퀘스트 진행 중이고, 종료 시각이 지났으면 자동 완료
        // if (IsQuestActive && DateTime.Now >= _questEndTime)
        // {
        //     CompleteQuest();
        // }
    }

    /// <summary>
    /// 소요 시간 문자열을 분(int)으로 변환
    /// "10" (숫자만) / "10분" / "1시간" 모두 처리 가능
    /// </summary>
    private int ParseTimeToMinutes(string timeText)
    {
        // 공백, 보이지 않는 문자(\r, \n 등) 제거
        string text = timeText.Trim();

        // "N시간" 형식 처리
        if (text.Contains("시간"))
        {
            string numberPart = text.Replace("시간", "").Trim();
            if (int.TryParse(numberPart, out int hours))
            {
                return hours * 60;
            }
        }

        // "N분" 형식 처리
        if (text.Contains("분"))
        {
            string numberPart = text.Replace("분", "").Trim();
            if (int.TryParse(numberPart, out int minutes))
            {
                return minutes;
            }
        }

        // ★ 추가: 숫자만 들어온 경우 (예: "10", "20")
        // CSV에서 단위 없이 숫자만 저장된 경우를 처리
        if (int.TryParse(text, out int rawMinutes))
        {
            return rawMinutes;
        }

        // 위 세 가지 모두 실패 시 기본값 10분
        Debug.LogWarning($"소요 시간 파싱 실패: {timeText}, 기본값 10분 사용");
        return 10;
    }
}