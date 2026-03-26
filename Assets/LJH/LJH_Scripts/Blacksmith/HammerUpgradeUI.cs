using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    private bool isSubscribed;
    private Coroutine bindCoroutine;

    private void Awake()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnClickUpgrade);
    }

    private void OnEnable()
    {
        RefreshUI();

        if (GoldManager.Instance != null)
        {
            SubscribeGoldEvent();
            RefreshUI();
        }
        else
        {
            bindCoroutine = StartCoroutine(BindGoldManager());
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

    private void OnGoldChanged(int gold)
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
            return;

        // 레벨 표시
        if (next != null)
            levelText.text = $"{levelLabel} : {current.level} → {next.level}";
        else
            levelText.text = $"{levelLabel} : {current.level} (MAX)";

        // 능력치 표시
        if (next != null)
            valueText.text = $"{valueLabel} : {current.value} → {next.value}";
        else
            valueText.text = $"{valueLabel} : {current.value} (MAX)";

        // 스테이지도 동일하게 가능
        if (next != null)
            stageText.text = $"{stageLabel} : {current.stageDisplay} → {next.stageDisplay}";
        else
            stageText.text = $"{stageLabel} : {current.stageDisplay} (MAX)";

        int gold = GoldManager.Instance != null ? GoldManager.Instance.CurrentGold : 0;
        int cost = upgradeSystem.CurrentUpgradeCost;

        if (upgradeSystem.IsMaxLevel())
        {
            goldText.text = $"{gold:N0} / MAX";
            goldText.color = Color.white;
            upgradeButton.interactable = false;
        }
        else
        {
            goldText.text = $"{gold:N0} / {cost:N0}";
            goldText.color = gold >= cost ? Color.white : Color.red;
            upgradeButton.interactable = gold >= cost;
        }
    }

    private void OnClickUpgrade()
    {
        if (upgradeSystem.TryUpgrade())
            RefreshUI();
    }
}