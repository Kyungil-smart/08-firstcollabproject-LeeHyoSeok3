using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    [Header("Save")]
    [SerializeField] private string saveId;

    [Header("CSV")]
    [SerializeField] private TextAsset csvFile;
    [SerializeField] private int startColumnIndex = 0;

    private int currentLevel = 0;
    
    [SerializeField] private bool forceMinimumOne = false;
    
    private List<UpgradeRow> rows;

    public string SaveId => saveId;
    public int CurrentLevel => currentLevel;

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

    public UpgradeRow NextRow
    {
        get
        {
            if (rows == null || rows.Count == 0)
                return null;

            int nextIndex = currentLevel + 1;

            if (nextIndex < 0 || nextIndex >= rows.Count)
                return null;

            return rows[nextIndex];
        }
    }

    private void Awake()
    {
        rows = UpgradeCSVLoader.Load(csvFile, startColumnIndex);

        if (rows == null || rows.Count == 0)
            Debug.LogError($"{gameObject.name} failed to load upgrade rows.");
    }

    public bool IsMaxLevel()
    {
        return rows == null || rows.Count == 0 || currentLevel >= rows.Count - 1;
    }

    public int CurrentValue
    {
        get
        {
            if (CurrentRow == null)
                return forceMinimumOne ? 1 : 0;

            if (forceMinimumOne)
                return Mathf.Max(1, CurrentRow.value);

            return CurrentRow.value;
        }
    }
    
    public int CurrentStageDisplay => CurrentRow != null ? CurrentRow.stageDisplay : 0;
    public double CurrentUpgradeCost => CurrentRow != null ? CurrentRow.cost : 0d;

    public bool CanUpgrade()
    {
        if (IsMaxLevel())
            return false;

        if (GoldManager.Instance == null)
            return false;

        return GoldManager.Instance.CurrentGold >= CurrentUpgradeCost;
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade())
            return false;

        if (GoldManager.Instance == null)
        {
            Debug.LogError("GoldManager가 씬에 없습니다.");
            return false;
        }

        if (!GoldManager.Instance.TrySpendGold(CurrentUpgradeCost))
            return false;

        currentLevel++;
        GameDataController.Instance?.SaveGame();
        return true;
    }

    public void SetLevel(int level)
    {
        // 누가 SetLevel 호출하는지 콜스택 출력
        Debug.Log($"[SetLevel 호출] id={saveId} / level={level}\n{System.Environment.StackTrace}");
    
        if (rows == null || rows.Count == 0)
        {
            currentLevel = 0;
            return;
        }

        currentLevel = Mathf.Clamp(level, 0, rows.Count - 1);
    }

    public UpgradeSaveData GetSaveData()
    {
        return new UpgradeSaveData
        {
            upgradeId = saveId,
            level = currentLevel
        };
    }
}