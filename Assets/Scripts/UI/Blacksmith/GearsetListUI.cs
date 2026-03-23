using System.Collections.Generic;
using UnityEngine;

public class GearsetListUI : MonoBehaviour
{
    [SerializeField] private CSVLoader csvLoader;
    [SerializeField] private GearsetUI[] slots; // 8칸 고정

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        List<Gearset> dataList = csvLoader.LoadGearset();

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < dataList.Count)
            {
                slots[i].SetData(dataList[i].GearsetName);
                slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].Clear();
                slots[i].gameObject.SetActive(false);
            }
        }
    }
}