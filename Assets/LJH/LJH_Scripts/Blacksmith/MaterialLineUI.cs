using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialLineUI : MonoBehaviour
{
    [SerializeField] private Image materialIconImage;
    [SerializeField] private TextMeshProUGUI materialCountText;

    [SerializeField] private Color enoughColor = Color.green;
    [SerializeField] private Color notEnoughColor = Color.red;

    public void SetData(string materialName, Sprite materialIcon, int ownedCount, int requiredCount)
    {
        if (materialIconImage != null)
            materialIconImage.sprite = materialIcon;

        materialCountText.text = $"{materialName} {ownedCount}/{requiredCount}";
        
        if (ownedCount < requiredCount)
            materialCountText.color = notEnoughColor;
        else
            materialCountText.color = enoughColor;
    }
}