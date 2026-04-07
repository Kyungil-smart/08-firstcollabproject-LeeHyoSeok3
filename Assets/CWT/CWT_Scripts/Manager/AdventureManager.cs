// ============================================
// 파일명: AdventureManager.cs
// 붙일 오브젝트: 빈 오브젝트 (씬에 하나만 존재)
// 역할: 모험 화면의 파티 이동 + 이벤트 연출을 총괄 관리
//       상태 머신 + 시간 기반 이동 + 비주얼 토글
// ============================================

using UnityEngine;
using DesignPattern;
using System;
using System.Data;
using UnityEngine.UI;

public class AdventureManager : Singleton<AdventureManager>
{
    // ============================================
    // 1. 상태 머신 (enum)
    //    파티가 현재 어떤 상태인지를 나타냄
    //    상태에 따라 스프라이트와 이벤트가 달라짐
    // ============================================
    public enum AdventureState
    {
        None,           // 모험 시작 전 / 종료 후
        Going,          // 우측 → 좌측 이동 중
        Discovering,    // 발견 지점과 겹치는 중
        Camping,        // 야영 지점과 겹치는 중
        Battle,         // 던전과 겹치는 중 (전투)
        Celebrate,      // 좌측 끝 도달 (자축)
        Returning,      // 좌측 → 우측 복귀 중
        Completed       // 모험 완료
    }

    // 현재 상태 (외부에서 읽기만 가능)
    public AdventureState CurrentState { get; private set; } = AdventureState.None;

    // ============================================
    // 2. Inspector에서 연결할 오브젝트들
    // ============================================

    [Header("─── 맵 지점 (PathArea의 자식들) ───")]
    [SerializeField] private RectTransform _startPoint;        // 우측 출발 위치
    [SerializeField] private RectTransform _discoverPoint;     // 발견 지점 (1/3)
    [SerializeField] private RectTransform _campPoint;         // 야영 지점 (2/3)
    [SerializeField] private RectTransform _dungeonPoint;      // 던전 시작
    [SerializeField] private RectTransform _leftEndPoint;      // 좌측 끝
    [SerializeField] private RectTransform _returnEndPoint;    // 복귀 종료 (= 출발 위치)

    [Header("─── 던전 아이콘 (DungeonPoint의 Image) ───")]
    [SerializeField] private Image _dungeonPointIcon;  // DungeonPoint에 붙어있는 Image 컴포넌트

    [Header("─── 파티 그룹 (이동 대상) ───")]
    [SerializeField] private RectTransform _partyGroup;        // 파티 전체를 움직일 RectTransform

    [Header("─── 파티 스프라이트 (4개 중 1개만 활성화) ───")]
    [SerializeField] private GameObject _partyDefault;         // 기본 상태
    [SerializeField] private GameObject _partyBattle;          // 전투 (흙먼지)
    [SerializeField] private GameObject _partyCelebrate;       // 자축 (웃음)
    [SerializeField] private GameObject _partyTent;            // 야영 (텐트)

    [Header("─── 말풍선 (BubbleGroup의 자식들) ───")]
    [SerializeField] private GameObject _bubbleQuestion;       // ? 말풍선
    [SerializeField] private GameObject _bubbleExclamation;    // ! 말풍선
    //[SerializeField] private GameObject _bubbleZzz;            // Zzz 말풍선

    [Header("─── 던전 배경 (DungeonBackground의 Image) ───")]
    [SerializeField] private Image _dungeonBackgroundImage;  // DungeonBackground에 붙어있는 Image

    // --------------------- 수정 : ScreenStateManager에서 화면 전환 담당 --------------------
    // [Header("─── 모험 화면 패널 ───")]
    // [SerializeField] private GameObject _worldMapPanel;        // 모험 화면 전체 패널
    // [SerializeField] private GameObject _mainScreenGroup;      // 메인 화면 UI 그룹

