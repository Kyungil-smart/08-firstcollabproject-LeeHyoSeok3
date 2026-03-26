using System.Text;
using TMPro;
using UnityEngine;

public class MaterialInventoryUI : MonoBehaviour
{
    [SerializeField] private MaterialInventory materialInventory;
    [SerializeField] private TextMeshProUGUI inventoryText;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (materialInventory == null)
        {
            Debug.LogWarning("materialInventory가 연결되지 않았습니다.");
            return;
        }

        if (inventoryText == null)
        {
            Debug.LogWarning("inventoryText가 연결되지 않았습니다.");
            return;
        }

        StringBuilder sb = new StringBuilder();

        foreach (var entry in materialInventory.GetAllMaterials())
        {
            if (entry.Key == null)
                continue;

            sb.AppendLine($"{entry.Key.materialName} : {entry.Value}");
        }

        inventoryText.text = sb.ToString();
    }
}