using UnityEngine;
using System;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private GearsetRecipeDatabaseSO m_recipeDatabase;

    private Dictionary<int, GearsetRecipeSO> m_itemDatabase = new Dictionary<int, GearsetRecipeSO>();
    private Dictionary<string, int> m_itemIdBySaveId = new Dictionary<string, int>(); // saveId로 파티 장착 시스템과 연동
    private List<ItemData> m_inventory = new List<ItemData>();

    private int m_partyEquippedItemID = 0;

    // 현재 파티 특성 표시명/비교키를 따로 보관합니다.
    private string m_currentPartyTrait = "None";
    private string m_currentPartyTraitKey = "";

    // 파티 특성이 변경될 때마다 발생하는 이벤트 지정(특성에 따라 퀘스트를 입장할 수 있는지 확인)
    public event Action<string> OnPartyTraitChanged;

    private void Awake()
    {
        LoadScriptableObjectData();
    }

    private void LoadScriptableObjectData()
    {
        if (m_recipeDatabase == null)
        {
            Debug.LogError("[DataManager] Database SO가 연결되지 않았습니다! 인스펙터를 확인해주세요.");
            return;
        }

        for (int i = 0; i < m_recipeDatabase.recipes.Count; i++)
        {
            GearsetRecipeSO recipe = m_recipeDatabase.recipes[i];
            if (recipe == null) continue;

            m_itemDatabase.Add(i, recipe);

            // saveId가 유효하면, saveId -> itemID 매핑도 함께 저장
            if (!string.IsNullOrWhiteSpace(recipe.saveId) && !m_itemIdBySaveId.ContainsKey(recipe.saveId))
            {
                m_itemIdBySaveId.Add(recipe.saveId, i);
            }

            // 박사님이 작성하신 SO 변수명에 맞춰 매핑합니다.
            ItemData newItem = new ItemData
            {
                id = i,
                name = recipe.gearsetName,
                description = recipe.gearDescription,
                icon = recipe.gearIcon,
                isCrafted = (recipe.saveId == "Rusty"),
                isUnlocked = (recipe.saveId == "Rusty"),

                // 특성 데이터 매핑
                traitKey = string.IsNullOrWhiteSpace(recipe.traitKey) ? recipe.traitName : recipe.traitKey,
                traitName = recipe.traitName,
                traitDescription = recipe.traitDescription,
                traitIcon = recipe.traitIcon
            };

            m_inventory.Add(newItem);
        }
    }

    public List<ItemData> GetInventoryItems() => m_inventory;
    public int GetEquippedItemID() => m_partyEquippedItemID;
    public string CurrentPartyTrait => m_currentPartyTrait; // 현재 파티 특성을 외부에서 읽을 수 있도록 하는 프로퍼티 선언
    public string CurrentPartyTraitKey => m_currentPartyTraitKey;

    // 💡 [에러 해결!] InventoryEquipmentPopup 등에서 호출하던 함수를 복구했습니다.
    public ItemData GetItemByID(int id)
    {
        for (int i = 0; i < m_inventory.Count; i++)
        {
            if (m_inventory[i].id == id)
            {
                return m_inventory[i];
            }
        }
        return null; // ItemData를 class로 변경했으므로 못 찾으면 null을 반환합니다.
    }

    public void UnlockItem(int id)
    {
        for (int i = 0; i < m_inventory.Count; i++)
        {
            if (m_inventory[i].id == id)
            {
                if (!m_inventory[i].isCrafted)
                {
                    Debug.LogWarning($"[DataManager] 제작되지 않은 아이템(ID {id})은 해금할 수 없습니다.");
                    return;
                }

                m_inventory[i].isUnlocked = true;
                break;
            }
        }
    }

    public void MarkItemCrafted(int id)
    {
        for (int i = 0; i < m_inventory.Count; i++)
        {
            if (m_inventory[i].id == id)
            {
                m_inventory[i].isCrafted = true;
                break;
            }
        }
    }

    /// <summary>
    /// saveId로 아이템을 해금하는 메서드입니다.
    /// </summary>
    /// <param name="saveId"></param>
    /// <returns></returns>
    public bool UnlockItemBySaveId(string saveId)
    {
        if (string.IsNullOrWhiteSpace(saveId))
        {
            Debug.LogWarning("[DataManager] UnlockItemBySaveId 실패: saveId가 비어 있습니다.");
            return false;
        }

        if (!m_itemIdBySaveId.TryGetValue(saveId, out int itemId))
        {
            Debug.LogWarning($"[DataManager] UnlockItemBySaveId 실패: saveId '{saveId}'와 매칭되는 아이템이 없습니다.");
            return false;
        }

        UnlockItem(itemId);
        Debug.Log($"[DataManager] saveId '{saveId}'에 해당하는 아이템(ID {itemId}) 해금 완료");
        return true;
    }

    public bool MarkItemCraftedBySaveId(string saveId)
    {
        if (string.IsNullOrWhiteSpace(saveId))
        {
            Debug.LogWarning("[DataManager] MarkItemCraftedBySaveId 실패: saveId가 비어 있습니다.");
            return false;
        }

        if (!m_itemIdBySaveId.TryGetValue(saveId, out int itemId))
        {
            Debug.LogWarning($"[DataManager] MarkItemCraftedBySaveId 실패: saveId '{saveId}'와 매칭되는 아이템이 없습니다.");
            return false;
        }

        MarkItemCrafted(itemId);
        Debug.Log($"[DataManager] saveId '{saveId}'에 해당하는 아이템(ID {itemId}) 제작 완료 표시");
        return true;
    }

    // 저장/로드 과정에서 호출할 아이템 상태 적용
    public void ApplyCraftedState(List<string> craftedSaveIds)
    {
        for (int i = 0; i < m_inventory.Count; i++)
        {
            bool isDefaultUnlocked = (i == 0);
            m_inventory[i].isCrafted = isDefaultUnlocked;
            m_inventory[i].isUnlocked = isDefaultUnlocked;
        }

        if (craftedSaveIds == null)
            return;

        foreach (string saveId in craftedSaveIds)
        {
            if (string.IsNullOrWhiteSpace(saveId))
                continue;

            MarkItemCraftedBySaveId(saveId);
        }
    }

    // 장비 해금 상태 저장을 위해, 현재 해금된 아이템들의 saveId 리스트를 반환
    public List<string> GetUnlockedSaveIds()
    {
        List<string> unlockedIds = new List<string>();

        for (int i = 0; i < m_inventory.Count; i++)
        {
            if (!m_inventory[i].isUnlocked)
                continue;

            if (m_itemDatabase.TryGetValue(i, out GearsetRecipeSO recipe) && recipe != null && !string.IsNullOrWhiteSpace(recipe.saveId))
            {
                unlockedIds.Add(recipe.saveId);
            }
        }

        return unlockedIds;
    }

    /// <summary>
    /// 현재 장착된 아이템의 saveId를 반환합니다. 장착된 아이템이 없거나 saveId가 유효하지 않으면 빈 문자열을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public string GetEquippedSaveId()
    {
        if (m_itemDatabase.TryGetValue(m_partyEquippedItemID, out GearsetRecipeSO recipe) && recipe != null)
            return recipe.saveId;

        return string.Empty;
    }

     /// <summary>
     /// 저장/로드 과정에서 호출할 아이템 해금 상태 적용
     /// </summary>
     /// <param name="unlockedSaveIds"></param>
    public void ApplyUnlockedState(List<string> unlockedSaveIds)
    {
        if (unlockedSaveIds == null)
            return;

        foreach (string saveId in unlockedSaveIds)
        {
            if (string.IsNullOrWhiteSpace(saveId))
                continue;

            UnlockItemBySaveId(saveId);
        }
    }

    /// <summary>
    /// 저장/로드 과정에서 호출할 장착 아이템 적용
    /// </summary>
    /// <param name="saveId"></param>
    /// <returns></returns>
    public bool EquipItemBySaveId(string saveId)
    {
        if (string.IsNullOrWhiteSpace(saveId))
            return false;

        if (!m_itemIdBySaveId.TryGetValue(saveId, out int itemId))
        {
            Debug.LogWarning($"[DataManager] EquipItemBySaveId 실패: saveId '{saveId}'와 매칭되는 아이템이 없습니다.");
            return false;
        }

        EquipItemToParty(itemId);
        return true;
    }

    public void EquipItemToParty(int itemID)
    {
        Debug.Log($"[DataManager] EquipItemToParty 호출됨 / itemID = {itemID}");

        ItemData item = GetItemByID(itemID);
        if (item == null)
        {
            Debug.LogWarning($"[DataManager] 장착 실패: itemID {itemID}를 찾을 수 없습니다.");
            return;
        }

        if (!item.isUnlocked)
        {
            Debug.LogWarning($"[DataManager] 장착 실패: itemID {itemID}는 아직 해금되지 않았습니다.");
            return;
        }

        if (m_itemDatabase.ContainsKey(itemID))
        {
            m_partyEquippedItemID = itemID;
            GearsetRecipeSO equippedData = m_itemDatabase[itemID];

            // 장비 장착 시, 표시명과 비교용 키를 함께 갱신합니다.
            string newTrait = equippedData.traitName;
            string newTraitKey = string.IsNullOrWhiteSpace(equippedData.traitKey)
                ? equippedData.traitName
                : equippedData.traitKey;

            Debug.Log($"[DataManager] 장착 아이템 이름 = {equippedData.gearsetName}");
            Debug.Log($"[DataManager] 장착 아이템 traitName = '{newTrait}'");
            Debug.Log($"[DataManager] 장착 아이템 traitKey = '{newTraitKey}'");
            Debug.Log($"[DataManager] 기존 파티 특성 = '{m_currentPartyTrait}'");
            Debug.Log($"[DataManager] 기존 파티 특성키 = '{m_currentPartyTraitKey}'");

            bool traitChanged = m_currentPartyTraitKey != newTraitKey;
            m_currentPartyTrait = newTrait;
            m_currentPartyTraitKey = newTraitKey;

            Debug.Log($"[DataManager] 변경 후 파티 특성 = '{m_currentPartyTrait}'");
            Debug.Log($"[DataManager] 변경 후 파티 특성키 = '{m_currentPartyTraitKey}'");
            Debug.Log($"[DataManager] traitChanged = {traitChanged}");

            if (traitChanged)
            {
                Debug.Log($"[DataManager] OnPartyTraitChanged 이벤트 호출");
                OnPartyTraitChanged?.Invoke(m_currentPartyTraitKey);
            }

        }
        else
        {
            Debug.LogWarning($"[DataManager] m_itemDatabase에 itemID {itemID}가 없음");
        }
    }

    // --- 퀘스트 시스템에서 호출할 특성 검사 로직 ---

    public string GetCurrentPartyTrait()
    {
        return m_currentPartyTrait;
    }

    public string GetCurrentPartyTraitKey()
    {
        return m_currentPartyTraitKey;
    }

    public bool CanEnterQuest(string requiredTraitKey)
    {
        return string.Equals(
            m_currentPartyTraitKey?.Trim(),
            requiredTraitKey?.Trim(),
            StringComparison.Ordinal);
    }
}
