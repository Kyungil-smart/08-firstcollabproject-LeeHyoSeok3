using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public double gold;
    public List<MaterialSaveData> materials = new();
    public List<string> craftedGearIds = new();
    public List<string> unlockedGearIds = new();
    public string equippedGearId;
    public List<UpgradeSaveData> upgrades = new();

      // 퀘스트 오프라인 보상을 위한 저장 데이터
    public bool isQuestActive;
    public string currentQuestDungeonName;
    public string questStartTime;
    public string questEndTime;

    public bool hasCompletedQuest;
    public string completedQuestDungeonName;
}

[Serializable]
public class MaterialSaveData
{
    public string materialId;
    public int count;
}

[Serializable]
public class UpgradeSaveData
{
    public string upgradeId;
    public int level;
}