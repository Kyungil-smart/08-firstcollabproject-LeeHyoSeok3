using UnityEngine;

[System.Serializable]
public class DungeonData
{
    public string dungeonName;       // 던전 이름 (평원, 산, 동굴...)
    public string dungeonNameEng;    // 영문 이름 (Fields, Mountain...)
    public string description;       // 던전 설명
    public string requiredAttribute; // 요구 특성 (가벼움, 견고함...)
    public string rewardItem;        // 획득 재료 (단단한 나뭇가지...)
    public string timeRequired;      // 소요 시간 (10분, 20분...)

    // ─── 아이콘 스프라이트 (자동으로 로드됨) ───
    public Sprite dungeonIcon;       // 던전 아이콘 (평원, 산, 동굴 그림)
    public Sprite attributeIcon;     // 특성 아이콘 (가벼움, 견고함 그림)
    public Sprite materialIcon;      // 재료 아이콘 (나뭇가지, 철 원석 그림)
}