    // 추가 : ScreenStateManager에서 구독하여 화면 전환 처리
    public event Action OnAdventureStarted;   // 모험 시작 이벤트 
    public event Action OnAdventureCompleted; // 모험 완료 이벤트

    // 추가 : 퀘스트가 중복 완료 되는 부분 방어
    private bool _isCompleting;

    // -------------------------------------------------------------------------------------

    [Header("─── 이벤트 감지 범위 ───")]
    [SerializeField] private float _eventRange = 40f;          // 지점과 파티가 이 거리 안이면 "겹침" 판정

    [Header("─── 자축 연출 시간 (초) ───")]
    [SerializeField] private float _celebrateDuration = 2f;    // 좌측 끝 도달 후 기뻐하는 시간

    // ============================================
    // 3. 내부 변수들
    // ============================================

    // 시간 관련
    private float _questDuration;       // 전체 퀘스트 시간 (초 단위)
    private float _goingDuration;       // 가는 시간 (전체의 2/3)
    private float _returningDuration;   // 복귀 시간 (전체의 1/3)
    private float _elapsedTime;         // 현재 상태에서 경과한 시간

    // 위치 관련 (X 좌표)
    private float _startX;             // 출발 X 좌표
    private float _leftEndX;           // 좌측 끝 X 좌표

    // 말풍선 토글 관련
    private float _bubbleTimer;        // 말풍선 ON/OFF 타이머
    private bool _bubbleVisible;       // 현재 말풍선이 보이는지 여부
    private const float BUBBLE_ON_TIME = 2f;   // 말풍선 표시 시간
    private const float BUBBLE_OFF_TIME = 2f;  // 말풍선 숨김 시간

    // 자축 타이머
    private float _celebrateTimer;

    // 발견 말풍선 교대 (? → ! → ? → ! 반복)
    private bool _showQuestion = true; // true면 ? 표시, false면 ! 표시

    // ============================================
    // 4. 초기화
    // ============================================

    /// <summary>
    /// Singleton의 OnAwake() 오버라이드
    /// Awake 시점에 자동 호출됨
    /// </summary>
    protected override void OnAwake()
    {
        // 시작할 때 모험 화면은 비활성화
        // if (_worldMapPanel != null)
        //     _worldMapPanel.SetActive(false);
    }

    // ============================================
    // 5. 모험 시작 (QuestManager에서 호출)
    // ============================================

    /// <summary>
    /// 모험을 시작하는 메서드
    /// QuestManager.StartQuest()에서 호출됨
    /// </summary>
    /// <param name="questMinutes">퀘스트 소요 시간 (분 단위)</param>
    public void StartAdventure(int questMinutes)
    {
        if (CurrentState != AdventureState.None) return;

        _isCompleting = false;

        // 1) 퀘스트 시간을 초 단위로 변환 -> 실제 모험하는 시간을 계산해준다.
        _questDuration = questMinutes * 60f;

        // 테스트용 코드 30초 모험용
        //_questDuration = 30f;

        // 2) 가는 시간 = 전체의 2/3, 복귀 시간 = 전체의 1/3
        float movingDuration = _questDuration - _celebrateDuration;
        _goingDuration = movingDuration * (2f / 3f);
        _returningDuration = movingDuration * (1f / 3f);

        // 3) 시작/끝 X 좌표 저장
        _startX = _startPoint.anchoredPosition.x;
        _leftEndX = _leftEndPoint.anchoredPosition.x;

        // 4) 파티를 출발 위치로 이동
        Vector2 startPos = _partyGroup.anchoredPosition;
        startPos.x = _startX;
        _partyGroup.anchoredPosition = startPos;

        // 5) 경과 시간 초기화
        _elapsedTime = 0f;

        Debug.Log($"[AdventureManager] 던전 아이콘 확인 - " +
                  $"Icon null? {QuestManager.Instance.CurrentQuest.dungeonIcon == null}, " +
                  $"Image null? {_dungeonPointIcon == null}");

        // ★ 추가: 선택한 던전의 아이콘을 DungeonPoint에 표시
        if (_dungeonPointIcon != null && QuestManager.Instance.CurrentQuest != null)
        {
            _dungeonPointIcon.sprite = QuestManager.Instance.CurrentQuest.dungeonIcon;
        }

        // ★ 추가: 선택한 던전의 배경을 DungeonBackground에 표시
        if (_dungeonBackgroundImage != null && QuestManager.Instance.CurrentQuest != null)
        {
            // 모험 시작 시에는 일반 던전 배경
            _dungeonBackgroundImage.sprite = QuestManager.Instance.CurrentQuest.dungeonBackground;
        }

        ApplyCurrentDungeonVisuals(false);

        // 6) 비주얼 초기화 (기본 스프라이트만 켜기)
        SetPartySprite(PartyVisual.Default);
        HideAllBubbles();

        // 화면 전환(테스트용)
        // if (_mainScreenGroup != null)
        //     _mainScreenGroup.SetActive(false);

        // if (_worldMapPanel != null)
        //     _worldMapPanel.SetActive(true);

        // 7) 상태를 Going으로 변경 → Update에서 이동 시작
        CurrentState = AdventureState.Going;

        // 8) 모험 시작 이벤트 호출
        OnAdventureStarted?.Invoke();

        Debug.Log($"[AdventureManager] 모험 시작! 총 시간: {questMinutes}분, " +
                  $"가는 시간: {_goingDuration}초, 복귀 시간: {_returningDuration}초");
    }

