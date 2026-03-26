using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using TMPro;

public class GearsetInventory : Singleton<GearsetInventory>
{
    [Header("기본 제작 완료 장비 (첫 장비 넣기)")]
    [SerializeField] private List<GearsetRecipeSO> defaultCraftedList;
    
    [SerializeField] private TextMeshProUGUI valueText;

    private HashSet<GearsetRecipeSO> craftedGearsets = new HashSet<GearsetRecipeSO>();
    
    protected override void OnAwake()
    {
        foreach (var recipe in defaultCraftedList)
        {
            if (recipe != null)
                craftedGearsets.Add(recipe);
        }

        RefreshUI();
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