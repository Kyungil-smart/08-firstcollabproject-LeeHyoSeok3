using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GearsetTooltipUI : MonoBehaviour
{
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Transform materialContent;
    [SerializeField] private GameObject materialLinePrefab;
    [SerializeField] private Vector2 offset = new Vector2(20f, -20f);

    [SerializeField] private RectTransform bgRect;
    [SerializeField] private RectTransform materialContentRect;

    [SerializeField] private float defaultBgHeight = 40f;
    [SerializeField] private float lineHeight = 30f;
    [SerializeField] private float lineSpacing = 2f;
    [SerializeField] private float bgPadding = 20f;

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
        if (recipe == null)
            return;

        isShowing = false;
        gameObject.SetActive(false);

        ClearMaterialLines();
        ResizeBgByRecipe(recipe);

        for (int i = 0; i < recipe.requiredMaterials.Count; i++)
        {
            var materialData = recipe.requiredMaterials[i];

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

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(materialContentRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(bgRect);

        tooltipRect.position = position + offset;
        isShowing = true;
        gameObject.SetActive(true);
    }

    public bool CanCraft(GearsetRecipeSO recipe)
    {
        if (recipe == null)
            return false;

        for (int i = 0; i < recipe.requiredMaterials.Count; i++)
        {
            var materialData = recipe.requiredMaterials[i];
            int ownedCount = GetOwnedMaterialCount(materialData.materialName);

            if (ownedCount < materialData.requiredCount)
                return false;
        }

        return true;
    }

    public void HideTooltip()
    {
        ResetBgHeight();
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
        return 1;
    }

    private void ResizeBgByRecipe(GearsetRecipeSO recipe)
    {
        int materialCount = recipe.requiredMaterials.Count;

        float contentHeight = 0f;

        if (materialCount > 0)
        {
            contentHeight = (materialCount * lineHeight) + ((materialCount - 1) * lineSpacing);
        }

        float finalHeight = defaultBgHeight + bgPadding + contentHeight;
        bgRect.sizeDelta = new Vector2(bgRect.sizeDelta.x, finalHeight);
    }

    public void ResetBgHeight()
    {
        bgRect.sizeDelta = new Vector2(bgRect.sizeDelta.x, defaultBgHeight);
    }
}