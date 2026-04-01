using System;
using TMPro;
using UnityEngine;
using DesignPattern;

public class GoldManager : Singleton<GoldManager>
{
    [SerializeField] private double currentGold = 0;
    [SerializeField] private TMP_Text mainGoldText;
    [SerializeField] private TMP_Text minimizedGoldText;

    public double CurrentGold => currentGold;

    public event Action<double> OnGoldChanged;

    protected override void OnAwake()
    {
        RefreshUI();
    }

    private void Start()
    {
        OnGoldChanged?.Invoke(currentGold);
    }

    public void AddGold(double amount)
    {
        if (amount <= 0)
            return;

        currentGold += amount;
        NotifyGoldChanged();
        GameDataController.Instance?.SaveGame();
    }

    public bool TrySpendGold(double amount)
    {
        if (amount <= 0)
            return true;

        if (currentGold < amount)
            return false;

        currentGold -= amount;
        NotifyGoldChanged();
        GameDataController.Instance?.SaveGame();
        return true;
    }

    public void SetGold(double amount)
    {
        currentGold = Math.Max(0, amount);
        NotifyGoldChanged();
    }

    private void NotifyGoldChanged()
    {
        RefreshUI();
        OnGoldChanged?.Invoke(currentGold);
    }

    private void RefreshUI()
    {
        string formatted = FormatGold(currentGold);

        if (mainGoldText != null)
            mainGoldText.text = formatted;

        if (minimizedGoldText != null)
            minimizedGoldText.text = formatted;
    }

    public static string FormatGold(double value)
    {
        if (value < 1000d)
            return Math.Floor(value).ToString("F0");

        string[] units = { "", "K", "M", "B", "T", "aa", "ab", "ac", "ad", "ae" };

        int unitIndex = 0;
        while (value >= 1000d && unitIndex < units.Length - 1)
        {
            value /= 1000d;
            unitIndex++;
        }

        return $"{value:F2}{units[unitIndex]}";
    }
}