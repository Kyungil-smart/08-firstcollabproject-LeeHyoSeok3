using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPopupUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform popupRect;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Button claimButton;

    [Header("Reward Data")]
    [SerializeField] private QuestRewardSo[] questRewards;

    private QuestRewardSo currentReward;


    private void Awake()
    {
        if (claimButton != null) claimButton.onClick.AddListener(OnClickClaim);
    }

    private void OnDestroy()
    {
        if (claimButton != null) claimButton.onClick.RemoveListener(OnClickClaim);
    }

    /// <summary>
    /// 메인 보상 버튼에서 호출
    /// </summary>
    public void Open()
    {
        if (QuestManager.Instance == null) return;

        var quest = QuestManager.Instance.CompletedQuest;

        if (quest == null)
        {
            Debug.LogWarning("[RewardPopupUI] 완료된 퀘스트 없음");
            return;
        }

        currentReward = FindRewardByDungeonName(quest.dungeonName);

        if (currentReward == null)
        {
            Debug.LogWarning($"[RewardPopupUI] 보상 데이터 없음: {quest.dungeonName}");
            return;
        }

        RefreshUI(quest, currentReward);

        PopupManager.Instance?.OpenRewardPopup();
    }

    private QuestRewardSo FindRewardByDungeonName(string dungeonName)
    {
        if (questRewards == null) return null;

        foreach (var reward in questRewards)
        {
            if (reward != null && reward.dungeonName == dungeonName)
                return reward;
        }

        return null;
    }

    private void RefreshUI(DungeonData quest, QuestRewardSo reward)
    {
        if (rewardText == null) return;

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"퀘스트: {quest.dungeonName}");
        sb.AppendLine();
        sb.AppendLine($"골드: {reward.gold}");

        if (reward.materials != null && reward.materials.Count > 0)
        {
            sb.AppendLine("재료:");

            foreach (var mat in reward.materials)
            {
                if (mat == null || mat.material == null) continue;
                sb.AppendLine($"- {mat.material.materialName} x {mat.count}");
            }
        }

        rewardText.text = sb.ToString();
    }

    private void OnClickClaim()
    {
        if (QuestManager.Instance == null)
            return;

        var quest = QuestManager.Instance.CompletedQuest;

        if (quest == null)
        {
            Debug.LogWarning("[RewardPopupUI] 완료된 퀘스트 없음");
            return;
        }

        if (currentReward == null)
        {
            Debug.LogWarning("[RewardPopupUI] 현재 보상 데이터가 비어 있음");
            return;
        }

        // 골드 지급
        if (currentReward.gold > 0)
        {
            GoldManager.Instance?.AddGold(currentReward.gold);
        }

        // 재료 지급
        if (currentReward.materials != null && currentReward.materials.Count > 0)
        {
            MaterialInventory.Instance?.AddMaterials(currentReward.materials);
        }

        Debug.Log("[RewardPopupUI] 보상 지급 완료");

        // 완료 퀘스트 초기화
        QuestManager.Instance.ClearCompletedQuest();

        // 팝업 닫기
        if (popupRect != null)
            PopupManager.Instance?.ClosePopup(popupRect);

        currentReward = null;
    }
}