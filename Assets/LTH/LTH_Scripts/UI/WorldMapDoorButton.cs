using UnityEngine;

public class WorldMapDoorButton : MonoBehaviour
{
    public void OnClickDoor()
    {
        if (ScreenStateManager.Instance == null) return;

        var state = ScreenStateManager.Instance.CurrentState;
        Debug.Log($"[DoorButton] Click / CurrentState = {state}");

        switch (state)
        {
            case ScreenStateManager.ScreenState.Main:
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