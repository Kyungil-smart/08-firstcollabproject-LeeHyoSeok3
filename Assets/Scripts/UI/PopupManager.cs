using UnityEngine;
using System.Collections.Generic;
using DesignPattern;

public class PopupManager : Singleton<PopupManager>
{
    [Header("팝업 프리팹 또는 씬 내 오브젝트")]
    [SerializeField] private RectTransform questBoardPopup;
    [SerializeField] private RectTransform blacksmithPopup;
    [SerializeField] private RectTransform partyEquipPopup;
    [SerializeField] private RectTransform settingsPopup;
    [SerializeField] private RectTransform offlineRewardPopup;
    [SerializeField] private RectTransform rewardPopup; 
    [SerializeField] private RectTransform WarningPopup;

    [Header("팝업을 배치할 Canvas")]
    [SerializeField] private Canvas popupCanvas;

    private readonly List<RectTransform> _activePopups = new List<RectTransform>();

    protected override void OnAwake()
    {
        base.OnAwake();

        if (popupCanvas == null)
            Debug.LogError("[PopupManager] popupCanvas가 할당되지 않았습니다.");
    }

    public void OpenQuestBoard()
    {
        if (questBoardPopup == null || questBoardPopup.gameObject.activeSelf)
            return;

        OpenPopup(questBoardPopup);
        SoundManager.Instance?.OneShot("QuestBoardOpen");
    }
    public void OpenBlacksmith()
    {
        if (blacksmithPopup == null || blacksmithPopup.gameObject.activeSelf)
            return;

        OpenPopup(blacksmithPopup);
        SoundManager.Instance?.OneShot("BlacksmithOpen");
    }
    public void OpenPartyEquip()
    {
        if (partyEquipPopup == null || partyEquipPopup.gameObject.activeSelf)
            return;

        OpenPopup(partyEquipPopup);
        SoundManager.Instance?.OneShot("PartyEquipOpen");
    }
    public void OpenSettings() => OpenPopup(settingsPopup);
    public void OpenOfflineRewardPopup() => OpenPopup(offlineRewardPopup);
    public void OpenRewardPopup()
    {
        if (rewardPopup == null || rewardPopup.gameObject.activeSelf)
            return;

        OpenPopup(rewardPopup);
        SoundManager.Instance?.OneShot("RewardSack");
    }
    public void OpenWarningPopup() => OpenPopup(WarningPopup);

    public void OpenPopup(RectTransform popup)
    {
        if (popup == null || popupCanvas == null)
            return;

        // 추가 : 이미 열려있는 팝업이면 다시 열지 않음 (사운드 겹침 현상 방지)
        if (popup.gameObject.activeSelf) return;

        popup.gameObject.SetActive(true);

        // 먼저 중앙 배치
        CenterInCanvas(popup);

        // 그 다음 화면 밖으로 안 나가게 보정
        ClampToCanvas(popup);

        if (!_activePopups.Contains(popup))
            _activePopups.Add(popup);
    }

    public void ClosePopup(RectTransform popup)
    {
        if (popup == null) return;

        popup.gameObject.SetActive(false);
        _activePopups.Remove(popup);
    }

    public void CloseAllPopups()
    {
        foreach (var popup in _activePopups)
        {
            if (popup != null)
                popup.gameObject.SetActive(false);
        }

        _activePopups.Clear();
    }

    /// <summary>
    /// Canvas 중앙에 배치
    /// </summary>
    private void CenterInCanvas(RectTransform popup)
    {
        popup.anchorMin = new Vector2(0.5f, 0.5f);
        popup.anchorMax = new Vector2(0.5f, 0.5f);
        popup.pivot = new Vector2(0.5f, 0.5f);
        popup.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// 팝업이 Canvas 바깥으로 나가지 않도록 보정
    /// </summary>
    private void ClampToCanvas(RectTransform popup)
    {
        RectTransform canvasRect = popupCanvas.GetComponent<RectTransform>();

        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 popupSize = popup.rect.size;

        // Canvas보다 팝업이 더 크면 일단 크기부터 줄임
        float clampedWidth = Mathf.Min(popupSize.x, canvasSize.x);
        float clampedHeight = Mathf.Min(popupSize.y, canvasSize.y);

        popup.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clampedWidth);
        popup.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clampedHeight);

        popupSize = popup.rect.size;

        float halfCanvasW = canvasSize.x * 0.5f;
        float halfCanvasH = canvasSize.y * 0.5f;
        float halfPopupW = popupSize.x * 0.5f;
        float halfPopupH = popupSize.y * 0.5f;

        Vector2 pos = popup.anchoredPosition;

        pos.x = Mathf.Clamp(pos.x, -halfCanvasW + halfPopupW, halfCanvasW - halfPopupW);
        pos.y = Mathf.Clamp(pos.y, -halfCanvasH + halfPopupH, halfCanvasH - halfPopupH);

        popup.anchoredPosition = pos;
    }

    /// <summary>
    /// 현재 게임 화면 기준으로 팝업 크기 조절
    /// </summary>
    public void ScalePopupToScreen(RectTransform popup, float widthRatio, float heightRatio)
    {
        if (popup == null || popupCanvas == null)
            return;

        float w = Screen.width * widthRatio;
        float h = Screen.height * heightRatio;

        RectTransform canvasRect = popupCanvas.GetComponent<RectTransform>();
        float maxW = canvasRect.rect.width;
        float maxH = canvasRect.rect.height;

        w = Mathf.Min(w, maxW);
        h = Mathf.Min(h, maxH);

        popup.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        popup.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }

    private void Start()
    {
        //if (questBoardPopup) ScalePopupToScreen(questBoardPopup, 0.38f, 0.46f);
        //if (blacksmithPopup) ScalePopupToScreen(blacksmithPopup, 0.677f, 0.694f);
        //if (partyEquipPopup) ScalePopupToScreen(partyEquipPopup, 0.401f, 0.2963f);
        //if (settingsPopup) ScalePopupToScreen(settingsPopup, 0.208f, 0.37f);

        if (questBoardPopup) questBoardPopup.gameObject.SetActive(false);
        if (blacksmithPopup) blacksmithPopup.gameObject.SetActive(false);
        if (partyEquipPopup) partyEquipPopup.gameObject.SetActive(false);
        if (settingsPopup) settingsPopup.gameObject.SetActive(false);
        if (offlineRewardPopup) offlineRewardPopup.gameObject.SetActive(false);
        if (rewardPopup) rewardPopup.gameObject.SetActive(false);
        if (WarningPopup) WarningPopup.gameObject.SetActive(false);

        _activePopups.Clear();
    }
}