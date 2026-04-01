using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TooltipHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SimpleTooltipUI tooltipUI;

    [TextArea]
    [SerializeField] private string tooltipMessage;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
    }

    public void SetTooltip(string message)
    {
        tooltipMessage = message;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipUI == null)
            return;

        if (string.IsNullOrWhiteSpace(tooltipMessage))
            return;

        if (Mouse.current == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        tooltipUI.Show(tooltipMessage, mousePos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipUI != null)
            tooltipUI.Hide();
    }

    public void CheckAndShowTooltip()
    {
        if (tooltipUI == null || rectTransform == null)
            return;

        if (string.IsNullOrWhiteSpace(tooltipMessage))
            return;

        if (Mouse.current == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera cam = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = canvas.worldCamera;

        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePos, cam);

        if (isInside)
            tooltipUI.Show(tooltipMessage, mousePos);
        else
            tooltipUI.Hide();
    }

    private void ShowTooltip(Vector2 screenPosition)
    {
        if (tooltipUI == null)
            return;

        if (string.IsNullOrWhiteSpace(tooltipMessage))
            return;

        tooltipUI.Show(tooltipMessage, screenPosition);
    }

    private void OnDisable()
    {
        if (tooltipUI != null)
            tooltipUI.Hide();
    }
}