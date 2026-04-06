using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GearsetSlotUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private GearsetRecipeSO recipe;

    [Header("References")]
    [SerializeField] private GearsetCraftPopupUI popupUI;
    [SerializeField] private MaterialInventory materialInventory;
    [SerializeField] private GearsetInventory gearsetInventory;
    [SerializeField] private Button slotButton;

    [Header("Slot UI")]
    [SerializeField] private Image backgroundImage;   // 배경
    [SerializeField] private Image iconImage;         // 장비 아이콘
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Tooltip")]
    [SerializeField] private TooltipHoverUI tooltipHoverUI;

    [Header("제작 상태 색상")]
    [SerializeField] private Color craftedColor = Color.white;
    [SerializeField] private Color notCraftedColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    private void Awake()
    {
        if (materialInventory == null)
            materialInventory = MaterialInventory.Instance;

        if (gearsetInventory == null)
            gearsetInventory = GearsetInventory.Instance;
    }

    private void Start()
    {
        RefreshSlotInfo();
        RefreshState();

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= RefreshSlotInfo;
            LocalizationManager.Instance.OnLanguageChanged += RefreshSlotInfo;
        }
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshSlotInfo;
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshSlotInfo;
    }

    public void OnClickSlot()
    {
        Debug.Log("슬롯 클릭됨");

        if (recipe == null)
        {
            Debug.LogWarning("recipe가 연결되지 않았습니다.");
            return;
        }

        if (gearsetInventory != null && gearsetInventory.IsCrafted(recipe))
        {
            Debug.Log("이미 제작 완료된 장비입니다.");
            return;
        }

        if (materialInventory == null)
        {
            Debug.LogWarning("materialInventory가 연결되지 않았습니다.");
            return;
        }

        if (popupUI == null)
        {
            Debug.LogWarning("popupUI가 연결되지 않았습니다.");
            return;
        }

        if (!materialInventory.CanCraft(recipe))
        {
            Debug.Log("재료 부족 -> 팝업 열리지 않음");
            return;
        }

        popupUI.Show(recipe, materialInventory, this);
    }

    public void MarkCrafted()
    {
        if (gearsetInventory != null)
            gearsetInventory.AddCrafted(recipe);

        RefreshState();
    }

    public void RefreshState()
    {
        bool isCrafted = gearsetInventory != null && gearsetInventory.IsCrafted(recipe);

        Color targetColor = isCrafted ? craftedColor : notCraftedColor;

        if (backgroundImage != null)
            backgroundImage.color = targetColor;

        if (iconImage != null)
            iconImage.color = targetColor;

        if (slotButton != null)
            slotButton.interactable = true;
    }

    public void RefreshSlotInfo()
    {
        if (recipe == null)
            return;

        if (nameText != null)
            nameText.text = recipe.GetGearsetName();

        if (iconImage != null && recipe.gearIcon != null)
            iconImage.sprite = recipe.gearIcon;

        // ⭐ 여기서 바로 세팅
        if (tooltipHoverUI != null)
            tooltipHoverUI.SetTooltip(recipe.GetGearDescription());
    }

    public GearsetRecipeSO GetRecipe()
    {
        return recipe;
    }
    
    public bool IsCrafted()
    {
        return gearsetInventory != null && gearsetInventory.IsCrafted(recipe);
    }
}