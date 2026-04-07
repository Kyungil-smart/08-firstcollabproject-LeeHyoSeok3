using System.Collections.Generic;
using UnityEngine;
using DesignPattern;

public class MaterialInventory : Singleton<MaterialInventory>
{
    private const int DefaultMaxMaterialCount = 99;

    [SerializeField] private List<MaterialEntry> startMaterials = new();
    [SerializeField] private List<MaterialDataSO> allMaterials = new();
    
    private Dictionary<string, int> materialDict = new();
    private Dictionary<string, MaterialDataSO> materialLookup = new();
    
    public event System.Action OnInventoryChanged;

    private void NotifyChanged()
    {
        OnInventoryChanged?.Invoke();
    }
    
    protected override void OnAwake()
    {
        BuildLookup();
        InitDefaultMaterials();
        RefreshUI();
    }

    private void BuildLookup()
    {
        materialLookup.Clear();

        foreach (var material in allMaterials)
        {
            if (material == null)
                continue;

            if (string.IsNullOrEmpty(material.saveId))
            {
                Debug.LogWarning($"{material.name} : saveId가 비어있습니다.");
                continue;
            }

            if (materialLookup.ContainsKey(material.saveId))
            {
                Debug.LogWarning($"중복된 Material saveId: {material.saveId}");
                continue;
            }

            materialLookup.Add(material.saveId, material);
        }
    }

    private void InitDefaultMaterials()
    {
        materialDict.Clear();

        foreach (var entry in startMaterials)
        {
            if (entry == null || entry.material == null || string.IsNullOrEmpty(entry.material.saveId))
                continue;

            string id = entry.material.saveId;
            int clampedCount = Mathf.Clamp(entry.count, 0, GetMaxCount(entry.material));

            if (materialDict.ContainsKey(id))
                materialDict[id] = Mathf.Min(materialDict[id] + clampedCount, GetMaxCount(entry.material));
            else
                materialDict.Add(id, clampedCount);
        }
    }

    public void LoadFromSave(List<MaterialSaveData> savedMaterials)
    {
        materialDict.Clear();

        if (savedMaterials == null)
        {
            InitDefaultMaterials();
            RefreshUI();
            return;
        }

        foreach (var saved in savedMaterials)
        {
            if (saved == null || string.IsNullOrEmpty(saved.materialId))
                continue;

            if (materialLookup.ContainsKey(saved.materialId))
            {
                MaterialDataSO material = materialLookup[saved.materialId];
                materialDict[saved.materialId] = Mathf.Clamp(saved.count, 0, GetMaxCount(material));
            }
            else
            {
                Debug.LogWarning($"로드 실패: materialId {saved.materialId} 를 찾을 수 없습니다.");
            }
        }

        RefreshUI();
        NotifyChanged();
    }

    public List<MaterialSaveData> GetSaveData()
    {
        List<MaterialSaveData> list = new();

        foreach (var pair in materialDict)
        {
            list.Add(new MaterialSaveData
            {
                materialId = pair.Key,
                count = pair.Value
            });
        }

        return list;
    }

    public int GetCount(MaterialDataSO material)
    {
        if (material == null || string.IsNullOrEmpty(material.saveId))
            return 0;

        return materialDict.TryGetValue(material.saveId, out int count) ? count : 0;
    }

    public bool CanCraft(GearsetRecipeSO recipe)
    {
        if (recipe == null)
            return false;

        foreach (var req in recipe.requirements)
        {
            if (req == null || req.material == null)
                return false;

            if (GetCount(req.material) < req.requiredCount)
                return false;
        }

        return true;
    }

    public void Consume(GearsetRecipeSO recipe)
    {
        if (recipe == null || !CanCraft(recipe))
            return;

        foreach (var req in recipe.requirements)
        {
            if (req == null || req.material == null || string.IsNullOrEmpty(req.material.saveId))
                continue;

            string id = req.material.saveId;

            if (materialDict.ContainsKey(id))
                materialDict[id] -= req.requiredCount;
        }

        RefreshUI();
        GameDataController.Instance?.SaveGame();
        NotifyChanged();
    }

    public void AddMaterial(MaterialDataSO material, int amount)
    {
        if (material == null)
        {
            Debug.LogWarning("AddMaterial 실패: material이 null입니다.");
            return;
        }

        if (string.IsNullOrEmpty(material.saveId))
        {
            Debug.LogWarning($"{material.name} : saveId가 비어있습니다.");
            return;
        }

        if (amount <= 0)
        {
            Debug.LogWarning($"AddMaterial 실패: 잘못된 수량 {amount}");
            return;
        }

        string id = material.saveId;

        int addedAmount = AddMaterialInternal(material, amount);

        if (addedAmount <= 0) return;

        Debug.Log($"{material.GetMaterialName()} {amount}개 획득, 현재 보유: {materialDict[id]}");

        RefreshUI();
        GameDataController.Instance?.SaveGame();
        NotifyChanged();
    }

    public void AddMaterials(List<MaterialEntry> rewards)
    {
        if (rewards == null || rewards.Count == 0) return;

        bool hasChange = false;

        foreach (var reward in rewards)
        {
            if (reward == null || reward.material == null)
                continue;

            if (string.IsNullOrEmpty(reward.material.saveId))
                continue;

            if (reward.count <= 0)
                continue;

            int addedAmount = AddMaterialInternal(reward.material, reward.count);

            if (addedAmount > 0)
                hasChange = true;
        }

        if (!hasChange)
            return;

        RefreshUI();
        GameDataController.Instance?.SaveGame();
        NotifyChanged();
    }

    // UI 출력용
    private int GetMaxCount(MaterialDataSO material)
    {
        if (material == null || material.maxCount <= 0)
            return DefaultMaxMaterialCount;

        return material.maxCount;
    }

    private int AddMaterialInternal(MaterialDataSO material, int amount)
    {
        if (material == null || amount <= 0 || string.IsNullOrEmpty(material.saveId))
            return 0;

        string id = material.saveId;
        int currentCount = materialDict.TryGetValue(id, out int savedCount) ? savedCount : 0;
        int maxCount = GetMaxCount(material);
        int addedAmount = Mathf.Clamp(amount, 0, maxCount - currentCount);

        if (addedAmount <= 0)
            return 0;

        materialDict[id] = currentCount + addedAmount;
        return addedAmount;
    }

    public Dictionary<MaterialDataSO, int> GetAllMaterials()
    {
        Dictionary<MaterialDataSO, int> result = new();

        foreach (var pair in materialDict)
        {
            if (materialLookup.TryGetValue(pair.Key, out var material))
            {
                result[material] = pair.Value;
            }
        }

        return result;
    }

    private void OnEnable()
    {
        // TryBindInventory();

        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += RefreshUI;

        if (GameDataController.Instance != null && GameDataController.Instance.IsLoaded)
            RefreshUI();
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshUI;
    }
    
    public void RefreshUI()
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
}

[System.Serializable]
public class MaterialEntry
{
    public MaterialDataSO material;
    public int count;
}