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
}
