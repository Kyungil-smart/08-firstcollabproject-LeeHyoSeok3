using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int gold;
    public List<MaterialSaveData> materials = new();
    public List<string> craftedGearIds = new();

    public List<UpgradeSaveData> upgrades = new();
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