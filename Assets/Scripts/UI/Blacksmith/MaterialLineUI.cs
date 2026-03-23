using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialLineUI : MonoBehaviour
{
    [SerializeField] private Image materialIconImage;
    [SerializeField] private TextMeshProUGUI materialCountText;

    public void SetData(string materialName, Sprite materialIcon, int ownedCount, int requiredCount)
    {
        if (materialIconImage != null)
            materialIconImage.sprite = materialIcon;

        materialCountText.text = $"{materialName} {ownedCount}/{requiredCount}";
    }
}