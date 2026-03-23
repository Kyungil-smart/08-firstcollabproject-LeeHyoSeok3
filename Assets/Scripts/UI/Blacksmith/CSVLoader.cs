using System.Collections.Generic;
using UnityEngine;

public class GearsetNameCSVLoader : MonoBehaviour
{
    [SerializeField] private TextAsset csvFile;

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

            if (values.Length < 3)
                continue;

            GearsetNameData data = new GearsetNameData
            {
                gearsetName = values[2].Trim()
            };

            list.Add(data);
        }

        return list;
    }
}