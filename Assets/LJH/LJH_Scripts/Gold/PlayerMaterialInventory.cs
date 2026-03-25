using System.Collections.Generic;
using UnityEngine;

public class MaterialInventory : MonoBehaviour
{
    [SerializeField] private List<MaterialEntry> startMaterials = new List<MaterialEntry>();

    private Dictionary<MaterialDataSO, int> materialDict = new Dictionary<MaterialDataSO, int>();

    private void Awake()
    {
        materialDict.Clear();

        Debug.Log("=== 인벤토리 Awake 시작 ===");

        foreach (var entry in startMaterials)
        {
            if (entry.material == null)
            {
                Debug.LogWarning("material이 null인 startMaterials 항목 있음");
                continue;
            }

            if (materialDict.ContainsKey(entry.material))
                materialDict[entry.material] += entry.count;
            else
                materialDict.Add(entry.material, entry.count);

            Debug.Log($"{entry.material.materialName} 등록됨: {materialDict[entry.material]}");
        }

        Debug.Log("=== 인벤토리 Awake 종료 ===");
    }

    public int GetCount(MaterialDataSO material)
    {
        if (material == null)
            return 0;

        if (materialDict.TryGetValue(material, out int count))
            return count;

        return 0;
    }

    public bool CanCraft(GearsetRecipeSO recipe)
    {
        if (recipe == null)
            return false;

        for (int i = 0; i < recipe.requirements.Count; i++)
        {
            var req = recipe.requirements[i];

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

        for (int i = 0; i < recipe.requirements.Count; i++)
        {
            var req = recipe.requirements[i];
            materialDict[req.material] -= req.requiredCount;

            Debug.Log($"{req.material.materialName} 소모 후 남은 개수: {materialDict[req.material]}");
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