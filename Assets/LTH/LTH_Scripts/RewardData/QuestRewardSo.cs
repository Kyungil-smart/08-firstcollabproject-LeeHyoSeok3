using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Quest Reward")]
public class QuestRewardSo : ScriptableObject
{
    public string dungeonName;

    public double gold;

    public List<MaterialEntry> materials = new();
}
