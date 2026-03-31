using UnityEngine;
using System.Collections.Generic;

// 게임 내 장비 레시피(SO) 데이터를 불러와 관리하는 클래스입니다.
public class DataManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    // 💡 유니티 Inspector에서 'GearsetRecipeDatabase.asset'을 끌어다 넣을 변수입니다.
    [SerializeField] private GearsetRecipeDatabaseSO m_recipeDatabase;

    // SO 원본 데이터를 보관하는 딕셔너리
    private Dictionary<int, GearsetRecipeSO> m_itemDatabase = new Dictionary<int, GearsetRecipeSO>();
    // 기존 UI 시스템(View)에 넘겨주기 위한 매핑 데이터
    private List<ItemData> m_inventory = new List<ItemData>();

    private int m_partyEquippedItemID = -1;

    private void Awake()
    {
        LoadScriptableObjectData();
    }

    private void LoadScriptableObjectData()
    {
        if (m_recipeDatabase == null) return;

        for (int i = 0; i < m_recipeDatabase.recipes.Count; i++)
        {
            GearsetRecipeSO recipe = m_recipeDatabase.recipes[i];
            if (recipe == null) continue;

            int itemID = i;
            m_itemDatabase.Add(itemID, recipe);

            // 💡 핵심 로직: saveId가 "Rusty"일 때만 true, 나머지는 false로 설정합니다.
            bool isItemUnlocked = (recipe.saveId == "Rusty");

            ItemData newItem = new ItemData
            {
                id = itemID,
                name = recipe.gearsetName,
                description = recipe.description,
                icon = Resources.Load<Sprite>($"Icons/{recipe.saveId}"),
                isUnlocked = isItemUnlocked // 추가된 필드에 데이터 반영
            };

            m_inventory.Add(newItem);
        }
        Debug.Log($"[DataManager] 총 {m_inventory.Count}개의 장비 데이터를 성공적으로 불러왔습니다!");
    }

    // Controller가 팝업을 열 때 인벤토리 목록을 가져가기 위한 메서드
    public List<ItemData> GetInventoryItems()
    {
        return m_inventory;
    }

    // 특정 아이템의 ID로 상세 정보를 검색하여 ItemData 형태로 반환하는 메서드
    public ItemData GetItemByID(int id)
    {
        if (m_itemDatabase.TryGetValue(id, out GearsetRecipeSO recipe))
        {
            return new ItemData
            {
                id = id,
                name = recipe.gearsetName,
                description = recipe.description,
                icon = Resources.Load<Sprite>($"Icons/{recipe.saveId}")
            };
        }
        return default;
    }

    // 유저가 상세 팝업에서 '장착' 버튼을 눌렀을 때 호출되는 메서드
    public void EquipItemToParty(int itemID)
    {
        if (m_itemDatabase.ContainsKey(itemID))
        {
            m_partyEquippedItemID = itemID;
            GearsetRecipeSO equippedRecipe = m_itemDatabase[itemID];

            Debug.Log($"[DataManager] 장착 완료! 파티에 '{equippedRecipe.gearsetName}' 세트가 적용되었습니다. (SaveID: {equippedRecipe.saveId})");

            // 추후 개발 단계: 
            // 여기에 기획서에 있던 '요구 특성 검사' 및 '캐릭터 무기 외형 변경(예: 얼음 칼, 젤리 지팡이)' 로직을 연결하면 됩니다.
        }
    }
}