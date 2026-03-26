using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnvilGoldController : MonoBehaviour, IPointerDownHandler
{
    [Header("Button")]
    [SerializeField] private Button anvilButton;

    [Header("Upgrade Systems")]
    [SerializeField] private UpgradeSystem clickUpgradeSystem;
    [SerializeField] private UpgradeSystem autoUpgradeSystem;

    [Header("Floating Text")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private RectTransform floatingParent;
    [SerializeField] private int maxFloatingText = 10;
    
    [SerializeField] private TMPro.TextMeshProUGUI gpsText;

    private float autoTimer;
    
    private readonly Queue<GameObject> floatingQueue = new Queue<GameObject>();

    private void Start()
    {
        if (anvilButton != null)
            anvilButton.onClick.AddListener(GainByClick);
    }

    private void Update()
    {
        HandleAutoGain();
        RefreshGPSUI();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPointerScreenPosition = eventData.position;
    }

    private Vector2 lastPointerScreenPosition;

    public void GainByClick()
    {
        if (clickUpgradeSystem == null || GoldManager.Instance == null)
            return;

        int amount = clickUpgradeSystem.CurrentValue;
        GoldManager.Instance.AddGold(amount);

        SpawnFloatingTextAtScreenPosition(amount, lastPointerScreenPosition);
    }

    private void HandleAutoGain()
    {
        if (autoUpgradeSystem == null || GoldManager.Instance == null)
            return;

        autoTimer += Time.deltaTime;

        if (autoTimer < 1f)
            return;

        autoTimer -= 1f;

        int amount = autoUpgradeSystem.CurrentValue; // 초당 골드
        GoldManager.Instance.AddGold(amount);
    }

    private void SpawnFloatingTextAtScreenPosition(int amount, Vector2 screenPosition)
    {
        if (floatingTextPrefab == null || floatingParent == null)
            return;

        GameObject obj = Instantiate(floatingTextPrefab, floatingParent);
        obj.transform.SetAsLastSibling();

        RectTransform textRect = obj.GetComponent<RectTransform>();
        if (textRect != null)
        {
            Canvas canvas = floatingParent.GetComponentInParent<Canvas>();
            Camera cam = null;

            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = canvas.worldCamera;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                floatingParent,
                screenPosition,
                cam,
                out localPoint
            );

            Vector2 randomOffset = new Vector2(
                Random.Range(-20f, 20f),
                Random.Range(-10f, 10f)
            );

            textRect.anchoredPosition = localPoint + randomOffset;
            textRect.localScale = Vector3.one;
            textRect.localRotation = Quaternion.identity;
        }

        FloatingText floatingText = obj.GetComponent<FloatingText>();
        if (floatingText != null)
            floatingText.SetText(amount);

        floatingQueue.Enqueue(obj);

        if (floatingQueue.Count > maxFloatingText)
        {
            GameObject oldObj = floatingQueue.Dequeue();
            if (oldObj != null)
                Destroy(oldObj);
        }
    }
    
    private void RefreshGPSUI()
    {
        if (gpsText == null || autoUpgradeSystem == null)
            return;

        float goldPerSecond = autoUpgradeSystem.CurrentValue;

        gpsText.text = $"{goldPerSecond:F0}/s";
    }
}