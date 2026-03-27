using UnityEngine;
using TMPro;

// 이 스크립트를 오브젝트에 넣으면 CanvasGroup도 자동으로 붙여주는 편리한 기능입니다.
[RequireComponent(typeof(CanvasGroup))]
public class FloatingTextReward : MonoBehaviour
{
    private TMP_Text m_text;
    private CanvasGroup m_canvasGroup;

    private float m_moveSpeed = 50f; // 위로 올라가는 속도
    private float m_duration = 1.5f; // 텍스트가 화면에 머무는 시간
    private float m_timer;           // 시간 측정용 변수

    private void Awake()
    {
        // 자신의 컴포넌트들을 미리 찾아옵니다.
        m_text = GetComponent<TMP_Text>();
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    // 풀 매니저가 이 텍스트를 꺼낼 때 호출할 초기화 함수입니다.
    public void Setup(string message, Vector3 spawnPosition)
    {
        m_text.text = message;
        transform.position = spawnPosition;

        m_timer = 0f;
        m_canvasGroup.alpha = 1f; // 투명도 100% (완전 선명함)

        gameObject.SetActive(true); // 오브젝트 켜기
    }

    private void Update()
    {
        // 1. 매 프레임마다 텍스트를 위로 이동시킵니다.
        transform.position += Vector3.up * m_moveSpeed * Time.deltaTime;

        // 2. 타이머를 증가시킵니다.
        m_timer += Time.deltaTime;

        // 3. 남은 시간에 비례하여 서서히 투명해지게 만듭니다. (1 -> 0)
        m_canvasGroup.alpha = 1f - (m_timer / m_duration);

        // 4. 지정된 시간이 다 지나면 스스로 풀(대기열)로 돌아갑니다.
        if (m_timer >= m_duration)
        {
            FloatingTextManagerReward.Instance.ReturnToPool(this);
        }
    }
}