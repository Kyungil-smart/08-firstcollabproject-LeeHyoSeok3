using System;

[Serializable]
public class UpgradeRow
{
    public int level;
    public int value;
    public int cost;
    public int stageDisplay;

    // CSV에서 읽어올 아이콘 키
    public string iconKey;
}