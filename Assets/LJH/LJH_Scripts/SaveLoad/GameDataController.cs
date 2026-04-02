using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using UnityEngine.SceneManagement;
using System;

public class GameDataController : Singleton<GameDataController>
{
    public static event System.Action OnGameLoaded;

    private bool isLoaded = false;
    public bool IsLoaded => isLoaded;
    private UpgradeSystem[] upgradeSystems;

    protected override void OnAwake()
    {
    }

    private void Start()
    {
        upgradeSystems = FindObjectsByType<UpgradeSystem>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        Debug.Log($"[INIT] 발견된 UpgradeSystem 수: {upgradeSystems.Length}");

        LoadGame();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        if (!isLoaded)
        {
            Debug.LogWarning("SaveGame 중단: isLoaded=false");
            return;
        }

        GameSaveData data = new GameSaveData();

        if (GoldManager.Instance != null)
            data.gold = GoldManager.Instance.CurrentGold;

        if (MaterialInventory.Instance != null)
            data.materials = MaterialInventory.Instance.GetSaveData();

        if (GearsetInventory.Instance != null)
            data.craftedGearIds = GearsetInventory.Instance.GetSaveData();

        var currentUpgradeSystems = FindObjectsByType<UpgradeSystem>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        Debug.Log($"[SAVE] 발견된 UpgradeSystem 수: {currentUpgradeSystems.Length}");

        foreach (var upgrade in currentUpgradeSystems)
        {
            if (upgrade == null || string.IsNullOrEmpty(upgrade.SaveId))
                continue;

            var saveData = upgrade.GetSaveData();
            Debug.Log($"[SAVE] {upgrade.gameObject.name} / id={saveData.upgradeId} / level={saveData.level}");
            data.upgrades.Add(saveData);
        }

        Debug.Log($"[SAVE] upgrades count = {data.upgrades.Count}");

        // 퀘스트 저장
        if (QuestManager.Instance != null)
        {
            data.isQuestActive = QuestManager.Instance.IsQuestActive;
            data.currentQuestDungeonName = QuestManager.Instance.CurrentQuest != null
                ? QuestManager.Instance.CurrentQuest.dungeonName
                : string.Empty;

            data.questStartTime = QuestManager.Instance.QuestStartTime != DateTime.MinValue
                ? QuestManager.Instance.QuestStartTime.ToString("o")
                : string.Empty;

            data.questEndTime = QuestManager.Instance.QuestEndTime != DateTime.MinValue
                ? QuestManager.Instance.QuestEndTime.ToString("o")
                : string.Empty;

            data.hasCompletedQuest = QuestManager.Instance.HasCompletedQuest;
            data.completedQuestDungeonName = QuestManager.Instance.CompletedQuest != null
                ? QuestManager.Instance.CompletedQuest.dungeonName
                : string.Empty;
        }

        SaveManager.Save(data);
    }

