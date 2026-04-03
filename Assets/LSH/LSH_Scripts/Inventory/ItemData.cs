using UnityEngine;

[System.Serializable]
public class ItemData // 💡 struct를 class로 변경합니다!
{
    public int id;
    public string name;
    public string description;
    public Sprite icon;
    public bool isUnlocked;

    // 💡 SO 구조에 맞춘 특성 데이터 추가
    public string traitKey;
    public string traitName;
    public string traitDescription;
    public Sprite traitIcon;
}
