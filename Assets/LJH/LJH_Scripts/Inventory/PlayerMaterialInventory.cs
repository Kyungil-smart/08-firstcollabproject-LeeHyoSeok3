using System.Collections.Generic;
using UnityEngine;
using DesignPattern;

public class MaterialInventory : Singleton<MaterialInventory>
{
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

            if (materialDict.ContainsKey(id))
                materialDict[id] += entry.count;
            else
                materialDict.Add(id, entry.count);
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
                materialDict[saved.materialId] = saved.count;
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

        if (materialDict.ContainsKey(id))
            materialDict[id] += amount;
        else
            materialDict.Add(id, amount);

        Debug.Log($"{material.materialName} {amount}개 획득, 현재 보유: {materialDict[id]}");

        RefreshUI();
        GameDataController.Instance?.SaveGame();
        NotifyChanged();
    }

    public void AddMaterials(List<MaterialEntry> rewards)
    {
        if (rewards == null || rewards.Count == 0)
        {
            Debug.LogWarning("AddMaterials 실패: rewards가 비어있습니다.");
            return;
        }

        foreach (var reward in rewards)
        {
            if (reward == null || reward.material == null)
                continue;

            if (string.IsNullOrEmpty(reward.material.saveId))
                continue;

            if (reward.count <= 0)
                continue;

            string id = reward.material.saveId;

            if (materialDict.ContainsKey(id))
                materialDict[id] += reward.count;
            else
                materialDict.Add(id, reward.count);
        }

        RefreshUI();
        GameDataController.Instance?.SaveGame();
        NotifyChanged();
    }

    // UI 출력용
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