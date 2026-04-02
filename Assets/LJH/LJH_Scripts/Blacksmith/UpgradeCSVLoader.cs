using System.Collections.Generic;
using UnityEngine;

public static class UpgradeCSVLoader
{
    public static List<UpgradeRow> Load(TextAsset csvFile, int startColumnIndex)
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV file is not assigned.");
            return null;
        }

        List<UpgradeRow> rows = new List<UpgradeRow>();
        string[] lines = csvFile.text.Split('\n');

        if (lines.Length <= 1)
        {
            Debug.LogError("CSV has no data.");
            return rows;
        }

        for (int i = 1; i < lines.Length; i++) // header 제외
        {
            string line = lines[i].Trim();

            if (string.IsNullOrEmpty(line))
                continue;

            string[] values = line.Split(',');

            // level, value, cost, stageDisplay, iconKey
            if (values.Length <= startColumnIndex + 4)
            {
                Debug.LogWarning($"Line {i + 1} does not have enough columns: {line}");
                continue;
            }

            UpgradeRow row = new UpgradeRow();

            if (!int.TryParse(values[startColumnIndex].Trim(), out row.level))
            {
                Debug.LogWarning($"Level parse failed at line {i + 1}: {line}");
                continue;
            }

            if (!int.TryParse(values[startColumnIndex + 1].Trim(), out row.value))
            {
                Debug.LogWarning($"Value parse failed at line {i + 1}: {line}");
                continue;
            }

            if (!int.TryParse(values[startColumnIndex + 2].Trim(), out row.cost))
            {
                Debug.LogWarning($"Cost parse failed at line {i + 1}: {line}");
                continue;
            }

            if (!int.TryParse(values[startColumnIndex + 3].Trim(), out row.stageDisplay))
            {
                Debug.LogWarning($"Stage display parse failed at line {i + 1}: {line}");
                continue;
            }

            row.iconKey = values[startColumnIndex + 4].Trim().Replace("\"", "");

            rows.Add(row);
        }

        Debug.Log($"CSV load complete. StartColumn={startColumnIndex}, RowCount={rows.Count}");
        return rows;
    }
}