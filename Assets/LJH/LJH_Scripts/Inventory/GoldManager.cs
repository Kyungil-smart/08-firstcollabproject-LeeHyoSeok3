using System;
using TMPro;
using UnityEngine;
using DesignPattern;

public class GoldManager : Singleton<GoldManager>
{
    [SerializeField] private int currentGold = 0;
    [SerializeField] private TMP_Text goldText;

    public int CurrentGold => currentGold;

    public event Action<int> OnGoldChanged;

    protected override void OnAwake()
    {
        RefreshUI();
    }

    private void Start()
    {
        OnGoldChanged?.Invoke(currentGold);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        currentGold += amount;
        NotifyGoldChanged();
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0)
            return true;

        if (currentGold < amount)
            return false;

        currentGold -= amount;
        NotifyGoldChanged();
        return true;
    }

    public void SetGold(int amount)
    {
        currentGold = Mathf.Max(0, amount);
        NotifyGoldChanged();
    }

    private void NotifyGoldChanged()
    {
        RefreshUI();
        OnGoldChanged?.Invoke(currentGold);
    }

    private void RefreshUI()
    {
        if (goldText != null)
            goldText.text = currentGold.ToString("N0");
    }
}