using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float moveSpeed = 80f;
    [SerializeField] private float lifeTime = 1f;

    private RectTransform rectTransform;
    private float timer;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>(true);

        if (text != null)
        {
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchoredPosition = Vector2.zero;
            textRect.localScale = Vector3.one;
            textRect.localRotation = Quaternion.identity;

            text.raycastTarget = false;
        }
    }

    public void SetText(int amount)
    {
        if (text == null)
        {
            Debug.LogError("FloatingText: TextMeshProUGUI를 찾지 못했습니다.");
            return;
        }

        text.text = $"+{amount}";
        text.enabled = true;
        text.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}