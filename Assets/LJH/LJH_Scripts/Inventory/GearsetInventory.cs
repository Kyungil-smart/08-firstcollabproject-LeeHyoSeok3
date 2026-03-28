using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using TMPro;

public class GearsetInventory : Singleton<GearsetInventory>
{
    [Header("기본 제작 완료 장비 (첫 장비 넣기)")]
    [SerializeField] private List<GearsetRecipeSO> defaultCraftedList;
    [SerializeField] private List<GearsetRecipeSO> allRecipes;
    [SerializeField] private TextMeshProUGUI valueText;

    private HashSet<GearsetRecipeSO> craftedGearsets = new HashSet<GearsetRecipeSO>();
    private Dictionary<string, GearsetRecipeSO> recipeLookup = new Dictionary<string, GearsetRecipeSO>();

    protected override void OnAwake()
    {
        BuildLookup();
        InitDefaultCrafted();
        RefreshUI();
    }

    private void BuildLookup()
    {
        recipeLookup.Clear();

        foreach (var recipe in allRecipes)
        {
            if (recipe == null)
                continue;

            if (string.IsNullOrEmpty(recipe.saveId))
            {
                Debug.LogWarning($"{recipe.name} : saveId가 비어있습니다.");
                continue;
            }

            if (recipeLookup.ContainsKey(recipe.saveId))
            {
                Debug.LogWarning($"중복된 Gearset saveId: {recipe.saveId}");
                continue;
            }

            recipeLookup.Add(recipe.saveId, recipe);
        }
    }

    private void InitDefaultCrafted()
    {
        craftedGearsets.Clear();

        foreach (var recipe in defaultCraftedList)
        {
            if (recipe != null)
                craftedGearsets.Add(recipe);
        }
    }

    public void LoadFromSave(List<string> craftedIds)
    {
        craftedGearsets.Clear();

        if (craftedIds == null)
        {
            RefreshUI();
            return;
        }

        foreach (var id in craftedIds)
        {
            if (string.IsNullOrEmpty(id))
                continue;

            if (recipeLookup.TryGetValue(id, out var recipe))
            {
                craftedGearsets.Add(recipe);
            }
            else
            {
                Debug.LogWarning($"로드 실패: gearsetId {id} 를 찾을 수 없습니다.");
            }
        }

        RefreshUI();
    }

    public List<string> GetSaveData()
    {
        List<string> ids = new();

        foreach (var gear in craftedGearsets)
        {
            if (gear == null || string.IsNullOrEmpty(gear.saveId))
                continue;

            ids.Add(gear.saveId);
        }

        return ids;
    }

    public bool IsCrafted(GearsetRecipeSO recipe)
    {
        if (recipe == null)
            return false;

        return craftedGearsets.Contains(recipe);
    }

    public void AddCrafted(GearsetRecipeSO recipe)
    {
        if (recipe == null)
            return;

        if (craftedGearsets.Contains(recipe))
            return;

        craftedGearsets.Add(recipe);

        RefreshUI();
        GameDataController.Instance?.SaveGame();
    }

    public IReadOnlyCollection<GearsetRecipeSO> GetAllCrafted()
    {
        return craftedGearsets;
    }

    public void RefreshUI()
    {
        if (valueText == null)
            return;

        if (craftedGearsets.Count == 0)
        {
            valueText.text = "보유 장비 없음";
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (var gear in craftedGearsets)
        {
            if (gear == null)
                continue;

            sb.AppendLine($"- {gear.name}");
        }

        valueText.text = sb.ToString();
    }
}