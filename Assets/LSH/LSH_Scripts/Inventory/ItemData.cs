using UnityEngine;

[System.Serializable]
public class ItemData
{
    public int id;

    // 장비
    public string nameKey;
    public string descriptionKey;
    public Sprite icon;

    // 상태
    public bool isCrafted;
    public bool isUnlocked;

    // 특성
    public string traitKey;
    public string traitNameKey;
    public string traitDescriptionKey;
    public Sprite traitIcon;
}