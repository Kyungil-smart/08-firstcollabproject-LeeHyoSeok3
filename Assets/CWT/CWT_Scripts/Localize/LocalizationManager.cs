using UnityEngine;
using System;
using System.Collections.Generic;
using DesignPattern;

public class LocalizationManager : Singleton<LocalizationManager>
{
    [SerializeField] private TextAsset csvFile;

    private Language _currentLanguage = Language.Korean;
    public Language CurrentLanguage => _currentLanguage;

    // KEY -> [KOREAN, ENGLISH]
    private Dictionary<string, string[]> _textData = new Dictionary<string, string[]>();

    public event Action OnLanguageChanged;

    protected override void OnAwake()
    {
        LoadCSV();
    }

    private void LoadCSV()
    {
        _textData.Clear();

        if (csvFile == null)
        {
            Debug.LogError("LocalizationManager: CSV 파일이 연결되지 않았습니다.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line))
                continue;

            line = line.Trim();

            // ,,
            if (line == ",," || line == ",")
                continue;

            List<string> values = ParseCSVLine(line);

            // 최소 KEY, KOREAN, ENGLISH 3칸 필요
            if (values.Count < 3)
                continue;

            string key = CleanCSVValue(values[0]);
            string korean = CleanCSVValue(values[1]);
            string english = CleanCSVValue(values[2]);

            // 헤더 스킵
            if (string.Equals(key, "KEY", StringComparison.OrdinalIgnoreCase))
                continue;

            // key 없으면 스킵
            if (string.IsNullOrWhiteSpace(key))
                continue;

            if (_textData.ContainsKey(key))
            {
                Debug.LogWarning($"LocalizationManager: 중복 key 발견 -> {key}");
                continue;
            }

            _textData.Add(key, new string[] { korean, english });
        }

        Debug.Log($"LocalizationManager: {_textData.Count}개 텍스트 로드 완료");
    }

    private List<string> ParseCSVLine(string line)
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
            else if (c == ',' && !insideQuotes)
            {
                result.Add(currentValue);
                currentValue = "";
            }
            else
            {
                currentValue += c;
            }
        }

        result.Add(currentValue);
        return result;
    }

    private string CleanCSVValue(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value
            .Replace("\"", "")
            .Replace("\r", "")
            .Replace("\uFEFF", "") // BOM 제거
            .Trim();
    }

    public string GetText(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogWarning("Localization key is empty");
            return string.Empty;
        }

        key = key.Trim();

        if (_textData.TryGetValue(key, out var values))
        {
            int index = _currentLanguage == Language.Korean ? 0 : 1;
            return values[index];
        }

        Debug.LogWarning($"Localization key not found: {key}");
        return key;
    }

    public void SetLanguage(Language newLanguage)
    {
        if (_currentLanguage == newLanguage)
            return;

        _currentLanguage = newLanguage;
        OnLanguageChanged?.Invoke();

        Debug.Log($"언어 변경: {newLanguage}");
    }

    public void Reload()
    {
        LoadCSV();
        OnLanguageChanged?.Invoke();
    }
}

public enum Language
{
    Korean,
    English
}