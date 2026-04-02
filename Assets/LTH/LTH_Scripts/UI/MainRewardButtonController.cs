using UnityEngine;
using UnityEngine.UI;

public class MainRewardButtonController : MonoBehaviour
{
    [SerializeField] private Button rewardButton;
    [SerializeField] private RewardPopupUI rewardPopupUI;

    private void Awake()
    {
        if (rewardButton != null)
        {
            rewardButton.onClick.AddListener(OnClickRewardButton);
        }
    }

    private void OnDestroy()
    {
        if (rewardButton != null)
        {
            rewardButton.onClick.RemoveListener(OnClickRewardButton);
        }
    }

    private void Update()
    {
        RefreshButtonState();
    }

    private void RefreshButtonState()
    {
        if (rewardButton == null) return;

        bool hasCompletedQuest = false;

        if (QuestManager.Instance != null)
        {
            hasCompletedQuest = QuestManager.Instance.HasCompletedQuest;
        }

        rewardButton.interactable = hasCompletedQuest;
    }

    private void OnClickRewardButton()
    {
        if (QuestManager.Instance == null) return;

        if (!QuestManager.Instance.HasCompletedQuest)
        {
            Debug.LogWarning("[Reward] 완료된 퀘스트 없음");
            return;
        }

        rewardPopupUI?.Open();
    }
}