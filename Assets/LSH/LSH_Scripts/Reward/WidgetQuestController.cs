using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WidgetQuestController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text m_timerText;
    [SerializeField] private GameObject m_rewardSackObject; // 보상 자루 오브젝트 
    [SerializeField] private Button m_rewardSackButton;
    [SerializeField] private RectTransform m_floatingTextSpawnPoint;

    // 오버플로우 방지를 위해 double 사용 
    private double m_currentGold = 0;
    private double m_questRewardGold = 1500;

    private QuestTimer m_questTimer;

    private void Awake()
    {
        m_rewardSackButton.onClick.AddListener(OnClickRewardSack);
        m_rewardSackObject.SetActive(false);
    }

    private void Start()
    {
        // 예시: 2시간(7200초)짜리 퀘스트 시작 [cite: 458, 459]
        // 테스트를 위해 10초로 설정했습니다.
        m_questTimer = new QuestTimer(10.0f);
    }

    private void Update()
    {
        if (m_questTimer == null || m_questTimer.IsComplete)
        {
            return;
        }

        m_questTimer.UpdateTimer(Time.deltaTime);

        // 정수 형태로 포맷팅하여 출력
        m_timerText.text = FormatTime(m_questTimer.RemainingSeconds);

        if (m_questTimer.IsComplete)
        {
            ShowRewardSack();
        }
    }

    private void ShowRewardSack()
    {
        m_timerText.text = "00:00";
        // 퀘스트 보드 앞에 보상 자루 표시 
        m_rewardSackObject.SetActive(true);
    }

    private void OnClickRewardSack()
    {
        // 1. 재화 합산 (double 타입 연산)
        m_currentGold += m_questRewardGold;

        // 2. 단위 변환 표시 (예: 1000 -> 1K) [cite: 325]
        string displayReward = FormatCurrency(m_questRewardGold);
        Debug.Log($"골드 획득: {displayReward} / 총 보유: {FormatCurrency(m_currentGold)}");

        // 3. 자루 UI 숨기기
        m_rewardSackObject.SetActive(false);

        // 4. 플로팅 텍스트 연출 호출 [cite: 321]
        ShowFloatingText($"+{displayReward}");
    }

    private void ShowFloatingText(string message)
    {
        // Debug.Log 대신, 우리가 방금 만든 풀링 매니저에게 텍스트를 띄워달라고 요청합니다!
        if (FloatingTextManagerReward.Instance != null)
        {
            FloatingTextManagerReward.Instance.ShowText(message, m_floatingTextSpawnPoint.position);
        }
    }

    private string FormatTime(int totalSeconds)
    {
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        if (hours > 0)
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        return $"{minutes:00}:{seconds:00}";
    }

    // 1000 단위를 K, M 등으로 변환하는 헬퍼 함수 [cite: 325]
    private string FormatCurrency(double amount)
    {
        if (amount >= 1000000)
            return (amount / 1000000D).ToString("0.##") + "M";
        if (amount >= 1000)
            return (amount / 1000D).ToString("0.##") + "K";

        return amount.ToString("0");
    }
}