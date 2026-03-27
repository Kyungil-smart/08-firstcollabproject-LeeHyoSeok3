// ============================================
// 파일명: LocalizationManager.cs
// 붙일 오브젝트: 빈 오브젝트 (씬에 하나만 존재)
// 역할: System_Data.csv를 읽어서 언어별 텍스트를 관리하고,
//       언어 변경 시 모든 UI에 알려주는 중앙 관리자
// ============================================

using UnityEngine;
using System;
using System.Collections.Generic;
using DesignPattern;

public class LocalizationManager : Singleton<LocalizationManager>
{
    // Resources/Data 폴더에 있는 System_Data CSV 파일
    // Inspector에서 드래그해서 연결
    [SerializeField] private TextAsset csvFile;

    // 현재 선택된 언어 (기본값: 한국어)
    private Language _currentLanguage = Language.Korean;

    // 현재 언어를 외부에서 읽을 수 있는 프로퍼티
    public Language CurrentLanguage => _currentLanguage;

    // CSV에서 읽어온 데이터를 저장하는 딕셔너리
    // 키(한국어 텍스트) → [한국어, 영어] 배열
    private Dictionary<string, string[]> _textData = new Dictionary<string, string[]>();

    // 언어가 변경될 때 발생하는 이벤트
    // 다른 스크립트에서 이 이벤트에 등록해두면 언어 변경 시 자동으로 호출됨
    public event Action OnLanguageChanged;

    protected override void OnAwake()
    {
        LoadCSV();
    }

    // CSV 파일을 읽어서 딕셔너리에 저장하는 메서드
    // System_Data.csv 구조: (빈칸), KOREAN, ENGLISH, (빈칸)
    private void LoadCSV()
    {
        if (csvFile == null)
        {
            Debug.LogError("LocalizationManager: CSV 파일이 연결되지 않았습니다.");
            return;
        }

        // CSV 텍스트를 줄 단위로 분리
        string[] lines = csvFile.text.Split('\n');

        // 첫 줄(,,,)과 둘째 줄(헤더)은 건너뛰고, 셋째 줄부터 데이터 읽기
        for (int i = 2; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            // 빈 줄이면 건너뛰기
            if (string.IsNullOrEmpty(line)) continue;

            // 쉼표로 분리
            string[] values = line.Split(',');

            // 최소 3개 컬럼이 있어야 함 (빈칸, KOREAN, ENGLISH)
            if (values.Length < 3) continue;

            // 컬럼 1 = 한국어 텍스트 (이게 키 역할도 함)
            string korean = values[1].Trim();
            // 컬럼 2 = 영어 텍스트
            string english = values[2].Trim();

            // 키가 비어있으면 건너뛰기
            if (string.IsNullOrEmpty(korean)) continue;

            // 딕셔너리에 저장 (중복 키 방지)
            if (!_textData.ContainsKey(korean))
            {
                // 인덱스 0 = 한국어, 인덱스 1 = 영어
                _textData[korean] = new string[] { korean, english };
            }
        }

        Debug.Log($"LocalizationManager: {_textData.Count}개 텍스트 로드 완료");
    }

    // 한국어 키를 넣으면 현재 언어에 맞는 텍스트를 돌려주는 메서드
    // 사용법: LocalizationManager.Instance.GetText("설정") 
    //   → 한국어일 때: "설정"
    //   → 영어일 때: "Settings"
    public string GetText(string koreanKey)
    {
        if (_textData.ContainsKey(koreanKey))
        {
            // Korean = 인덱스 0, English = 인덱스 1
            int index = _currentLanguage == Language.Korean ? 0 : 1;
            return _textData[koreanKey][index];
        }

        // 키를 못 찾으면 키 자체를 그대로 반환
        return koreanKey;
    }

    // 언어를 변경하는 메서드
    public void SetLanguage(Language newLanguage)
    {
        // 이미 같은 언어면 아무것도 안 함
        if (_currentLanguage == newLanguage) return;

        _currentLanguage = newLanguage;

        // 등록된 모든 리스너에게 언어 변경 알림
        // 이걸 받은 UI들이 자기 텍스트를 자동으로 갱신함
        OnLanguageChanged?.Invoke();

        Debug.Log($"언어 변경: {newLanguage}");
    }
}

// 언어 종류를 나타내는 enum
public enum Language
{
    Korean,
    English
}