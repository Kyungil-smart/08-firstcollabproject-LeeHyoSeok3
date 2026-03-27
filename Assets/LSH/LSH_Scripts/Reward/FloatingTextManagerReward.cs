using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManagerReward : MonoBehaviour
{
    // 싱글톤(Singleton) 패턴: 박사님의 프로젝트 어디서든 쉽게 매니저를 부를 수 있게 합니다.
    public static FloatingTextManagerReward Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private FloatingTextReward m_textPrefab; // 아까 만든 프리팹을 넣을 자리
    [SerializeField] private Transform m_poolContainer; // 텍스트들이 정리될 부모 폴더 역할
    [SerializeField] private int m_initialPoolSize = 10; // 처음에 미리 만들어둘 개수

    // Queue(큐)는 선입선출(먼저 들어온 게 먼저 나감) 방식의 대기열 창고입니다.
    private Queue<FloatingTextReward> m_textPool = new Queue<FloatingTextReward>();

    private void Awake()
    {
        // 싱글톤 기본 세팅
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePool();
    }

    // 시작할 때 지정된 개수만큼 텍스트를 미리 찍어냅니다. (가비지 컬렉션 방지)
    private void InitializePool()
    {
        for (int i = 0; i < m_initialPoolSize; i++)
        {
            CreateNewTextObject();
        }
    }

    private FloatingTextReward CreateNewTextObject()
    {
        // Instantiate는 게임 시작할 때(Awake)만 호출되므로 게임 도중엔 렉이 안 걸립니다.
        FloatingTextReward newText = Instantiate(m_textPrefab, m_poolContainer);
        newText.gameObject.SetActive(false); // 처음엔 꺼둡니다.
        m_textPool.Enqueue(newText); // 창고(Queue)에 집어넣습니다.
        return newText;
    }

    // 외부에서 텍스트를 띄우고 싶을 때 부르는 함수
    public void ShowText(string message, Vector3 spawnPosition)
    {
        // 창고에 텍스트가 다 떨어졌다면 임시로 하나 더 만듭니다.
        if (m_textPool.Count == 0)
        {
            CreateNewTextObject();
        }

        // 창고에서 하나 꺼냅니다. (Dequeue)
        FloatingTextReward textObj = m_textPool.Dequeue();

        // 텍스트 내용과 위치를 세팅하고 작동시킵니다.
        textObj.Setup(message, spawnPosition);
    }

    // 텍스트가 연출을 끝내고 창고로 돌아올 때 부르는 함수
    public void ReturnToPool(FloatingTextReward textObj)
    {
        textObj.gameObject.SetActive(false); // 화면에서 끄고
        m_textPool.Enqueue(textObj); // 다시 창고에 넣습니다.
    }
}