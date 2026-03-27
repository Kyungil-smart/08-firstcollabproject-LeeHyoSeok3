// ============================================
// 파일명: QuestBoardPopupUI.cs
// 붙일 오브젝트: QuestBoardPopup
// 역할: 퀘스트 보드 팝업 열기/닫기 + PopupManager 크기 덮어씌움 보정
// ============================================

using UnityEngine;
using System.Collections;

public class QuestBoardPopupUI : MonoBehaviour
{
    // 퀘스트 보드 팝업의 원래 크기 (Inspector에서 조절 가능)
    // 네가 설정한 크기에 맞게 바꿔줘
    [SerializeField] private float _popupWidth = 800f;
    [SerializeField] private float _popupHeight = 600f;

    // 크기 조절용 RectTransform
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    // PopupManager가 SetActive(true) 하면 자동으로 호출됨
    private void OnEnable()
    {
        // 1프레임 기다렸다가 보정 (PopupManager 작업이 끝난 뒤 실행)
        StartCoroutine(FixSizeAndPosition());
    }

    private IEnumerator FixSizeAndPosition()
    {
        yield return null;

        // PopupManager가 바꿔버린 크기를 원래 크기로 강제 세팅
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _popupWidth);
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _popupHeight);

        // 위치를 앵커 기준 정중앙으로 보정
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    // 대장간 화면에서 퀘스트보드 클릭버튼
    public void OpenPopup()
    {
        // 이미 열려있으면 중복 실행 방지
        if (gameObject.activeSelf) return;

        // PopupManager를 통해 열기
        PopupManager.Instance.OpenQuestBoard();
        SoundManager.Instance.OneShot("mouseClick");
    }

    // 퀘스트 보드 팝업창에서 종료버튼
    public void ClosePopup()
    {
        SoundManager.Instance.OneShot("mouseClick");
        PopupManager.Instance.ClosePopup(_rectTransform);
    }
}