    public void LoadGame()
    {
        GameSaveData data = SaveManager.Load();

        if (data == null)
        {
            Debug.LogWarning("[LOAD] save data null");

            if (GoldManager.Instance != null)
                GoldManager.Instance.SetGold(0);

            if (MaterialInventory.Instance != null)
                MaterialInventory.Instance.LoadFromSave(null);

            if (GearsetInventory.Instance != null)
                GearsetInventory.Instance.LoadFromSave(null);

            if (upgradeSystems != null)
            {
                foreach (var upgrade in upgradeSystems)
                {
                    if (upgrade == null || string.IsNullOrEmpty(upgrade.SaveId))
                        continue;

                    upgrade.SetLevel(0);
                }
            }

            isLoaded = true;
            OnGameLoaded?.Invoke();
            RefreshAllUIs();
            SaveGame();
            return;
        }

        Debug.Log($"[LOAD] upgrades count = {(data.upgrades != null ? data.upgrades.Count : -1)}");

        if (GoldManager.Instance != null)
            GoldManager.Instance.SetGold(data.gold);

        if (MaterialInventory.Instance != null)
            MaterialInventory.Instance.LoadFromSave(data.materials);

        if (GearsetInventory.Instance != null)
            GearsetInventory.Instance.LoadFromSave(data.craftedGearIds);

        if (upgradeSystems != null && data.upgrades != null)
        {
            Dictionary<string, int> loadedUpgradeLevels = new Dictionary<string, int>();

            foreach (var saved in data.upgrades)
            {
                if (saved == null || string.IsNullOrEmpty(saved.upgradeId))
                    continue;

                Debug.Log($"[LOAD-DATA] id={saved.upgradeId} / level={saved.level}");
                loadedUpgradeLevels[saved.upgradeId] = saved.level;
            }

            foreach (var upgrade in upgradeSystems)
            {
                if (upgrade == null || string.IsNullOrEmpty(upgrade.SaveId))
                    continue;

                if (loadedUpgradeLevels.TryGetValue(upgrade.SaveId, out int level))
                {
                    Debug.Log($"[APPLY] {upgrade.gameObject.name} / id={upgrade.SaveId} / level={level}");
                    upgrade.SetLevel(level);
                }
                else
                {
                    Debug.LogWarning($"[APPLY] 저장 데이터 없음 -> 0으로 초기화 / id={upgrade.SaveId}");
                    upgrade.SetLevel(0);
                }
            }
        }

          // 퀘스트 로드
            DungeonDataLoader dungeonLoader = FindFirstObjectByType<DungeonDataLoader>();

            DungeonData loadedCurrentQuest = null;
            DungeonData loadedCompletedQuest = null;
            DateTime loadedStartTime = DateTime.MinValue;
            DateTime loadedEndTime = DateTime.MinValue;

            if (dungeonLoader != null)
            {
                if (data.isQuestActive && !string.IsNullOrEmpty(data.currentQuestDungeonName))
                {
                    loadedCurrentQuest = dungeonLoader.FindDungeonByName(data.currentQuestDungeonName);

                    if (!string.IsNullOrEmpty(data.questStartTime))
                        DateTime.TryParse(data.questStartTime, out loadedStartTime);

                    if (!string.IsNullOrEmpty(data.questEndTime))
                        DateTime.TryParse(data.questEndTime, out loadedEndTime);
                }

                if (data.hasCompletedQuest && !string.IsNullOrEmpty(data.completedQuestDungeonName))
                {
                    loadedCompletedQuest = dungeonLoader.FindDungeonByName(data.completedQuestDungeonName);
                }
            }

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.LoadQuestState(
                    loadedCurrentQuest,
                    loadedStartTime,
                    loadedEndTime,
                    loadedCompletedQuest
                );

                QuestManager.Instance.ApplyOfflineQuestProgress();
            }

        isLoaded = true;
        OnGameLoaded?.Invoke();
        RefreshAllUIs();
    }

    public void ResetGame()
    {
        SaveManager.DeleteSave();
        isLoaded = false;
        OnGameLoaded = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RefreshAllUIs()
    {
        RefreshMaterialInventoryUIs();
        RefreshGearsetSlots();
        RefreshUpgradeUIs();
    }

    private void RefreshMaterialInventoryUIs()
    {
        MaterialInventoryUI[] uiList = FindObjectsByType<MaterialInventoryUI>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var ui in uiList)
        {
            if (ui != null)
                ui.RefreshUI();
        }
    }

    private void RefreshGearsetSlots()
    {
        GearsetSlotUI[] slotList = FindObjectsByType<GearsetSlotUI>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var slot in slotList)
        {
            if (slot != null)
                slot.RefreshState();
        }
    }

    private void RefreshUpgradeUIs()
    {
        UpgradeUI[] uiList = FindObjectsByType<UpgradeUI>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var ui in uiList)
        {
            if (ui != null)
                ui.RefreshUI();
        }
    }
}