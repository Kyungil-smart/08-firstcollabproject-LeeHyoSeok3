using UnityEngine;
using UnityEngine.UI;

public class WorldMapDoorButton : MonoBehaviour
{
    [SerializeField] private Button doorButton;

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
        if (QuestManager.Instance == null) return;

        bool canOpenWorldMap = QuestManager.Instance.IsQuestActive;

        var state = ScreenStateManager.Instance != null
            ? ScreenStateManager.Instance.CurrentState
            : ScreenStateManager.ScreenState.Main;

        // Main / Minimized 상태일 때만 퀘스트 진행 여부에 따라 월드맵 진입 가능
        // WorldMap / WorldMapMinimized 상태에서는 "되돌아가기" 버튼 역할이라 항상 허용
        if (state == ScreenStateManager.ScreenState.Main || state == ScreenStateManager.ScreenState.Minimized)
        {
            doorButton.interactable = canOpenWorldMap;
        }
        else
        {
            doorButton.interactable = true;
        }
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