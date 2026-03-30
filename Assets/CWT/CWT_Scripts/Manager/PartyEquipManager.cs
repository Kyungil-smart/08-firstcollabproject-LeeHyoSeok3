// ============================================
// 파일명: PartyEquipManager.cs
// 붙일 오브젝트: 빈 오브젝트 (씬에 하나만 존재)
// 역할: 파티가 현재 장착 중인 장비의 특성을 관리
//       ★ 임시 스크립트 - 나중에 다른 팀원의 장비 시스템과 교체 예정
// ============================================

using UnityEngine;
using DesignPattern;

public class PartyEquipManager : Singleton<PartyEquipManager>
{
    // ─── 현재 장착 중인 장비 특성 ───
    // Inspector에서 테스트용으로 직접 입력 가능
    // 예: "가벼움", "견고함", "끈적거림", "따뜻함" 등
    [Header("현재 장착 특성 (테스트용, Inspector에서 변경)")]
    [SerializeField] private string _currentAttribute = "가벼움";

    // 외부에서 현재 특성을 읽을 수 있는 프로퍼티
    public string CurrentAttribute => _currentAttribute;

    /// <summary>
    /// 장비 특성 변경 (나중에 장비 시스템 연동 시 호출)
    /// </summary>
    public void SetAttribute(string newAttribute)
    {
        _currentAttribute = newAttribute;
        Debug.Log($"[PartyEquipManager] 장비 특성 변경: {_currentAttribute}");
    }

    /// <summary>
    /// 퀘스트의 요구 특성과 현재 장착 특성이 일치하는지 확인
    /// </summary>
    /// <param name="requiredAttribute">던전이 요구하는 특성</param>
    /// <returns>true면 입장 가능, false면 입장 불가</returns>
    public bool CanEnterDungeon(string requiredAttribute)
    {
        // 요구 특성이 비어있으면 (예: 평원) → 항상 입장 가능
        if (string.IsNullOrEmpty(requiredAttribute))
            return true;

        // 현재 장착 특성과 요구 특성이 같으면 입장 가능
        return _currentAttribute == requiredAttribute;
    }
}