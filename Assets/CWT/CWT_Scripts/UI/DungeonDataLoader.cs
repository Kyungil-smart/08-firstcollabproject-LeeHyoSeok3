using System.Collections.Generic;
using UnityEngine;

public class DungeonDataLoader : MonoBehaviour
{
    [SerializeField] List<DungeonData> _dungeonDatalist = new List<DungeonData>();
    [SerializeField] private GameObject _questPanel;
    [SerializeField] private Transform _content;

    [Header("던전 아이콘 (CSV 순서대로)")]
    [SerializeField] private Sprite[] _dungeonIcons;

    [Header("특성 아이콘")]
    [SerializeField] private IconEntry[] _traitIcons;

    [Header("재료 아이콘")]
    [SerializeField] private IconEntry[] _materialIcons;

    private void Start()
    {
        LoadDungeonData();
    }

    void LoadDungeonData()
    {
        // Content 안에 이미 패널이 있으면 실행하지 않음
        if (_content.childCount > 0) return;

        TextAsset csvFile = Resources.Load<TextAsset>("Data/Dungeon_Data");
        string[] lines = csvFile.text.Split('\n');

        int dungeonIndex = 0;

        for (int i = 2; i < lines.Length; i++)
        {
            List<string> values = ParseCSVLine(lines[i]);

            if (values.Count < 7 || string.IsNullOrEmpty(values[1].Trim())) continue;

            DungeonData data = new DungeonData
            {
                dungeonName = values[1].Trim(),
                dungeonNameEng = values[2].Trim(),
                description = values[3].Trim(),
                requiredAttribute = values[5].Trim(),
                rewardItem = values[6].Trim(),
                timeRequired = values[7].Trim(),

                dungeonIcon = (dungeonIndex < _dungeonIcons.Length) ? _dungeonIcons[dungeonIndex] : null,
                attributeIcon = FindIcon(_traitIcons, values[5].Trim()),
                materialIcon = FindIcon(_materialIcons, values[6].Trim()),
            };

            dungeonIndex++;
            _dungeonDatalist.Add(data);

            GameObject _questItem = Instantiate(_questPanel, _content);
            QuestItemUI _ui = _questItem.GetComponent<QuestItemUI>();
            _ui.SetData(data);
        }
    }

    private Sprite FindIcon(IconEntry[] icons, string name)
    {
        if (string.IsNullOrEmpty(name) || icons == null) return null;

        foreach (IconEntry entry in icons)
        {
            if (entry.name == name)
                return entry.sprite;
        }
        return null;
    }

    List<string> ParseCSVLine(string line)
    {
        List<string> result = new List<string>();
        string currentValue = "";
        bool insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                insideQuotes = !insideQuotes;
            }
            else if (c == ',')
            {
                if (insideQuotes)
                {
                    currentValue += c;
                }
                else
                {
                    result.Add(currentValue);
                    currentValue = "";
                }
            }
            else
            {
                currentValue += c;
            }
        }
        result.Add(currentValue);
        return result;
    }
}

[System.Serializable]
public class IconEntry
{
    public string name;
    public Sprite sprite;
}