using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class OfflineRewardPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Button receiveButton;

    private Action _onClickReceive;

    public void Setup(string message, Action onClickReceive)
    {
        if (rewardText != null) rewardText.text = message;

        _onClickReceive = onClickReceive;

        if (receiveButton != null)
        {
            receiveButton.onClick.RemoveAllListeners();
            receiveButton.onClick.AddListener(OnClickReceiveButton);
        }
    }

    private void OnClickReceiveButton()
    {
        _onClickReceive?.Invoke();
    }
}
