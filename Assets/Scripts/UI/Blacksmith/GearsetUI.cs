using TMPro;
using UnityEngine;

public class GearsetUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    public void SetData(string gearsetName)
    {
        nameText.text = gearsetName;
    }
    
    public void Clear()
    {
        nameText.text = "";
    }
}
