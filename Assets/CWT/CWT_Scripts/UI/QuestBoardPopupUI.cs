using UnityEngine;

public class QuestBoardPopupUI : MonoBehaviour
{
    [SerializeField] private GameObject _questBoardPopup;


    // 대장간 화면에서 퀘스트보드 클릭버튼
    public void OpenPopup()
    {
        _questBoardPopup.SetActive(true);
        SoundManager.Instance.OneShot("mouseClick");
    }

    // 퀘스트 보드 팝업창에서 종료버튼
    public void ClosePopup()
    {
        SoundManager.Instance.OneShot("mouseClick");
        _questBoardPopup.SetActive(false);
    }
}
