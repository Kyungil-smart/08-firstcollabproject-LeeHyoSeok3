using UnityEngine;
using TMPro;
using UnityEngine.Pool;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    // 자신을 관리하는 풀의 참조를 가지고 있습니다.
    private IObjectPool<FloatingText> _pool;

    public void SetPool(IObjectPool<FloatingText> pool)
    {
        _pool = pool;
    }

    /// <summary>
    /// 풀에서 꺼내질 때 텍스트와 위치를 초기화합니다.
    /// </summary>
    public void Setup(string message, Vector3 spawnPosition)
    {
        _text.text = message;
        transform.position = spawnPosition;

        // 참고: 위로 올라가며 사라지는 연출은 Animator 컴포넌트가 
        // 오브젝트 활성화(OnEnable) 시 자동으로 재생하도록 세팅하는 것이 편합니다.
    }

    /// <summary>
    /// 애니메이션 클립의 마지막 프레임에 Animation Event로 이 함수를 달아줍니다.
    /// </summary>
    public void OnAnimationComplete()
    {
        // 애니메이션이 끝나면 스스로를 파괴하지 않고 풀로 반환합니다.
        _pool?.Release(this);
    }
}