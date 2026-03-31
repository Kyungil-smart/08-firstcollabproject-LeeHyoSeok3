using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 임시 테스트용 스트립트 입니다
/// 퀘스트 완료 후 보상이 제대로 지급이 되는지 테스트하기 위한 코드이며, 추후 PopupManager에서 보상 팝업을 띄우는 코드로 대체될 예정입니다.
/// </summary>
public class MainRewardButtonController : MonoBehaviour
{
    [SerializeField] private Button rewardButton;

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
        if (rewardButton == null)
            return;

        bool hasCompletedQuest = false;

        if (QuestManager.Instance != null)
        {
            hasCompletedQuest = QuestManager.Instance.HasCompletedQuest;
        }

        rewardButton.interactable = hasCompletedQuest;
    }

    private void OnClickRewardButton()
    {
        Debug.Log("[MainRewardButtonController] 보상 획득 버튼 클릭");
    }
}
