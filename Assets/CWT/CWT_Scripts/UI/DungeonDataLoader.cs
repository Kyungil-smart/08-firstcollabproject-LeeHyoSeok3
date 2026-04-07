using System.Collections.Generic;
using Unity.VisualScripting;
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

    // ★ 추가: 던전 배경 스프라이트 (CSV 순서대로 DG_01~08)
    [Header("던전 배경 (CSV 순서대로)")]
    [SerializeField] private Sprite[] _dungeonBackgrounds;

    // ★ 추가: 전투 중 던전 배경 (몬스터 버전, 아직 없으면 비워두기)
    [Header("전투 중 던전 배경 (CSV 순서대로, 없으면 비워두기)")]
    [SerializeField] private Sprite[] _dungeonBattleBackgrounds;

    [Header("퀘스트 보상 데이터")]
    [SerializeField] private QuestRewardSo[] _questRewards;

    private bool _hasLoadedDungeonData;
    private bool _hasBuiltQuestUI;

    private void Start()
    {
        LoadDungeonData();
    }

    void LoadDungeonData()
    {
        EnsureDungeonDataLoaded();
        EnsureQuestPanelsBuilt();
    }

    public void EnsureDungeonDataLoaded()
    {
        if (_hasLoadedDungeonData)
            return;

        TextAsset csvFile = Resources.Load<TextAsset>("Data/Dungeon_Data");
        if (csvFile == null)
        {
            Debug.LogError("[DungeonDataLoader] Data/Dungeon_Data CSV를 찾을 수 없습니다.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        int dungeonIndex = 0;

        for (int i = 2; i < lines.Length; i++)
        {
            List<string> values = ParseCSVLine(lines[i]);

            if (values.Count < 9 || string.IsNullOrEmpty(values[1].Trim())) continue;

            DungeonData data = new DungeonData
            {
                dungeonName = values[1].Trim(),
                dungeonNameEng = values[2].Trim(),
                description = values[3].Trim(),
                requiredAttribute = values[5].Trim(),
                requiredTraitKey = values[6].Trim(),
                rewardItem = values[7].Trim(),
                timeRequired = values[8].Trim(),

                dungeonIcon = (dungeonIndex < _dungeonIcons.Length) ? _dungeonIcons[dungeonIndex] : null,
                attributeIcon = FindIcon(_traitIcons, values[5].Trim()),
                materialIcon = FindIcon(_materialIcons, values[7].Trim()),

                // ★ 추가: 던전 배경 스프라이트 연결
                dungeonBackground = (dungeonIndex < _dungeonBackgrounds.Length) ?
                    _dungeonBackgrounds[dungeonIndex] : null,

                // ★ 추가: 전투 중 던전 배경 (없으면 null)
                dungeonBattleBackground = (dungeonIndex < _dungeonBattleBackgrounds.Length) ?
                    _dungeonBattleBackgrounds[dungeonIndex] : null,
            };

            QuestRewardSo reward = FindRewardByDungeonName(data.dungeonName);

            if (reward == null)
            {
                Debug.LogWarning($"[Reward] 매칭 실패: {data.dungeonName}");
            }
            else
            {
                Debug.Log($"[Reward] 매칭 성공: {data.dungeonName} / 골드: {reward.gold}");

                if (reward.materials != null)
                {
                    foreach (var mat in reward.materials)
                    {
                        if (mat != null && mat.material != null)
                        {
                            Debug.Log($"[Reward] 재료: {mat.material.GetMaterialName()} x {mat.count}");
                        }
                    }
                }
            }

            dungeonIndex++;
            _dungeonDatalist.Add(data);
        }

        _hasLoadedDungeonData = true;
    }

    private void EnsureQuestPanelsBuilt()
    {
        if (_hasBuiltQuestUI)
            return;

        if (_content == null || _questPanel == null)
            return;

        // 이미 패널이 있으면 중복 생성하지 않음
        if (_content.childCount > 0)
        {
            _hasBuiltQuestUI = true;
            return;
        }

        for (int i = 0; i < _dungeonDatalist.Count; i++)
        {
            DungeonData data = _dungeonDatalist[i];
            if (data == null)
                continue;

            GameObject questItem = Instantiate(_questPanel, _content);
            QuestItemUI ui = questItem.GetComponent<QuestItemUI>();
            ui.SetData(data);
        }

        _hasBuiltQuestUI = true;
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

    private QuestRewardSo FindRewardByDungeonName(string dungeonName)
    {
        if (string.IsNullOrEmpty(dungeonName) || _questRewards == null) return null;

        foreach (var reward in _questRewards)
        {
            if (reward == null) continue;

            if (reward.dungeonName == dungeonName) return reward;
        }
        return null;
    }

    public DungeonData FindDungeonByName(string dungeonName)
    {
        if (string.IsNullOrEmpty(dungeonName)) return null;

        EnsureDungeonDataLoaded();

        foreach (var data in _dungeonDatalist)
        {
            if (data != null && data.dungeonName == dungeonName) return data;
        }
        return null;
    }
}

[System.Serializable]
public class IconEntry
{
    public string name;
    public Sprite sprite;
}