    // ============================================
    // 6. Update - 매 프레임 실행되는 핵심 로직
    // ============================================

    private void Update()
    {
        // 모험이 진행 중이 아니면 아무것도 안 함
        if (CurrentState == AdventureState.None ||
            CurrentState == AdventureState.Completed)
            return;

        // 현재 상태에 따라 다른 로직 실행
        switch (CurrentState)
        {
            case AdventureState.Going:
                UpdateGoing();
                break;
            case AdventureState.Discovering:
                UpdateDiscovering();
                break;
            case AdventureState.Camping:
                UpdateCamping();
                break;
            case AdventureState.Battle:
                UpdateBattle();
                break;
            case AdventureState.Celebrate:
                UpdateCelebrate();
                break;
            case AdventureState.Returning:
                UpdateReturning();
                break;
        }
    }

    // ============================================
    // 7. 각 상태별 Update 로직
    // ============================================

    /// <summary>
    /// Going 상태: 우측 → 좌측으로 이동
    /// 이동 중에 발견/야영/던전 지점 감지
    /// </summary>
    private void UpdateGoing()
    {
        // 시간 누적
        _elapsedTime += Time.deltaTime;

        // 진행률 계산 (0.0 ~ 1.0)
        // 0.0 = 출발, 1.0 = 좌측 끝 도달
        float progress = Mathf.Clamp01(_elapsedTime / _goingDuration);

        // 진행률에 따라 파티 X 좌표 계산
        // Lerp(시작X, 끝X, 진행률) → 시작에서 끝까지 부드럽게 이동
        float currentX = Mathf.Lerp(_startX, _leftEndX, progress);
        MovePartyTo(currentX);

        // ─── 위치 기반 이벤트 감지 ───

        // 발견 지점과 겹치면 → Discovering 상태로 전환
        if (IsNearPoint(_discoverPoint))
        {
            EnterDiscovering();
            return; // 상태 전환했으므로 여기서 멈춤
        }

        // 야영 지점과 겹치면 → Camping 상태로 전환
        if (IsNearPoint(_campPoint))
        {
            EnterCamping();
            return;
        }

        // 던전 지점과 겹치면 → Battle 상태로 전환
        if (IsNearPoint(_dungeonPoint))
        {
            EnterBattle();
            return;
        }

        // 좌측 끝에 도달하면 → Celebrate 상태로 전환
        if (progress >= 1.0f)
        {
            EnterCelebrate();
        }
    }

