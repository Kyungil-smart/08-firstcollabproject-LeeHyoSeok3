using System.Collections.Generic;
using UnityEngine;

public class GearsetNameCSVLoader : MonoBehaviour
{
    [SerializeField] private TextAsset csvFile;

    private List<GearsetNameData> gearsetList = new List<GearsetNameData>();

    private void Awake()
    {
        gearsetList = LoadGearsetNames();
    }

    public List<GearsetNameData> LoadGearsetNames()
    {
        List<GearsetNameData> list = new List<GearsetNameData>();

        if (csvFile == null)
        {
            Debug.LogError("GearsetNameCSVLoader: csvFile이 연결되지 않았습니다.");
            return list;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = lines[i].Split(',');

            if (values.Length < 4)
                continue;

            GearsetNameData data = new GearsetNameData
            {
                gearsetName = values[2].Trim(),
                description = values[4].Trim(),
            };

            list.Add(data);
        }

        return list;
    }

    public GearsetNameData GetByName(string gearsetName)
    {
        string target = gearsetName.Trim().ToLower();

        foreach (var data in gearsetList)
        {
            if (data.gearsetName.Trim().ToLower() == target)
                return data;
        }

        Debug.LogWarning($"기어셋 데이터를 찾지 못했습니다: {gearsetName}");
        return null;
    }
}