using UnityEngine;

[CreateAssetMenu(menuName = "Game/Material Data")]
public class MaterialDataSO : ScriptableObject
{
    public string saveId;
    public string materialNameKey;
    public Sprite icon;

    public string GetMaterialName()
    {
        if (string.IsNullOrEmpty(materialNameKey))
            return string.Empty;

        if (LocalizationManager.Instance == null)
            return materialNameKey;

        return LocalizationManager.Instance.GetText(materialNameKey);
    }
}