    /// <summary>
    /// Discovering 상태: 발견 지점과 겹치는 동안
    /// ? / ! 말풍선을 2초 간격으로 토글
    /// </summary>
    private void UpdateDiscovering()
    {
        // 이동은 계속 진행
        _elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(_elapsedTime / _goingDuration);
        float currentX = Mathf.Lerp(_startX, _leftEndX, progress);
        MovePartyTo(currentX);

        // 말풍선 토글
        UpdateDiscoverBubble();

        // 발견 지점에서 벗어나면 → Going으로 복귀
        if (!IsNearPoint(_discoverPoint))
        {
            ExitDiscovering();
        }
    }

    /// <summary>
    /// Camping 상태: 야영 지점과 겹치는 동안
    /// 파티 → 텐트 스프라이트, Zzz 말풍선 토글
    /// </summary>
    private void UpdateCamping()
    {
        _elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(_elapsedTime / _goingDuration);
        float currentX = Mathf.Lerp(_startX, _leftEndX, progress);
        MovePartyTo(currentX);

        // Zzz 말풍선은 애니메이션으로 대체 예정

        if (!IsNearPoint(_campPoint))
        {
            ExitCamping();
        }
    }

    /// <summary>
    /// Battle 상태: 던전과 겹치는 동안
    /// 전투 스프라이트(흙먼지) 표시
    /// </summary>
    private void UpdateBattle()
    {
        // 이동은 계속 진행
        _elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(_elapsedTime / _goingDuration);
        float currentX = Mathf.Lerp(_startX, _leftEndX, progress);
        MovePartyTo(currentX);

        // 좌측 끝에 도달하면 → Celebrate로 전환
        if (progress >= 1.0f)
        {
            EnterCelebrate();
        }
    }

    /// <summary>
    /// Celebrate 상태: 좌측 끝에서 기뻐하는 연출
    /// 일정 시간 후 방향 전환 → Returning
    /// </summary>
    private void UpdateCelebrate()
    {
        _celebrateTimer += Time.deltaTime;

        // 자축 시간이 끝나면 → 복귀 시작
        if (_celebrateTimer >= _celebrateDuration)
        {
            EnterReturning();
        }
    }

    /// <summary>
    /// Returning 상태: 좌측 → 우측으로 복귀
    /// 복귀 중에는 발견/야영 이벤트 비활성화
    /// 웃음 스프라이트 유지
    /// </summary>
    private void UpdateReturning()
    {
        // 시간 누적 (복귀용 별도 타이머)
        _elapsedTime += Time.deltaTime;

        // 복귀 진행률 (0.0 = 좌측 끝, 1.0 = 우측 도착)
        float progress = Mathf.Clamp01(_elapsedTime / _returningDuration);

        // 좌측 끝 → 출발 위치로 이동 (반대 방향)
        float currentX = Mathf.Lerp(_leftEndX, _startX, progress);
        MovePartyTo(currentX);

        // 우측 끝에 도달하면 → 모험 완료
        if (progress >= 1.0f)
        {
            CompleteAdventure();
        }
    }

    // ============================================
    // 8. 상태 진입/퇴장 메서드
    // ============================================

    private void EnterDiscovering()
    {
        CurrentState = AdventureState.Discovering;
        _bubbleTimer = 0f;
        _bubbleVisible = true;
        _showQuestion = true; // ? 부터 시작

        Debug.Log("[AdventureManager] 발견 지점 진입!");
    }

    private void ExitDiscovering()
    {
        CurrentState = AdventureState.Going;
        HideAllBubbles();
        SetPartySprite(PartyVisual.Default);

        Debug.Log("[AdventureManager] 발견 지점 벗어남");
    }

    private void EnterCamping()
    {
        CurrentState = AdventureState.Camping;

        // ★ v3 변경: 파티 스프라이트 전부 숨기기 (파티는 안 보이지만 계속 이동)
        HideAllPartySprites();

        // ★ v3 변경: 텐트를 CampPoint 위치에 고정 출력
        _partyTent.SetActive(true);

        Debug.Log("[AdventureManager] 야영 지점 진입! 파티 숨김 + 텐트 출력");
    }

