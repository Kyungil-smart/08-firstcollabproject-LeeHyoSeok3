using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using UnityEngine.SceneManagement;

public class GameDataController : Singleton<GameDataController>
{
    private bool isLoaded = false;
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

        LoadGame();

        RefreshMaterialInventoryUIs();
        RefreshGearsetSlots();
        RefreshUpgradeUIs();
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
            return;

        GameSaveData data = new GameSaveData();

        if (GoldManager.Instance != null)
            data.gold = GoldManager.Instance.CurrentGold;

        if (MaterialInventory.Instance != null)
            data.materials = MaterialInventory.Instance.GetSaveData();

        if (GearsetInventory.Instance != null)
            data.craftedGearIds = GearsetInventory.Instance.GetSaveData();

        if (upgradeSystems != null)
        {
            foreach (var upgrade in upgradeSystems)
            {
                if (upgrade == null || string.IsNullOrEmpty(upgrade.SaveId))
                    continue;

                data.upgrades.Add(upgrade.GetSaveData());
            }
        }

        SaveManager.Save(data);
    }

    public void LoadGame()
    {
        GameSaveData data = SaveManager.Load();

        if (data == null)
        {
            isLoaded = true;
            SaveGame();
            return;
        }

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

                loadedUpgradeLevels[saved.upgradeId] = saved.level;
            }

            foreach (var upgrade in upgradeSystems)
            {
                if (upgrade == null || string.IsNullOrEmpty(upgrade.SaveId))
                    continue;

                if (loadedUpgradeLevels.TryGetValue(upgrade.SaveId, out int level))
                    upgrade.SetLevel(level);
                else
                    upgrade.SetLevel(0);
            }
        }

        isLoaded = true;
    }

    public void ResetGame()
    {
        SaveManager.DeleteSave();
        isLoaded = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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