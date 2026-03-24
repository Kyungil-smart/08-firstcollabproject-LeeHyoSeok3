using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    [Header("CSV")]
    [SerializeField] private TextAsset csvFile;
    [SerializeField] private int startColumnIndex = 0;

    [Header("Runtime Data")]
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private int currentGold = 100000;

    private List<UpgradeRow> rows;

    public int CurrentLevel => currentLevel;
    public int CurrentGold => currentGold;

    public UpgradeRow CurrentRow
    {
        get
        {
            if (rows == null || rows.Count == 0)
                return null;

            if (currentLevel < 0 || currentLevel >= rows.Count)
                return null;

            return rows[currentLevel];
        }
    }

    private void Awake()
    {
        rows = UpgradeCSVLoader.Load(csvFile, startColumnIndex);

        if (rows == null || rows.Count == 0)
        {
            Debug.LogError($"{gameObject.name} failed to load upgrade rows.");
        }
    }

    public bool IsMaxLevel()
    {
        return rows == null || rows.Count == 0 || currentLevel >= rows.Count - 1;
    }

    public int CurrentValue => CurrentRow != null ? CurrentRow.value : 0;
    public int CurrentStageDisplay => CurrentRow != null ? CurrentRow.stageDisplay : 0;
    public int CurrentUpgradeCost => CurrentRow != null ? CurrentRow.cost : 0;

    public bool CanUpgrade()
    {
        if (IsMaxLevel())
            return false;

        return currentGold >= CurrentUpgradeCost;
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade())
            return false;

        currentGold -= CurrentUpgradeCost;
        currentLevel++;
        return true;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
    }
}