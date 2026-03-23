using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GearsetTooltipUI : MonoBehaviour
{
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Transform materialContent;
    [SerializeField] private GameObject materialLinePrefab;
    [SerializeField] private Vector2 offset = new Vector2(20f, -20f);

    private bool isShowing;

    private void Update()
    {
        if (isShowing && Mouse.current != null)
        {
            tooltipRect.position = Mouse.current.position.ReadValue() + offset;
        }
    }

    public void ShowTooltip(GearsetRecipeSO recipe, Vector2 position)
    {
        if (recipe == null) return;

        gameObject.SetActive(true);
        isShowing = true;

        titleText.text = recipe.gearsetName;

        ClearMaterialLines();

        for (int i = 0; i < recipe.requiredMaterials.Count; i++)
        {
            GearsetMaterialData materialData = recipe.requiredMaterials[i];

            GameObject lineObj = Instantiate(materialLinePrefab, materialContent);
            MaterialLineUI lineUI = lineObj.GetComponent<MaterialLineUI>();

            int ownedCount = GetOwnedMaterialCount(materialData.materialName);

            lineUI.SetData(
                materialData.materialName,
                materialData.materialIcon,
                ownedCount,
                materialData.requiredCount
            );
        }

        tooltipRect.position = position + offset;
    }

    public void HideTooltip()
    {
        isShowing = false;
        gameObject.SetActive(false);
    }

    private void ClearMaterialLines()
    {
        for (int i = materialContent.childCount - 1; i >= 0; i--)
        {
            Destroy(materialContent.GetChild(i).gameObject);
        }
    }

    private int GetOwnedMaterialCount(string materialName)
    {
        // 나중에 인벤토리와 연결
        return 0;
    }
}