    private void ExitCamping()
    {
        CurrentState = AdventureState.Going;

        // ★ v3 변경: 텐트 숨기기
        _partyTent.SetActive(false);

        // ★ v3 변경: 파티 스프라이트 다시 보이기
        SetPartySprite(PartyVisual.Default);
        HideAllBubbles();

        Debug.Log("[AdventureManager] 야영 지점 벗어남! 텐트 숨김 + 파티 복구");
    }

    private void EnterBattle()
    {
        CurrentState = AdventureState.Battle;
        SetPartySprite(PartyVisual.Battle);

        ApplyCurrentDungeonVisuals(true);

        Debug.Log("[AdventureManager] 던전 진입! 전투 시작!");
    }

    private void EnterCelebrate()
    {
        CurrentState = AdventureState.Celebrate;
        SetPartySprite(PartyVisual.Celebrate); // 웃음 스프라이트
        _celebrateTimer = 0f;

        // 파티 방향 반전 (좌→우로 바뀌니까)
        Vector3 scale = _partyGroup.localScale;
        scale.x = -Mathf.Abs(scale.x); // X 스케일을 반전
        _partyGroup.localScale = scale;

        Debug.Log("[AdventureManager] 좌측 끝 도달! 자축!");
    }

    private void EnterReturning()
    {
        CurrentState = AdventureState.Returning;
        _elapsedTime = 0f; // 복귀용 타이머 초기화
        // 웃음 스프라이트 유지 (기획서: 던전 떠나도 웃음 상태 유지)

        Debug.Log("[AdventureManager] 복귀 시작!");
    }

    /// <summary>
    /// 모험 완료 처리
    /// </summary>
    private void CompleteAdventure()
    {
        if (_isCompleting || CurrentState == AdventureState.None) return;

        _isCompleting = true;
        SoundManager.Instance?.OneShot("AdventureComplete");

        CurrentState = AdventureState.Completed;

        // 파티 스케일 원상복구
        Vector3 scale = _partyGroup.localScale;
        scale.x = Mathf.Abs(scale.x);
        _partyGroup.localScale = scale;

        // 파티 스프라이트 전부 숨기기
        HideAllPartySprites();
        HideAllBubbles();

        // 모험 완료 이벤트 호출
        OnAdventureCompleted?.Invoke();

        // 모험 화면 끄고 메인 화면 켜기
        // if (_worldMapPanel != null)
        //     _worldMapPanel.SetActive(false);

        // if (_mainScreenGroup != null)
        //     _mainScreenGroup.SetActive(true);

        // QuestManager에게 완료 알림
        if (QuestManager.Instance != null)
            QuestManager.Instance.CompleteQuest();

        Debug.Log("[AdventureManager] 모험 완료! 메인 화면으로 복귀");

        // 상태 초기화
        CurrentState = AdventureState.None;
    }

    // ============================================
    // 9. 유틸리티 메서드
    // ============================================

    /// <summary>
    /// 파티의 X 좌표를 변경하는 메서드
    /// Y 좌표는 유지하고 X만 바꿈
    /// </summary>
    private void MovePartyTo(float x)
    {
        Vector2 pos = _partyGroup.anchoredPosition;
        pos.x = x;
        _partyGroup.anchoredPosition = pos;
    }

    /// <summary>
    /// 파티가 특정 지점 근처에 있는지 확인
    /// X 좌표 차이가 _eventRange 이내이면 true
    /// </summary>
    private bool IsNearPoint(RectTransform point)
    {
        float partyX = _partyGroup.anchoredPosition.x;
        float pointX = point.anchoredPosition.x;
        return Mathf.Abs(partyX - pointX) <= _eventRange;
    }

    // ============================================
    // 10. 비주얼 (스프라이트) 관리
    // ============================================

