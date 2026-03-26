using UnityEngine;
using UnityEngine.Pool;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private FloatingText_Reward _textPrefab;
    [SerializeField] private int _defaultCapacity = 20;
    [SerializeField] private int _maxSize = 100;

    private IObjectPool<FloatingText_Reward> _textPool;

    private void Awake()
    {
        // 간단한 싱글톤 패턴 적용 (어디서든 쉽게 부를 수 있도록)
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitPool();
    }

    private void InitPool()
    {
        _textPool = new ObjectPool<FloatingText_Reward>(
            createFunc: () =>
            {
                // 풀에 여분이 없을 때 새로 생성하는 로직
                var obj = Instantiate(_textPrefab, transform);
                obj.SetPool(_textPool);
                return obj;
            },
            actionOnGet: obj => obj.gameObject.SetActive(true),   // 꺼낼 때 활성화
            actionOnRelease: obj => obj.gameObject.SetActive(false), // 반환할 때 비활성화
            actionOnDestroy: obj => Destroy(obj.gameObject),
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
        );
    }

    /// <summary>
    /// 지정된 위치에 플로팅 텍스트를 띄웁니다.
    /// </summary>
    /// <param name="message">출력할 내용 (예: "+100", "보상 획득!")</param>
    /// <param name="position">텍스트가 나타날 월드 또는 UI 좌표</param>
    public void SpawnText(string message, Vector3 position)
    {
        // 풀에서 오브젝트를 하나 꺼내서 Setup 함수를 실행합니다.
        FloatingText_Reward floatingText = _textPool.Get();
        floatingText.Setup(message, position);
    }
}