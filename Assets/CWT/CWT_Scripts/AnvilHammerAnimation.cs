using UnityEngine;
using UnityEngine.EventSystems;

public class AnvilHammerAnimation : MonoBehaviour, IPointerClickHandler
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        Debug.Log($"[AnvilHammer] Animator 찾기: {(_animator != null ? "성공" : "실패")}");
    }

    // 이 오브젝트를 직접 클릭했을 때 호출됨
    // Button 컴포넌트와 무관하게 동작
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[AnvilHammer] 클릭 감지됨!");
        PlayHammering();
    }

    public void PlayHammering()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Hit");
            Debug.Log("[AnvilHammer] Hit 트리거 발동!");
        }
    }
}