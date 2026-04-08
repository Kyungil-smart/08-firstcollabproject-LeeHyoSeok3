using UnityEngine;
using System.Collections.Generic;
using DesignPattern;
using UnityEngine.UI;

public class PopupManager : Singleton<PopupManager>
{
    [Header("Popup RectTransforms")]
    [SerializeField] private RectTransform questBoardPopup;
    [SerializeField] private RectTransform blacksmithPopup;
    [SerializeField] private RectTransform partyEquipPopup;
    [SerializeField] private RectTransform settingsPopup;
    [SerializeField] private RectTransform offlineRewardPopup;
    [SerializeField] private RectTransform rewardPopup;
    [SerializeField] private RectTransform WarningPopup;

    [Header("Popup Canvas")]
    [SerializeField] private Canvas popupCanvas;

    [Header("Quest Board Button Visual")]
    [SerializeField] private Image questBoardButtonImage;
    [SerializeField] private Sprite questBoardDefaultSprite;
    [SerializeField] private Sprite questBoardOpenedSprite;

    [Header("Blacksmith Button Visual")]
    [SerializeField] private Image blacksmithButtonImage;
    [SerializeField] private Sprite blacksmithDefaultSprite;
    [SerializeField] private Sprite blacksmithOpenedSprite;

    private readonly List<RectTransform> _activePopups = new List<RectTransform>();

    protected override void OnAwake()
    {
        base.OnAwake();

        if (popupCanvas == null)
            Debug.LogError("[PopupManager] popupCanvas is not assigned.");
    }

    public void OpenQuestBoard()
    {
        if (questBoardPopup == null || questBoardPopup.gameObject.activeSelf)
            return;

        OpenPopup(questBoardPopup);
        UpdatePopupButtonVisual(questBoardPopup, true);
        SoundManager.Instance?.OneShot("QuestBoardOpen");
    }

    public void OpenBlacksmith()
    {
        if (blacksmithPopup == null || blacksmithPopup.gameObject.activeSelf)
            return;

        OpenPopup(blacksmithPopup);
        UpdatePopupButtonVisual(blacksmithPopup, true);
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

        if (popup.gameObject.activeSelf)
            return;

        RefreshPopupLocalizedUI(popup);
        popup.gameObject.SetActive(true);

        CenterInCanvas(popup);
        ClampToCanvas(popup);

        if (!_activePopups.Contains(popup))
            _activePopups.Add(popup);
    }

    public void ClosePopup(RectTransform popup)
    {
        if (popup == null)
            return;

        popup.gameObject.SetActive(false);
        _activePopups.Remove(popup);
        UpdatePopupButtonVisual(popup, false);
    }

    public void CloseAllPopups()
    {
        foreach (var popup in _activePopups)
        {
            if (popup == null)
                continue;

            popup.gameObject.SetActive(false);
            UpdatePopupButtonVisual(popup, false);
        }

        _activePopups.Clear();
    }

    private void RefreshPopupLocalizedUI(RectTransform popup)
    {
        if (popup == null)
            return;

        LocalizedText[] localizedTexts = popup.GetComponentsInChildren<LocalizedText>(true);
        foreach (var localizedText in localizedTexts)
        {
            localizedText.SendMessage("UpdateText", SendMessageOptions.DontRequireReceiver);
        }

        GearsetSlotUI[] gearSlots = popup.GetComponentsInChildren<GearsetSlotUI>(true);
        foreach (var slot in gearSlots)
        {
            slot.RefreshSlotInfo();
            slot.RefreshState();
        }
    }

    private void CenterInCanvas(RectTransform popup)
    {
        popup.anchorMin = new Vector2(0.5f, 0.5f);
        popup.anchorMax = new Vector2(0.5f, 0.5f);
        popup.pivot = new Vector2(0.5f, 0.5f);
        popup.anchoredPosition = Vector2.zero;
    }

    private void ClampToCanvas(RectTransform popup)
    {
        RectTransform canvasRect = popupCanvas.GetComponent<RectTransform>();

        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 popupSize = popup.rect.size;

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

    private void UpdatePopupButtonVisual(RectTransform popup, bool isOpen)
    {
        if (popup == questBoardPopup)
        {
            ApplyButtonSprite(questBoardButtonImage, isOpen ? questBoardOpenedSprite : questBoardDefaultSprite);
            return;
        }

        if (popup == blacksmithPopup)
        {
            ApplyButtonSprite(blacksmithButtonImage, isOpen ? blacksmithOpenedSprite : blacksmithDefaultSprite);
        }
    }

    private static void ApplyButtonSprite(Image targetImage, Sprite sprite)
    {
        if (targetImage == null || sprite == null)
            return;

        targetImage.sprite = sprite;
    }

    private void Start()
    {
        if (questBoardPopup) questBoardPopup.gameObject.SetActive(false);
        if (blacksmithPopup) blacksmithPopup.gameObject.SetActive(false);
        if (partyEquipPopup) partyEquipPopup.gameObject.SetActive(false);
        if (settingsPopup) settingsPopup.gameObject.SetActive(false);
        if (offlineRewardPopup) offlineRewardPopup.gameObject.SetActive(false);
        if (rewardPopup) rewardPopup.gameObject.SetActive(false);
        if (WarningPopup) WarningPopup.gameObject.SetActive(false);

        _activePopups.Clear();
        UpdatePopupButtonVisual(questBoardPopup, false);
        UpdatePopupButtonVisual(blacksmithPopup, false);
    }
}