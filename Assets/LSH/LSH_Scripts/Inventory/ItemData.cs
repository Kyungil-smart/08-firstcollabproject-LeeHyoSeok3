using UnityEngine;

[System.Serializable]
public struct ItemData
{
    public int id;
    public string name;
    public string description;
    public Sprite icon;

    // 💡 새로 추가된 필드: 아이템 해금 여부
    public bool isUnlocked;
}