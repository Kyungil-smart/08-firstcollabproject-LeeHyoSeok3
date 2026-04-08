using UnityEngine;
using UnityEngine.UI;

public class WorldMapDoorButton : MonoBehaviour
{
    [SerializeField] private Button doorButton;
    [SerializeField] private Image doorButtonImage;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite openedSprite;

    private void Start()
    {
        RefreshDoorState();
    }

    private void Update()
    {
        RefreshDoorState();
    }

    private void RefreshDoorState()
    {
        if (doorButton == null) return;

        doorButton.interactable = true;

        if (QuestManager.Instance == null)
        {
            ApplyDoorSprite(defaultSprite);
            return;
        }

        bool canOpenWorldMap = QuestManager.Instance.IsQuestActive;

        var state = ScreenStateManager.Instance != null
            ? ScreenStateManager.Instance.CurrentState
            : ScreenStateManager.ScreenState.Main;

        if (state == ScreenStateManager.ScreenState.Main ||
            state == ScreenStateManager.ScreenState.Minimized)
        {
            ApplyDoorSprite(canOpenWorldMap ? openedSprite : defaultSprite);
            return;
        }

        ApplyDoorSprite(defaultSprite);
    }

    private void ApplyDoorSprite(Sprite nextSprite)
    {
        if (doorButtonImage == null || nextSprite == null) return;

        doorButtonImage.sprite = nextSprite;
    }

    public void OnClickDoor()
    {
        if (ScreenStateManager.Instance == null) return;

        var state = ScreenStateManager.Instance.CurrentState;
        Debug.Log($"[DoorButton] Click / CurrentState = {state}");

        switch (state)
        {
            case ScreenStateManager.ScreenState.Main:
                if (QuestManager.Instance == null || !QuestManager.Instance.IsQuestActive)
                {
                    Debug.Log("[DoorButton] 진행 중인 퀘스트가 없어 월드맵으로 이동할 수 없습니다.");
                    return;
                }

                ScreenStateManager.Instance.GoToWorldMap();
                if (QuestManager.Instance != null && QuestManager.Instance.IsQuestActive)
                {
                    AdventureManager.Instance.SyncFromQuestTime(
                        QuestManager.Instance.QuestStartTime,
                        QuestManager.Instance.QuestEndTime
                    );
                }
                Debug.Log("[DoorButton] Quest active, SyncFromQuestTime 호출");
                break;

            case ScreenStateManager.ScreenState.WorldMap:
                ScreenStateManager.Instance.GoToMain();
                break;

            case ScreenStateManager.ScreenState.Minimized:
                if (QuestManager.Instance == null || !QuestManager.Instance.IsQuestActive)
                {
                    Debug.Log("[DoorButton] 진행 중인 퀘스트가 없어 월드맵 최소화 화면으로 이동할 수 없습니다.");
                    return;
                }

                ScreenStateManager.Instance.GoToWorldMapMinimized();
                if (QuestManager.Instance != null && QuestManager.Instance.IsQuestActive)
                {
                    AdventureManager.Instance.SyncFromQuestTime(
                        QuestManager.Instance.QuestStartTime,
                        QuestManager.Instance.QuestEndTime
                    );
                }
                break;

            case ScreenStateManager.ScreenState.WorldMapMinimized:
                ScreenStateManager.Instance.GoToMinimized();
                break;
        }
    }
}