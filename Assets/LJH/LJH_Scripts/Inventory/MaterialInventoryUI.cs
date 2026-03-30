using System.Text;
using TMPro;
using UnityEngine;

public class MaterialInventoryUI : MonoBehaviour
{
    [SerializeField] private MaterialInventory materialInventory;
    [SerializeField] private TextMeshProUGUI inventoryText;

    private void Awake()
    {
        TryBindInventory();
    }

    private void OnEnable()
    {
        TryBindInventory();

        if (GameDataController.Instance != null && GameDataController.Instance.IsLoaded)
            RefreshUI();
        else
            GameDataController.OnGameLoaded += OnGameLoaded_Handler;
    }

    private void OnDisable()
    {
        GameDataController.OnGameLoaded -= OnGameLoaded_Handler;
    }

    private void OnGameLoaded_Handler()
    {
        GameDataController.OnGameLoaded -= OnGameLoaded_Handler;
        RefreshUI();
    }

    private void TryBindInventory()
    {
        if (materialInventory == null)
            materialInventory = MaterialInventory.Instance;
    }

    public void RefreshUI()
    {
        TryBindInventory();

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