    // 파티 비주얼 종류
    private enum PartyVisual
    {
        Default,
        Battle,
        Celebrate,
        Tent
    }

    /// <summary>
    /// 파티 스프라이트를 전환하는 메서드
    /// 4개 중 1개만 활성화, 나머지는 비활성화
    /// </summary>
    private void SetPartySprite(PartyVisual visual)
    {
        _partyDefault.SetActive(visual == PartyVisual.Default);
        _partyBattle.SetActive(visual == PartyVisual.Battle);
        _partyCelebrate.SetActive(visual == PartyVisual.Celebrate);
    }

    /// <summary>
    /// 파티 스프라이트 전부 숨기기 (모험 완료 시)
    /// </summary>
    private void HideAllPartySprites()
    {
        _partyDefault.SetActive(false);
        _partyBattle.SetActive(false);
        _partyCelebrate.SetActive(false);
    }

    // ============================================
    // 11. 말풍선 시스템
    // ============================================

    /// <summary>
    /// 발견 지점 말풍선 토글 (? → ! → ? → ! 반복)
    /// 2초 표시 → 2초 숨김 → 반복
    /// </summary>
    private void UpdateDiscoverBubble()
    {
        _bubbleTimer += Time.deltaTime;

        if (_bubbleVisible)
        {
            // 현재 보이는 상태 → ON 시간이 지나면 숨김
            if (_bubbleTimer >= BUBBLE_ON_TIME)
            {
                _bubbleTimer = 0f;
                _bubbleVisible = false;
                HideAllBubbles();
            }
            else
            {
                // ? 또는 ! 중 하나만 표시
                _bubbleQuestion.SetActive(_showQuestion);
                _bubbleExclamation.SetActive(!_showQuestion);
                //_bubbleZzz.SetActive(false);
            }
        }
        else
        {
            // 현재 숨김 상태 → OFF 시간이 지나면 다시 표시
            if (_bubbleTimer >= BUBBLE_OFF_TIME)
            {
                _bubbleTimer = 0f;
                _bubbleVisible = true;
                _showQuestion = !_showQuestion; // ? ↔ ! 교대
            }
        }
    }

    /// <summary>
    /// 모든 말풍선 숨기기
    /// </summary>
    private void HideAllBubbles()
    {
        if (_bubbleQuestion != null) _bubbleQuestion.SetActive(false);
        if (_bubbleExclamation != null) _bubbleExclamation.SetActive(false);
        //if (_bubbleZzz != null) _bubbleZzz.SetActive(false);
    }

#region SyncSystem
    // ============================================
    // 12. 동기화 시스템
    // ============================================
    public void SyncFromQuestTime(DateTime questStartTime, DateTime questEndTime)
    {
        if (questStartTime == DateTime.MinValue || questEndTime == DateTime.MinValue)
        {
            Debug.LogWarning("[AdventureManager] 퀘스트 시간이 동기화 되지 않았습니다.");
            return;
        }

        // 전체 퀘스트 시간(초)
        _questDuration = (float)(questEndTime - questStartTime).TotalSeconds;

        if (_questDuration <= 0f) return;

        float movingDuration = _questDuration - _celebrateDuration;
        _goingDuration = movingDuration * (2f / 3f);
        _returningDuration = movingDuration * (1f / 3f);

        _startX = _startPoint.anchoredPosition.x;
        _leftEndX = _leftEndPoint.anchoredPosition.x;

        float elapsedSinceStart = (float)(DateTime.Now - questStartTime).TotalSeconds;
        elapsedSinceStart = Mathf.Clamp(elapsedSinceStart, 0f, _questDuration);

        ApplyCurrentDungeonVisuals(false);
        ApplyProgressByElapsedTime(elapsedSinceStart);

        Debug.Log($"[AdventureManager] Sync 완료 / elapsed = {elapsedSinceStart:F1}s / total = {_questDuration:F1}s / state = {CurrentState}");

    }

