using UnityEngine;

// 1. 아이템 데이터 구조체 정의 (시스템, 장비 CSV 데이터 기반)
// 인스펙터 창이나 다른 클래스에서 리스트로 쉽게 관리하기 위해 [System.Serializable] 속성을 부여합니다.
[System.Serializable]
public struct ItemData
{
    public int id;              // 아이템 고유 ID (CSV 데이터의 ID와 매칭)
    public string name;         // 아이템 이름
    public string description;  // 아이템 상세 설명
    public Sprite icon;         // UI에 표시될 장비 아이콘 이미지
}