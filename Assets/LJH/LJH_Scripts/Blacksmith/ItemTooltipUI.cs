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

    private GearsetRecipeSO currentRecipe;
    private Vector2 currentPosition;
    
    private bool isShowing;
    
    private void Awake()
    {
        TryBindInventory();
    }
    
    private void Update()
    {
        if (isShowing && Mouse.current != null)
        {
            tooltipRect.position = Mouse.current.position.ReadValue() + offset;
        }
    }

    private void OnEnable()
    {
        TryBindInventory();

        if (materialInventory != null)
            materialInventory.OnInventoryChanged += RefreshTooltip;
    }

    private void OnDisable()
    {
        if (materialInventory != null)
            materialInventory.OnInventoryChanged -= RefreshTooltip;
    }

    private void TryBindInventory()
    {
        if (materialInventory == null)
            materialInventory = MaterialInventory.Instance;
    }
    
    public void ShowTooltip(GearsetRecipeSO recipe, Vector2 position)
    {
        currentRecipe = recipe;
        currentPosition = position;

        DrawTooltip(recipe, position);
    }

    private void DrawTooltip(GearsetRecipeSO recipe, Vector2 position)
    {
        isShowing = false;
        gameObject.SetActive(false);

        ClearMaterialLines();
        ResizeBgByRecipe(recipe);

        if (titleText != null)
            titleText.text = recipe.gearsetName;

        for (int i = 0; i < recipe.requirements.Count; i++)
        {
            var requirement = recipe.requirements[i];

            GameObject lineObj = Instantiate(materialLinePrefab, materialContent);
            MaterialLineUI lineUI = lineObj.GetComponent<MaterialLineUI>();

            int ownedCount = GetOwnedMaterialCount(requirement.material);

            lineUI.SetData(
                requirement.material.materialName,
                requirement.material.icon,
                ownedCount,
                requirement.requiredCount
            );
        }

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
    
    private void RefreshTooltip()
    {
        if (!isShowing || currentRecipe == null)
            return;

        DrawTooltip(currentRecipe, currentPosition);
    }
}