using System.Collections.Generic;
using UnityEngine;

public class DungeonDataLoader : MonoBehaviour
{
    [SerializeField] List<DungeonData> _dungeonDatalist = new List<DungeonData>();

    [SerializeField] private GameObject _questPanel;
    [SerializeField] private Transform _content;
    private QuestItemUI _questItemUI;

    private void Start()
    {
        LoadDungeonData();
    }

    void LoadDungeonData()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Data/Dungeon_Data");
        string[] lines = csvFile.text.Split('\n');

        for (int i = 2; i < lines.Length; i++)
        {
            List<string> values = ParseCSVLine(lines[i]);
            DungeonData data = new DungeonData
            {
                dungeonName = values[1],
                dungeonNameEng = values[2],
                description = values[3],
                requiredAttribute = values[5],
                rewardItem = values[6],
                timeRequired = values[7]
            };

            _dungeonDatalist.Add(data);
            GameObject _questItem = Instantiate(_questPanel, _content);
            QuestItemUI _ui = _questItem.GetComponent<QuestItemUI>();
            _ui.SetData(data);
        }

    }
    // csv 파일 내부 큰따옴표 안에 쉼표를 무시하고, 한 줄을 올바르게 쪼개는 함수
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
