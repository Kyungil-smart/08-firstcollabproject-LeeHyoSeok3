using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 버튼 클릭 한 번으로 지정한 화면 상태로 전환한다.
/// Mini 버튼 오브젝트에 붙이면 별도 OnClick 수동 연결 없이 바로 사용할 수 있다.
/// </summary>
[RequireComponent(typeof(Button))]
public class MinimizeButtonBridge : MonoBehaviour
{
    [SerializeField] private Button targetButton;
    [SerializeField] private ScreenStateManager.ScreenState targetState = ScreenStateManager.ScreenState.Minimized;

    private void Reset()
    {
        targetButton = GetComponent<Button>();
    }

    private void Awake()
    {
        if (targetButton == null)
            targetButton = GetComponent<Button>();

        if (targetButton != null)
            targetButton.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        if (targetButton != null)
            targetButton.onClick.RemoveListener(HandleClick);
    }

    public void HandleClick()
    {
        ScreenStateManager.Instance?.TransitionTo(targetState);
    }
}
