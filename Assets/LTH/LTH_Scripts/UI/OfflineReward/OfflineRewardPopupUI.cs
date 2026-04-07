using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class OfflineRewardPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Button receiveButton;

    private Func<string> _messageProvider;
    private Action _onClickReceive;

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += RefreshMessage;
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshMessage;
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshMessage;
    }

    public void Setup(Func<string> messageProvider, Action onClickReceive)
    {
        _messageProvider = messageProvider;
        RefreshMessage();

        _onClickReceive = onClickReceive;

        if (receiveButton != null)
        {
            receiveButton.onClick.RemoveAllListeners();
            receiveButton.onClick.AddListener(OnClickReceiveButton);
        }
    }

    private void RefreshMessage()
    {
        if (rewardText == null)
            return;

        rewardText.text = _messageProvider != null ? _messageProvider.Invoke() : string.Empty;
    }

    private void OnClickReceiveButton()
    {
        _onClickReceive?.Invoke();
    }
}
