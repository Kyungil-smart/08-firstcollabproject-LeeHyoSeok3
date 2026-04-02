using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private GearsetRecipeDatabaseSO m_recipeDatabase;

    private Dictionary<int, GearsetRecipeSO> m_itemDatabase = new Dictionary<int, GearsetRecipeSO>();
    private List<ItemData> m_inventory = new List<ItemData>();

    private int m_partyEquippedItemID = 0;

    // 현재 파티 특성을 저장할 변수 (초기값은 특성 없음)
    private string m_currentPartyTrait = "None";

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

            // 박사님이 작성하신 SO 변수명에 맞춰 매핑합니다.
            ItemData newItem = new ItemData
            {
                id = i,
                name = recipe.gearsetName,
                description = recipe.gearDescription,
                icon = recipe.gearIcon,
                isUnlocked = (recipe.saveId == "Rusty"),

                // 특성 데이터 매핑
                traitName = recipe.traitName,
                traitDescription = recipe.traitDescription,
                traitIcon = recipe.traitIcon
            };

            m_inventory.Add(newItem);
        }
    }

    public List<ItemData> GetInventoryItems() => m_inventory;
    public int GetEquippedItemID() => m_partyEquippedItemID;

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
                m_inventory[i].isUnlocked = true;
                break;
            }
        }
    }

    public void EquipItemToParty(int itemID)
    {
        if (m_itemDatabase.ContainsKey(itemID))
        {
            m_partyEquippedItemID = itemID;
            GearsetRecipeSO equippedData = m_itemDatabase[itemID];

            // 장비 장착 시, 파티의 현재 특성 이름도 함께 갱신됩니다!
            m_currentPartyTrait = equippedData.traitName;

            Debug.Log($"[DataManager] '{equippedData.gearsetName}' 장착 완료! 현재 파티 특성: [{m_currentPartyTrait}]");
        }
    }

    // --- 퀘스트 시스템에서 호출할 특성 검사 로직 ---

    public string GetCurrentPartyTrait()
    {
        return m_currentPartyTrait;
    }

    public bool CanEnterQuest(string requiredTrait)
    {
        // 문자열 기반으로 요구 특성과 장착 특성이 일치하는지 검사합니다.
        return m_currentPartyTrait == requiredTrait;
    }
}