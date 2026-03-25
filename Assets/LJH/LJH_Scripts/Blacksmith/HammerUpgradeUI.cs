using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnClickUpgrade);

        RefreshUI();
    }

    public void RefreshUI()
    {
        UpgradeRow current = upgradeSystem.CurrentRow;

        if (current == null)
        {
            Debug.LogError($"{gameObject.name} CurrentRow is null.");
            return;
        }

        levelText.text = $"{levelLabel} : {current.level}";
        valueText.text = $"{valueLabel} : {current.value}";
        stageText.text = $"{stageLabel} : {current.stageDisplay}";

        int gold = upgradeSystem.CurrentGold;
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
        upgradeSystem.TryUpgrade();
        RefreshUI();
    }
}