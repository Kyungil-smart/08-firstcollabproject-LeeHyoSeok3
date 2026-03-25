using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletedGearsetSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gearsetNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image gearsetIconImage;

    public void SetData(GearsetNameData nameData, GearsetRecipeSO recipe)
    {
        if (nameData != null)
        {
            if (gearsetNameText != null)
                gearsetNameText.text = nameData.gearsetName;

            if (descriptionText != null)
                descriptionText.text = nameData.description;
        }
        
    }
}