    private void ApplyProgressByElapsedTime(float elapsedSinceStart)
    {
        HideAllBubbles();

        // 1) Going 구간
        if (elapsedSinceStart < _goingDuration)
        {
            float progress = Mathf.Clamp01(elapsedSinceStart / _goingDuration);
            float currentX = Mathf.Lerp(_startX, _leftEndX, progress);
            MovePartyTo(currentX);

            _elapsedTime = elapsedSinceStart;

            // 기본은 Going
            CurrentState = AdventureState.Going;
            SetPartySprite(PartyVisual.Default);

            // 현재 위치 기준으로 이벤트 상태 덮어쓰기
            if (IsNearPoint(_discoverPoint))
            {
                CurrentState = AdventureState.Discovering;
                _bubbleTimer = 0f;
                _bubbleVisible = true;
                _showQuestion = true;
            }
            else if (IsNearPoint(_campPoint))
            {
                CurrentState = AdventureState.Camping;
                HideAllPartySprites();
                _partyTent.SetActive(true);
            }
            else if (IsNearPoint(_dungeonPoint))
            {
                CurrentState = AdventureState.Battle;
                SetPartySprite(PartyVisual.Battle);
                ApplyCurrentDungeonVisuals(true);
            }

            // 진행 방향: 출발 → 좌측
            Vector3 scale = _partyGroup.localScale;
            scale.x = Mathf.Abs(scale.x);
            _partyGroup.localScale = scale;

            return;
        }

        // 2) Celebrate 구간
        if (elapsedSinceStart < _goingDuration + _celebrateDuration)
        {
            MovePartyTo(_leftEndX);

            CurrentState = AdventureState.Celebrate;
            SetPartySprite(PartyVisual.Celebrate);

            _celebrateTimer = elapsedSinceStart - _goingDuration;

            // 복귀 준비 방향(좌 -> 우)
            Vector3 scale = _partyGroup.localScale;
            scale.x = -Mathf.Abs(scale.x);
            _partyGroup.localScale = scale;

            return;
        }

        // 3) Returning 구간
        if (elapsedSinceStart < _questDuration)
        {
            float returningElapsed = elapsedSinceStart - (_goingDuration + _celebrateDuration);
            float progress = Mathf.Clamp01(returningElapsed / _returningDuration);
            float currentX = Mathf.Lerp(_leftEndX, _startX, progress);
            MovePartyTo(currentX);

            CurrentState = AdventureState.Returning;
            SetPartySprite(PartyVisual.Celebrate);
            _elapsedTime = returningElapsed;

            Vector3 scale = _partyGroup.localScale;
            scale.x = -Mathf.Abs(scale.x);
            _partyGroup.localScale = scale;

            return;
        }

        // 4) 완전 종료 구간
        CurrentState = AdventureState.Completed;
        MovePartyTo(_startX);
        HideAllPartySprites();
        HideAllBubbles();

        Vector3 resetScale = _partyGroup.localScale;
        resetScale.x = Mathf.Abs(resetScale.x);
        _partyGroup.localScale = resetScale;
    }

    private void ApplyCurrentDungeonVisuals(bool useBattleBackground)
    {
        if (QuestManager.Instance == null || QuestManager.Instance.CurrentQuest == null)
            return;

        DungeonData currentQuest = QuestManager.Instance.CurrentQuest;

        if (_dungeonPointIcon != null)
        {
            _dungeonPointIcon.sprite = currentQuest.dungeonIcon;
            _dungeonPointIcon.enabled = currentQuest.dungeonIcon != null;
        }

        if (_dungeonBackgroundImage != null)
        {
            Sprite background = currentQuest.dungeonBackground;

            if (useBattleBackground && currentQuest.dungeonBattleBackground != null)
                background = currentQuest.dungeonBattleBackground;

            _dungeonBackgroundImage.sprite = background;
            _dungeonBackgroundImage.enabled = background != null;
        }
    }
#endregion
}
