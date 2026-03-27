using System.Collections.Generic;
using UnityEngine;
using DesignPattern;

public class MaterialInventory : Singleton<MaterialInventory>
{
    [SerializeField] private List<MaterialEntry> startMaterials = new();
    [SerializeField] private List<MaterialDataSO> allMaterials = new();

    private Dictionary<MaterialDataSO, int> materialDict = new();
    private Dictionary<string, MaterialDataSO> materialLookup = new();

    protected override void OnAwake()
    {
        BuildLookup();
        InitDefaultMaterials();
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
            if (entry.material == null)
                continue;

            if (materialDict.ContainsKey(entry.material))
                materialDict[entry.material] += entry.count;
            else
                materialDict.Add(entry.material, entry.count);
        }
    }

    public void LoadFromSave(List<MaterialSaveData> savedMaterials)
    {
        materialDict.Clear();

        if (savedMaterials == null)
            return;

        foreach (var saved in savedMaterials)
        {
            if (saved == null || string.IsNullOrEmpty(saved.materialId))
                continue;

            if (materialLookup.TryGetValue(saved.materialId, out var material))
            {
                materialDict[material] = saved.count;
            }
            else
            {
                Debug.LogWarning($"로드 실패: materialId {saved.materialId} 를 찾을 수 없습니다.");
            }
        }
    }

    public List<MaterialSaveData> GetSaveData()
    {
        List<MaterialSaveData> list = new();

        foreach (var pair in materialDict)
        {
            if (pair.Key == null || string.IsNullOrEmpty(pair.Key.saveId))
                continue;

            list.Add(new MaterialSaveData
            {
                materialId = pair.Key.saveId,
                count = pair.Value
            });
        }

        return list;
    }

    public int GetCount(MaterialDataSO material)
    {
        if (material == null) return 0;
        return materialDict.TryGetValue(material, out int count) ? count : 0;
    }

    public bool CanCraft(GearsetRecipeSO recipe)
    {
        if (recipe == null) return false;

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
            materialDict[req.material] -= req.requiredCount;
        }

        GameDataController.Instance?.SaveGame();
    }

    public void AddMaterial(MaterialDataSO material, int amount)
    {
        if (material == null)
        {
            Debug.LogWarning("AddMaterial 실패: material이 null입니다.");
            return;
        }

        if (amount <= 0)
        {
            Debug.LogWarning($"AddMaterial 실패: 잘못된 수량 {amount}");
            return;
        }

        if (materialDict.ContainsKey(material))
            materialDict[material] += amount;
        else
            materialDict.Add(material, amount);

        Debug.Log($"{material.materialName} {amount}개 획득, 현재 보유: {materialDict[material]}");

        GameDataController.Instance?.SaveGame();
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

            AddMaterial(reward.material, reward.count);
        }
    }

    public Dictionary<MaterialDataSO, int> GetAllMaterials()
    {
        return materialDict;
    }
}

[System.Serializable]
public class MaterialEntry
{
    public MaterialDataSO material;
    public int count;
}