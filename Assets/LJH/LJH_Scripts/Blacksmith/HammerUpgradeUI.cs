using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeSystem upgradeSystem;

    [Header("Labels")]
    [SerializeField] private string levelLabel = "Level";
    [SerializeField] private string valueLabel = "Power";
    [SerializeField] private string stageLabel = "Stage";

    [Header("UI")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private Button upgradeButton;

    [Header("Icon")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private List<Sprite> iconList; // imports 폴더에서 끌어다 넣기

    [Header("Hammer Animation")]
    [SerializeField] private HammerAnimationOverrideController hammerAnimationOverrideController;

    private Dictionary<string, Sprite> iconDict;

    private bool isSubscribed;
    private Coroutine bindCoroutine;

    private void Awake()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnClickUpgrade);

        iconDict = new Dictionary<string, Sprite>();

        foreach (var sprite in iconList)
        {
            if (sprite != null && !iconDict.ContainsKey(sprite.name))
                iconDict.Add(sprite.name, sprite);
        }
    }

    private void Start()
    {
        RefreshUI();
    }

    private void OnEnable()
    {
        if (GoldManager.Instance != null)
            SubscribeGoldEvent();
        else
            bindCoroutine = StartCoroutine(BindGoldManager());

        if (GameDataController.Instance != null && GameDataController.Instance.IsLoaded)
        {
            RefreshUI();
        }
        else
        {
            GameDataController.OnGameLoaded += OnGameLoaded_Handler;
        }
    }

    private void OnDisable()
    {
        if (bindCoroutine != null)
        {
            StopCoroutine(bindCoroutine);
            bindCoroutine = null;
        }

        UnsubscribeGoldEvent();
        GameDataController.OnGameLoaded -= OnGameLoaded_Handler;
    }

    private void OnGameLoaded_Handler()
    {
        GameDataController.OnGameLoaded -= OnGameLoaded_Handler;
        RefreshUI();
    }

    private IEnumerator BindGoldManager()
    {
        while (GoldManager.Instance == null)
            yield return null;

        SubscribeGoldEvent();
        bindCoroutine = null;
        RefreshUI();
    }

    private void SubscribeGoldEvent()
    {
        if (isSubscribed || GoldManager.Instance == null)
            return;

        GoldManager.Instance.OnGoldChanged += OnGoldChanged;
        isSubscribed = true;
    }

    private void UnsubscribeGoldEvent()
    {
        if (!isSubscribed || GoldManager.Instance == null)
            return;

        GoldManager.Instance.OnGoldChanged -= OnGoldChanged;
        isSubscribed = false;
    }

    private void OnGoldChanged(double gold)
    {
        RefreshUI();
    }

    public void ForceRefresh()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (upgradeSystem == null)
            return;

        UpgradeRow current = upgradeSystem.CurrentRow;
        UpgradeRow next = upgradeSystem.NextRow;

        if (current == null)
        {
            if (levelText != null) levelText.text = $"{levelLabel} : -";
            if (valueText != null) valueText.text = $"{valueLabel} : -";
            if (stageText != null) stageText.text = $"{stageLabel} : -";
            if (goldText != null) goldText.text = "- / -";
            if (upgradeButton != null) upgradeButton.interactable = false;

            SetIcon(defaultIcon);
            return;
        }

        if (levelText != null)
        {
            int level = upgradeSystem.CurrentLevel;
            int nextLevel = level + 1;

            levelText.text = next != null
                ? $"{levelLabel} : {level} → {nextLevel}"
                : $"{levelLabel} : {level} (MAX)";
        }

        if (valueText != null)
        {
            valueText.text = next != null
                ? $"{valueLabel} : {upgradeSystem.CurrentValue} → {next.value}"
                : $"{valueLabel} : {upgradeSystem.CurrentValue} (MAX)";
        }

        if (stageText != null)
        {
            stageText.text = next != null
                ? $"{stageLabel} : {current.stageDisplay} → {next.stageDisplay}"
                : $"{stageLabel} : {current.stageDisplay} (MAX)";
        }

        UpdateIcon(current);

        double gold = GoldManager.Instance != null ? GoldManager.Instance.CurrentGold : 0d;
        double cost = upgradeSystem.CurrentUpgradeCost;

        if (upgradeSystem.IsMaxLevel())
        {
            if (goldText != null)
            {
                goldText.text = $"{GoldManager.FormatGold(gold)} / MAX";
                goldText.color = Color.white;
            }

            if (upgradeButton != null)
                upgradeButton.interactable = false;
        }
        else
        {
            if (goldText != null)
            {
                goldText.text = $"{GoldManager.FormatGold(gold)} / {GoldManager.FormatGold(cost)}";
                goldText.color = gold >= cost ? Color.white : Color.red;
            }

            if (upgradeButton != null)
                upgradeButton.interactable = gold >= cost;
        }
    }

    private void UpdateIcon(UpgradeRow row)
    {
        if (iconImage == null)
            return;

        if (row == null || string.IsNullOrEmpty(row.iconKey))
        {
            SetIcon(defaultIcon);
            return;
        }

        if (iconDict.TryGetValue(row.iconKey, out var sprite))
        {
            SetIcon(sprite);
        }
        else
        {
            Debug.LogWarning($"아이콘 없음: {row.iconKey}");
            SetIcon(defaultIcon);
        }
    }

    private void SetIcon(Sprite sprite)
    {
        if (iconImage == null)
            return;

        iconImage.sprite = sprite;
        iconImage.enabled = sprite != null;
    }

    private void OnClickUpgrade()
    {
        if (!upgradeSystem.TryUpgrade())
            return;

        if (hammerAnimationOverrideController != null)
            hammerAnimationOverrideController.ApplyHammerAnimationByLevel();

        RefreshUI();
    }
}