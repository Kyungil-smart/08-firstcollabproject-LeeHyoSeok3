// ============================================
// 파일명: LocalizedText.cs
// 붙일 오브젝트: 언어 변경이 필요한 TextMeshPro 오브젝트
// 역할: Inspector에서 한국어 키만 적어두면 언어 변경 시 자동으로 텍스트 교체
// 
// 사용법 (코드 작성 필요 없음):
//   1. 텍스트 오브젝트에 이 컴포넌트 붙이기
//   2. Inspector에서 Key 칸에 한국어 텍스트 입력 (예: "강화", "설정")
//   3. 끝! 언어 버튼 누르면 자동으로 바뀜
// ============================================

using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    // Inspector에서 한국어 키를 적어두는 칸
    // System_Data.csv의 KOREAN 컬럼에 있는 텍스트를 입력
    // 예: "강화", "설정", "퀘스트", "대장간" 등
    [SerializeField] private string _key;

    // 이 오브젝트에 붙어있는 텍스트 컴포넌트
    private TextMeshProUGUI _text;

    private void Start()
    {
        // 자기 자신에 붙어있는 TextMeshPro 컴포넌트 가져오기
        _text = GetComponent<TextMeshProUGUI>();

        // LocalizationManager의 언어 변경 이벤트에 등록
        // 언어가 바뀌면 UpdateText가 자동으로 호출됨
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        }

        // 처음 시작할 때도 현재 언어로 텍스트 세팅
        UpdateText();
    }

    // 언어가 변경되면 자동으로 호출되는 메서드
    private void UpdateText()
    {
        if (_text == null || LocalizationManager.Instance == null) return;

        // 키를 넣으면 현재 언어에 맞는 텍스트가 나옴
        _text.text = LocalizationManager.Instance.GetText(_key);
    }

    // 오브젝트가 파괴될 때 이벤트 등록 해제
    // 이걸 안 하면 파괴된 오브젝트를 호출하려다 에러가 남 (메모리 누수 방지)
    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }
}