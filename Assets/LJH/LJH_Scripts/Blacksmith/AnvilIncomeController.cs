using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnvilGoldController : MonoBehaviour, IPointerDownHandler
{
    [Header("Button")]
    [SerializeField] private Button anvilButton;
    [SerializeField] private Button minimizedAnvilButton; // 최소화 화면에서 클리커 버튼

    [Header("Upgrade Systems")]
    [SerializeField] private UpgradeSystem clickUpgradeSystem;
    [SerializeField] private UpgradeSystem autoUpgradeSystem;

    [Header("Floating Text")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private RectTransform mainFloatingParent;
    [SerializeField] private RectTransform minimizedFloatingParent;
    [SerializeField] private int maxFloatingText = 10;

    [SerializeField] private TMPro.TextMeshProUGUI gpsText;

    [Header("Animation")]
    [SerializeField] private KkangKkangiAnimationController mainKkangKkangiAnimationController;
    [SerializeField] private KkangKkangiAnimationController minimizedKkangKkangiAnimationController;

    [Header("Roots")]
    [SerializeField] private GameObject mainRoot;
    [SerializeField] private GameObject minimizedRoot;

    private float autoTimer; // Note : 자동 생산을 담당

    private readonly Queue<GameObject> floatingQueue = new Queue<GameObject>();
    private bool isAutoProductionLoopPlaying;

    // ------------------------------------------------------
    // 추가 : 오프라인 보상 계산을 위해 현재 자동 생산 타이머와 초당 골드 값을 외부에서 접근할 수 있도록 프로퍼티 추가
    public float CurrentAutoTimer => autoTimer;
    public int CurrentGoldPerSecond
    {
        get
        {
            if (autoUpgradeSystem == null) return 0;

            return autoUpgradeSystem.CurrentValue;
        }
    }

    public void SetAutoTimer(float value)
    {
        autoTimer = Mathf.Clamp(value, 0f, 1f);
    }
    // ------------------------------------------------------

    private void Start()
    {
        if (anvilButton != null)
            anvilButton.onClick.AddListener(GainByClick);

        if (minimizedAnvilButton != null)
            minimizedAnvilButton.onClick.AddListener(GainByClick);
    }

    private void Update()
    {
        UpdateAutoProductionLoop(); // 자동 생산 사운드 관리
        HandleAutoGain();
        RefreshGPSUI();
    }

    private void OnEnable()
    {
        if (GameDataController.Instance != null && GameDataController.Instance.IsLoaded)
            RefreshGPSUI();
        else
            GameDataController.OnGameLoaded += OnGameLoaded_Handler;
    }

    private void OnDisable()
    {
        GameDataController.OnGameLoaded -= OnGameLoaded_Handler;

        if (isAutoProductionLoopPlaying) // 자동 생산 사운드가 재생 중이라면 정지
        {
            // OnDisable()에서 SoundManager.Instance를 직접 부르지 않고, 
            // FindFirstObjectByType<SoundManager>()로 실제 씬에 남아 있는 매니저가 있을 때만 
            // LoopStop() 하도록 변경 (씬 전환 시 SoundManager가 파괴될 수 있기 때문)
            SoundManager soundManager = FindFirstObjectByType<SoundManager>();
            if (soundManager != null)
            {
                soundManager.LoopStop();
            }
            isAutoProductionLoopPlaying = false;
        }
    }

    private void OnGameLoaded_Handler()
    {
        GameDataController.OnGameLoaded -= OnGameLoaded_Handler;
        RefreshGPSUI();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPointerScreenPosition = eventData.position;
    }

    private Vector2 lastPointerScreenPosition;

    private bool IsMinimizedMode()
    {
        return minimizedRoot != null && minimizedRoot.activeInHierarchy;
    }

    private RectTransform GetFloatingParent(bool isMinimized)
    {
        if (isMinimized) return minimizedFloatingParent;

        return mainFloatingParent;
    }

    // 외부에서 클릭 위치를 설정할 수 있도록 메서드 추가 (최소화 화면에서 작동할 수 있도록 추가)
    public void SetPointerScreenPosition(Vector2 screenPosition)
    {
        lastPointerScreenPosition = screenPosition;
    }

    public void GainByClick()
    {
        if (clickUpgradeSystem == null || GoldManager.Instance == null)
            return;

        SoundManager.Instance?.OneShot("HammerHit");

        int amount = clickUpgradeSystem.CurrentValue;
        GoldManager.Instance.AddGold(amount);

        if (IsMinimizedMode())
        {
            if (minimizedKkangKkangiAnimationController != null)
                minimizedKkangKkangiAnimationController.PlayCraftAnimation();
        }
        else
        {
            if (mainKkangKkangiAnimationController != null)
                mainKkangKkangiAnimationController.PlayCraftAnimation();
        }
        SpawnFloatingTextAtScreenPosition(amount, lastPointerScreenPosition);
    }

    private void HandleAutoGain()
    {
        if (autoUpgradeSystem == null || GoldManager.Instance == null)
            return;

        autoTimer += Time.deltaTime;

        if (autoTimer < 1f)
            return;

        autoTimer -= 1f;

        int amount = autoUpgradeSystem.CurrentValue; // 초당 골드
        GoldManager.Instance.AddGold(amount);
    }

    /// <summary>
    /// 자동 생산 사운드 재생 여부를 관리하는 메서드
    /// 자동 생산이 활성화되면 루프 사운드를 재생하고, 비활성화되면 사운드를 정지하는 로직
    /// </summary>
    private void UpdateAutoProductionLoop()
    {
        if (autoUpgradeSystem == null)
            return;
        // 월드맵에서는 자동 생산 사운드를 재생하지 않도록 추가
        bool isAdventureScreen =
            ScreenStateManager.Instance != null &&
            (ScreenStateManager.Instance.CurrentState == ScreenStateManager.ScreenState.WorldMap ||
             ScreenStateManager.Instance.CurrentState == ScreenStateManager.ScreenState.WorldMapMinimized);

        bool shouldPlayLoop = autoUpgradeSystem.CurrentValue > 0 && !isAdventureScreen;

        if (shouldPlayLoop && !isAutoProductionLoopPlaying)
        {
            SoundManager.Instance?.LoopShot("AutoProductionLoop");
            isAutoProductionLoopPlaying = true;
        }
        else if (!shouldPlayLoop && isAutoProductionLoopPlaying)
        {
            SoundManager.Instance?.LoopStop();
            isAutoProductionLoopPlaying = false;
        }
    }

    private void SpawnFloatingTextAtScreenPosition(int amount, Vector2 screenPosition)
    {
        bool isMinimized = minimizedAnvilButton != null && minimizedAnvilButton.gameObject.activeInHierarchy;
        RectTransform currentFloatingParent = GetFloatingParent(isMinimized);

        if (floatingTextPrefab == null || currentFloatingParent == null)
        {
            return;
        }

        GameObject obj = Instantiate(floatingTextPrefab, currentFloatingParent);
        obj.transform.SetAsLastSibling();

        RectTransform textRect = obj.GetComponent<RectTransform>();
        if (textRect != null)
        {
            Canvas canvas = currentFloatingParent.GetComponentInParent<Canvas>();
            Camera cam = null;

            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = canvas.worldCamera;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                currentFloatingParent,
                screenPosition,
                cam,
                out localPoint
            );

            Vector2 randomOffset = new Vector2(
                Random.Range(-20f, 20f),
                Random.Range(-10f, 10f)
            );

            textRect.anchoredPosition = localPoint + randomOffset;
            textRect.localScale = Vector3.one;
            textRect.localRotation = Quaternion.identity;
        }

        FloatingText floatingText = obj.GetComponent<FloatingText>();
        if (floatingText != null)
            floatingText.SetText(amount);

        floatingQueue.Enqueue(obj);

        if (floatingQueue.Count > maxFloatingText)
        {
            GameObject oldObj = floatingQueue.Dequeue();
            if (oldObj != null)
                Destroy(oldObj);
        }
    }

    private void RefreshGPSUI()
    {
        if (gpsText == null || autoUpgradeSystem == null)
            return;

        float goldPerSecond = autoUpgradeSystem.CurrentValue;

        gpsText.text = $"{goldPerSecond:F0}/s";
    }
}