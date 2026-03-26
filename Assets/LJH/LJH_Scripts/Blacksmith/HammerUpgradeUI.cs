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

    private void Start()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnClickUpgrade);

        StartCoroutine(BindGoldManager());
        RefreshUI();
    }

    private void OnDisable()
    {
        if (isSubscribed && GoldManager.Instance != null)
        {
            GoldManager.Instance.OnGoldChanged -= OnGoldChanged;
            isSubscribed = false;
        }
    }

    private IEnumerator BindGoldManager()
    {
        while (GoldManager.Instance == null)
            yield return null;

        GoldManager.Instance.OnGoldChanged += OnGoldChanged;
        isSubscribed = true;

        RefreshUI();
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
        if (current == null)
            return;

        levelText.text = $"{levelLabel} : {current.level}";
        valueText.text = $"{valueLabel} : {current.value}";
        stageText.text = $"{stageLabel} : {current.stageDisplay}";

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