using System;
using DesignPattern;
using UnityEngine;

public class PartyEquipManager : Singleton<PartyEquipManager>
{
    [Header("Equipment Data Reference")]
    [SerializeField] private DataManager dataManager;

    [Header("Fallback Attribute")]
    [SerializeField] private string defaultAttribute = "None";
    [SerializeField] private string defaultAttributeKey = "";

    public event Action<string> OnAttributeChanged;

    public string CurrentAttribute
    {
        get
        {
            if (dataManager != null && !string.IsNullOrWhiteSpace(dataManager.CurrentPartyTrait))
                return dataManager.CurrentPartyTrait;

            return defaultAttribute;
        }
    }

    public string CurrentAttributeKey
    {
        get
        {
            if (dataManager != null && !string.IsNullOrWhiteSpace(dataManager.CurrentPartyTraitKey))
                return dataManager.CurrentPartyTraitKey;

            return defaultAttributeKey;
        }
    }

    protected override void OnAwake()
    {
        BindDataManagerIfNeeded();
    }

    private void OnEnable()
    {
        BindDataManagerIfNeeded();

        if (dataManager != null)
        {
            dataManager.OnPartyTraitChanged += HandlePartyTraitChanged;
        }
    }

    private void OnDisable()
    {
        if (dataManager != null)
        {
            dataManager.OnPartyTraitChanged -= HandlePartyTraitChanged;
        }
    }

    public void SetAttribute(string newAttribute)
    {
        defaultAttribute = string.IsNullOrWhiteSpace(newAttribute) ? "None" : newAttribute.Trim();
        OnAttributeChanged?.Invoke(CurrentAttribute);
        Debug.Log($"[PartyEquipManager] fallback attribute changed: {CurrentAttribute}");
    }

    public bool CanEnterDungeon(string requiredTraitKey)
    {
        Debug.Log("[PartyEquipManager] CanEnterDungeon called");
        Debug.Log($"[PartyEquipManager] CurrentAttribute = '{CurrentAttribute}'");
        Debug.Log($"[PartyEquipManager] CurrentAttributeKey = '{CurrentAttributeKey}'");
        Debug.Log($"[PartyEquipManager] requiredTraitKey = '{requiredTraitKey}'");

        if (string.IsNullOrWhiteSpace(requiredTraitKey))
        {
            Debug.Log("[PartyEquipManager] requiredTraitKey is empty, allowing entry");
            return true;
        }

        bool result = string.Equals(
            CurrentAttributeKey?.Trim(),
            requiredTraitKey.Trim(),
            StringComparison.Ordinal);

        Debug.Log($"[PartyEquipManager] compare result = {result}");
        return result;
    }

    private void BindDataManagerIfNeeded()
    {
        if (dataManager == null)
        {
            dataManager = FindFirstObjectByType<DataManager>();
        }
    }

    private void HandlePartyTraitChanged(string newAttributeKey)
    {
        Debug.Log($"[PartyEquipManager] trait sync event received / newAttributeKey = '{newAttributeKey}'");
        Debug.Log($"[PartyEquipManager] CurrentAttribute = '{CurrentAttribute}'");
        Debug.Log($"[PartyEquipManager] CurrentAttributeKey = '{CurrentAttributeKey}'");
        OnAttributeChanged?.Invoke(CurrentAttribute);
    }
}
