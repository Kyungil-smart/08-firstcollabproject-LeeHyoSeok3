using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleTooltipUI : MonoBehaviour
{
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private RectTransform bgRect;
    [SerializeField] private TextMeshProUGUI tooltipText;

    [Header("Position")]
    [SerializeField] private Vector2 offset = new Vector2(28f, 12f);

    [Header("Padding")]
    [SerializeField] private float leftPadding = 16f;
    [SerializeField] private float rightPadding = 16f;
    [SerializeField] private float topPadding = 12f;
    [SerializeField] private float bottomPadding = 12f;
    [SerializeField] private float maxWidth = 300f;

    private bool isShowing;
    private RectTransform textRect;
    private Canvas parentCanvas;
    private RectTransform canvasRect;

    private void Awake()
    {
        if (tooltipText == null)
        {
            Debug.LogError("[SimpleTooltipUI] tooltipText가 연결되지 않았습니다.", this);
            return;
        }

        if (tooltipRect == null)
        {
            Debug.LogError("[SimpleTooltipUI] tooltipRect가 연결되지 않았습니다.", this);
            return;
        }

        if (bgRect == null)
        {
            Debug.LogError("[SimpleTooltipUI] bgRect가 연결되지 않았습니다.", this);
            return;
        }

        textRect = tooltipText.rectTransform;

        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
            canvasRect = parentCanvas.transform as RectTransform;

        tooltipRect.anchorMin = new Vector2(0.5f, 0.5f);
        tooltipRect.anchorMax = new Vector2(0.5f, 0.5f);
        tooltipRect.pivot = new Vector2(0, 1);

        Hide();
    }

    private void Update()
    {
        if (!isShowing || Mouse.current == null)
            return;

        UpdatePosition(Mouse.current.position.ReadValue());
    }

    public void Show(string message, Vector2 screenPosition)
    {
        if (tooltipRect == null || bgRect == null || tooltipText == null)
        {
            Debug.LogError("[SimpleTooltipUI] Show 실패: 필수 참조가 비어 있습니다.", this);
            return;
        }

        if (textRect == null)
            textRect = tooltipText.rectTransform;

        tooltipText.text = message;

        ResizeBg();
        Canvas.ForceUpdateCanvases();
        UpdatePosition(screenPosition);

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        isShowing = true;
    }

    private void ResizeBg()
    {
        if (tooltipText == null || bgRect == null)
        {
            Debug.LogError("[SimpleTooltipUI] ResizeBg 실패: tooltipText 또는 bgRect가 null입니다.", this);
            return;
        }

        if (textRect == null)
            textRect = tooltipText.rectTransform;

        if (textRect == null)
        {
            Debug.LogError("[SimpleTooltipUI] ResizeBg 실패: textRect를 가져오지 못했습니다.", this);
            return;
        }

        tooltipText.enableWordWrapping = true;

        float textWidth = maxWidth - (leftPadding + rightPadding);
        if (textWidth < 1f)
            textWidth = 1f;

        textRect.sizeDelta = new Vector2(textWidth, textRect.sizeDelta.y);
        tooltipText.ForceMeshUpdate();

        Vector2 preferred = tooltipText.GetPreferredValues(tooltipText.text, textWidth, 0f);

        float bgWidth = preferred.x + leftPadding + rightPadding;
        float bgHeight = preferred.y + topPadding + bottomPadding;

        bgRect.sizeDelta = new Vector2(bgWidth, bgHeight);

        textRect.anchorMin = new Vector2(0, 1);
        textRect.anchorMax = new Vector2(0, 1);
        textRect.pivot = new Vector2(0, 1);
        textRect.anchoredPosition = new Vector2(leftPadding, -topPadding);
        textRect.sizeDelta = new Vector2(
            bgWidth - (leftPadding + rightPadding),
            bgHeight - (topPadding + bottomPadding)
        );
    }

    private void UpdatePosition(Vector2 mouseScreenPos)
    {
        if (canvasRect == null || tooltipRect == null || bgRect == null)
            return;

        Camera cam = null;
        if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = parentCanvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mouseScreenPos,
            cam,
            out Vector2 localPoint
        );

        float width = bgRect.rect.width;
        float height = bgRect.rect.height;

        Vector2 target = localPoint + offset;

        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        if (target.x + width > canvasWidth * 0.5f)
            target.x = localPoint.x - width - Mathf.Abs(offset.x);

        if (target.y - height < -canvasHeight * 0.5f)
            target.y = localPoint.y + height + Mathf.Abs(offset.y);

        tooltipRect.anchoredPosition = target;
    }

    public void Hide()
    {
        isShowing = false;
        gameObject.SetActive(false);
    }
}