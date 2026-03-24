using System.Collections.Generic;
using UnityEngine;

public class GearsetListUI : MonoBehaviour
{
    [SerializeField] private GearsetNameCSVLoader csvLoader;
    [SerializeField] private GearsetRecipeDatabaseSO recipeDatabase;
    [SerializeField] private GearsetItemUI[] slots;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (csvLoader == null)
        {
            Debug.LogError("GearsetListUI: csvLoader가 연결되지 않았습니다.");
            return;
        }

        if (recipeDatabase == null)
        {
            Debug.LogError("GearsetListUI: recipeDatabase가 연결되지 않았습니다.");
            return;
        }

        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("GearsetListUI: slots가 비어 있습니다.");
            return;
        }

        List<GearsetNameData> nameList = csvLoader.LoadGearsetNames();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            if (i < nameList.Count)
            {
                string gearsetName = nameList[i].gearsetName;
                GearsetRecipeSO recipe = recipeDatabase.GetRecipeByName(gearsetName);

                slots[i].gameObject.SetActive(true);
                slots[i].SetData(gearsetName, recipe);
            }
            else
            {
                slots[i].Clear();
                slots[i].gameObject.SetActive(false);
            }
        }
    }
}