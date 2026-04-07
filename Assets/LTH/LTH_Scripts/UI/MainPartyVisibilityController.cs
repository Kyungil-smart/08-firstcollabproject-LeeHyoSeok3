using UnityEngine;


/// <summary>
/// 대장간에 파티 캐릭터를 퀘스트 진행 상황에 따라 자동으로 숨기거나 보여주는 역할
/// </summary>
public class MainPartyVisibilityController : MonoBehaviour
{
    [Header("Hide During Quest")]
    [SerializeField] private GameObject[] CharactersToHideDuringQuest;
    private bool hideWhenQuestActive = true;

    private void OnEnable()
    {
        if (AdventureManager.Instance != null)
        {
            AdventureManager.Instance.OnAdventureStarted += HandleAdventureStarted;
            AdventureManager.Instance.OnAdventureCompleted += HandleAdventureCompleted;
        }

        GameDataController.OnGameLoaded += HandleGameLoaded;

        RefreshVisibility();
    }

    private void OnDisable()
    {
        if (AdventureManager.Instance != null)
        {
            AdventureManager.Instance.OnAdventureStarted -= HandleAdventureStarted;
            AdventureManager.Instance.OnAdventureCompleted -= HandleAdventureCompleted;
        }

        GameDataController.OnGameLoaded -= HandleGameLoaded;
    }

    private void HandleAdventureStarted()
    {
        SetTargetsVisible(!hideWhenQuestActive);
    }

    private void HandleAdventureCompleted()
    {
        SetTargetsVisible(true);
    }

    private void HandleGameLoaded()
    {
        RefreshVisibility();
    }

    public void RefreshVisibility()
    {
        bool shouldShow = true;

        if (hideWhenQuestActive && QuestManager.Instance != null && QuestManager.Instance.IsQuestActive)
            shouldShow = false;

        SetTargetsVisible(shouldShow);
    }

    private void SetTargetsVisible(bool isVisible)
    {
        if (CharactersToHideDuringQuest == null)
            return;

        for (int i = 0; i < CharactersToHideDuringQuest.Length; i++)
        {
            if (CharactersToHideDuringQuest[i] != null)
                CharactersToHideDuringQuest[i].SetActive(isVisible);
        }
    }
}