using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnvilGoldController : MonoBehaviour, IPointerClickHandler
{
    [Header("Upgrade Systems")]
    [SerializeField] private UpgradeSystem clickUpgradeSystem;
    [SerializeField] private UpgradeSystem autoUpgradeSystem;

    [Header("Optional UI Refresh")]
    [SerializeField] private UpgradeUI clickUpgradeUI;
    [SerializeField] private UpgradeUI autoUpgradeUI;

    [Header("Gold UI")]
    [SerializeField] private TMP_Text goldText;

    [Header("Auto Click")]
    [SerializeField] private float autoGainInterval = 1f;

    [Header("Floating Text")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private RectTransform floatingParent;
    [SerializeField] private int maxFloatingText = 10;

    private int sharedGold = 0;
    private float autoTimer;
    private Queue<GameObject> floatingQueue = new Queue<GameObject>();

    private RectTransform anvilRect;

    private void Awake()
    {
        anvilRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        sharedGold = 0;
        ApplyGoldToSystems();
        RefreshAllUI();
    }

    private void Update()
    {
        HandleAutoGain();
        DetectSpendFromUpgradeSystem();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GainByClick();
    }

    public void GainByClick()
    {
        if (clickUpgradeSystem == null)
            return;

        int amount = clickUpgradeSystem.CurrentValue;
        AddGold(amount);
        SpawnFloatingText(amount);
    }

    private void HandleAutoGain()
    {
        if (autoUpgradeSystem == null)
            return;

        autoTimer += Time.deltaTime;

        if (autoTimer < autoGainInterval)
            return;

        autoTimer -= autoGainInterval;

        int amount = autoUpgradeSystem.CurrentValue;

        AddGold(amount); 
    }

    private void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        sharedGold += amount;
        ApplyGoldToSystems();
        RefreshAllUI();
    }

    private void DetectSpendFromUpgradeSystem()
    {
        if (clickUpgradeSystem == null || autoUpgradeSystem == null)
            return;

        int clickGold = clickUpgradeSystem.CurrentGold;
        int autoGold = autoUpgradeSystem.CurrentGold;

        if (clickGold == sharedGold && autoGold == sharedGold)
            return;

        int newSharedGold = Mathf.Min(clickGold, autoGold);

        if (newSharedGold != sharedGold)
        {
            sharedGold = Mathf.Max(0, newSharedGold);
            ApplyGoldToSystems();
            RefreshAllUI();
        }
        else
        {
            ApplyGoldToSystems();
            RefreshAllUI();
        }
    }

    private void ApplyGoldToSystems()
    {
        if (clickUpgradeSystem != null)
            clickUpgradeSystem.SetGold(sharedGold);

        if (autoUpgradeSystem != null)
            autoUpgradeSystem.SetGold(sharedGold);
    }

    private void RefreshAllUI()
    {
        RefreshGoldText();

        if (clickUpgradeUI != null)
            clickUpgradeUI.RefreshUI();

        if (autoUpgradeUI != null)
            autoUpgradeUI.RefreshUI();
    }

    private void RefreshGoldText()
    {
        if (goldText != null)
            goldText.text = sharedGold.ToString("N0");
    }

    private void SpawnFloatingText(int amount)
    {
        if (floatingTextPrefab == null || floatingParent == null)
            return;

        GameObject obj = Instantiate(floatingTextPrefab, floatingParent);
        obj.transform.SetAsLastSibling();

        RectTransform textRect = obj.GetComponent<RectTransform>();
        if (textRect != null)
        {
            textRect.anchoredPosition = Vector2.zero;
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
}