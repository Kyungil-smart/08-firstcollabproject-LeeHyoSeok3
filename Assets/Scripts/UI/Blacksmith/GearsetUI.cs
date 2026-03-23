using TMPro;
using UnityEngine;

public class GearsetItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GearsetHoverUI hoverUI;

    public void SetData(string gearsetName, GearsetRecipeSO recipe)
    {
        nameText.text = gearsetName;

        if (hoverUI != null)
            hoverUI.SetData(gearsetName, recipe);
    }

    public void Clear()
    {
        nameText.text = "";

        if (hoverUI != null)
            hoverUI.ClearData();
    }
}