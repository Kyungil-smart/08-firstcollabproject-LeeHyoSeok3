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

    [SerializeField] private MaterialInventory materialInventory;

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

        if (titleText != null)
            titleText.text = recipe.gearsetName;

        Debug.Log($"툴팁 열기: {recipe.gearsetName}");
        Debug.Log($"requirements 개수: {recipe.requirements.Count}");

        for (int i = 0; i < recipe.requirements.Count; i++)
        {
            var requirement = recipe.requirements[i];

            if (requirement == null)
            {
                Debug.LogError($"requirements[{i}]가 null");
                continue;
            }

            if (requirement.material == null)
            {
                Debug.LogError($"requirements[{i}].material이 null");
                continue;
            }

            if (materialLinePrefab == null)
            {
                Debug.LogError("materialLinePrefab이 연결되지 않음");
                return;
            }

            if (materialContent == null)
            {
                Debug.LogError("materialContent가 연결되지 않음");
                return;
            }

            GameObject lineObj = Instantiate(materialLinePrefab, materialContent);
            MaterialLineUI lineUI = lineObj.GetComponent<MaterialLineUI>();

            if (lineUI == null)
            {
                Debug.LogError("materialLinePrefab에 MaterialLineUI가 없음");
                continue;
            }

            int ownedCount = GetOwnedMaterialCount(requirement.material);

            lineUI.SetData(
                requirement.material.materialName,
                requirement.material.icon,
                ownedCount,
                requirement.requiredCount
            );
        }

        Canvas.ForceUpdateCanvases();

        if (materialContentRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(materialContentRect);

        if (bgRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(bgRect);

        tooltipRect.position = position + offset;
        isShowing = true;
        gameObject.SetActive(true);
    }

    public bool CanCraft(GearsetRecipeSO recipe)
    {
        if (recipe == null || materialInventory == null)
            return false;

        return materialInventory.CanCraft(recipe);
    }

    public void HideTooltip()
    {
        ResetBgHeight();
        isShowing = false;
        gameObject.SetActive(false);
    }

    private void ClearMaterialLines()
    {
        if (materialContent == null)
            return;

        for (int i = materialContent.childCount - 1; i >= 0; i--)
        {
            Destroy(materialContent.GetChild(i).gameObject);
        }
    }

    private int GetOwnedMaterialCount(MaterialDataSO material)
    {
        if (materialInventory == null)
        {
            Debug.LogError("materialInventory가 연결되지 않았습니다.");
            return 0;
        }

        return materialInventory.GetCount(material);
    }

    private void ResizeBgByRecipe(GearsetRecipeSO recipe)
    {
        if (bgRect == null || recipe == null)
            return;

        int materialCount = recipe.requirements.Count;

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
        if (bgRect == null)
            return;

        bgRect.sizeDelta = new Vector2(bgRect.sizeDelta.x, defaultBgHeight);
    }
}