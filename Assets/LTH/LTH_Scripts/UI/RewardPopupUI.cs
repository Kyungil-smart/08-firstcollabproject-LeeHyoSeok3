using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPopupUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform popupRect;
    [SerializeField] private Button claimButton;

    [Header("Gold Reward UI")]
    [SerializeField] private GameObject goldRewardRoot;
    [SerializeField] private Image goldIconImage;
    [SerializeField] private TMP_Text goldAmountText;
    [SerializeField] private Sprite goldIconOverride;

    [Header("Material Reward UI")]
    [SerializeField] private GameObject materialRewardRoot;
    [SerializeField] private Image materialIconImage;
    [SerializeField] private TMP_Text materialAmountText;
    [SerializeField] private TMP_Text materialNameText;

    [Header("Reward Data")]
    [SerializeField] private QuestRewardSo[] questRewards;

    private QuestRewardSo currentReward;

    private void Awake()
    {
        if (claimButton != null)
            claimButton.onClick.AddListener(OnClickClaim);
    }

    private void OnDestroy()
    {
        if (claimButton != null)
            claimButton.onClick.RemoveListener(OnClickClaim);
    }

    public void Open()
    {
        if (QuestManager.Instance == null)
            return;

        DungeonData quest = QuestManager.Instance.CompletedQuest;
        if (quest == null) return;

        currentReward = FindRewardByDungeonName(quest.dungeonName);

        if (currentReward == null) return;

        RefreshUI(currentReward);
        PopupManager.Instance?.OpenRewardPopup();
    }

    private QuestRewardSo FindRewardByDungeonName(string dungeonName)
    {
        if (questRewards == null) return null;

        foreach (QuestRewardSo reward in questRewards)
        {
            if (reward != null && reward.dungeonName == dungeonName)
                return reward;
        }

        return null;
    }

    private void RefreshUI(QuestRewardSo reward)
    {
        RefreshGoldUI(reward);
        RefreshMaterialUI(reward);
    }

    private void RefreshGoldUI(QuestRewardSo reward)
    {
        bool hasGold = reward != null && reward.gold > 0;

        if (goldRewardRoot != null)
            goldRewardRoot.SetActive(hasGold);

        if (!hasGold) return;

        if (goldIconImage != null)
        {
            goldIconImage.sprite = goldIconOverride;
            goldIconImage.enabled = goldIconOverride != null;
        }

        if (goldAmountText != null)
            goldAmountText.text = GoldManager.FormatGold(reward.gold);
    }

    private void RefreshMaterialUI(QuestRewardSo reward)
    {
        MaterialEntry materialReward = GetPrimaryMaterialReward(reward);
        bool hasMaterial = materialReward != null && materialReward.material != null;

        if (materialRewardRoot != null)
            materialRewardRoot.SetActive(hasMaterial);

        if (!hasMaterial) return;

        if (materialIconImage != null)
        {
            materialIconImage.sprite = materialReward.material.icon;
            materialIconImage.enabled = materialReward.material.icon != null;
        }

        if (materialAmountText != null)
            materialAmountText.text = $"x {materialReward.count}";

        if (materialNameText != null)
            materialNameText.text = materialReward.material.GetMaterialName();
    }

    private static MaterialEntry GetPrimaryMaterialReward(QuestRewardSo reward)
    {
        if (reward == null || reward.materials == null)
            return null;

        foreach (MaterialEntry material in reward.materials)
        {
            if (material != null && material.material != null && material.count > 0)
                return material;
        }

        return null;
    }

    private void OnClickClaim()
    {
        if (QuestManager.Instance == null)
            return;

        DungeonData quest = QuestManager.Instance.CompletedQuest;
        if (quest == null) return;

        if (currentReward == null) return;

        if (currentReward.gold > 0)
            GoldManager.Instance?.AddGold(currentReward.gold);

        if (currentReward.materials != null && currentReward.materials.Count > 0)
            MaterialInventory.Instance?.AddMaterials(currentReward.materials);

        QuestManager.Instance.ClearCompletedQuest();

        if (popupRect != null)
            PopupManager.Instance?.ClosePopup(popupRect);

        currentReward = null;
    }
}