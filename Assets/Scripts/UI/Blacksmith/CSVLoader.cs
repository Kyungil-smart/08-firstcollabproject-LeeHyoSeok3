using System.Collections.Generic;
using UnityEngine;

public class CSVLoader : MonoBehaviour
{
    [SerializeField] private TextAsset csvFile;

    public List<Gearset> LoadGearset()
    {
        List<Gearset> equipmentList = new List<Gearset>();

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = lines[i].Split(',');

            if (values.Length < 4)
                continue;

            Gearset data = new Gearset
            {
                GearsetName = values[2].Trim()
            };

            equipmentList.Add(data);
        }

        return equipmentList;
    }
}