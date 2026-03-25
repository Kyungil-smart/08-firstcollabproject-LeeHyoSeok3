using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private RectTransform rectTransform;
    private float timer;

    [SerializeField] private float moveSpeed = 80f;
    [SerializeField] private float lifeTime = 1f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void SetText(int amount)
    {
        if (text == null)
        {
            Debug.LogError("FloatingText: 자식에서 TextMeshProUGUI를 찾지 못했습니다.");
            return;
        }

        text.text = $"+{amount}";
        Debug.Log("텍스트 설정됨: " + text.text);
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