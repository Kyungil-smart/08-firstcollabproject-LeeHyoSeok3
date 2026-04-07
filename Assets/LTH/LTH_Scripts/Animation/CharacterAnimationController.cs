using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    protected Animator animator;
    private Vector3 initialLocalScale;
    private Vector3 initialLocalPosition;
    private Vector2 initialAnchoredPosition;
    private bool hasCachedTransformState;
    private RectTransform cachedRectTransform;

    protected virtual void Awake()
    {
        EnsureAnimator();
        CacheInitialTransformState();
    }

    protected virtual void Start()
    {
        EnsureAnimator();
    }

    protected void EnsureAnimator()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void CacheInitialTransformState()
    {
        if (hasCachedTransformState)
            return;

        initialLocalScale = transform.localScale;
        initialLocalPosition = transform.localPosition;
        cachedRectTransform = transform as RectTransform;

        if (cachedRectTransform != null)
            initialAnchoredPosition = cachedRectTransform.anchoredPosition;

        hasCachedTransformState = true;
    }

    protected virtual void ResetAnimationState()
    {
        EnsureAnimator();
        CacheInitialTransformState();

        transform.localScale = initialLocalScale;
        transform.localPosition = initialLocalPosition;

        if (cachedRectTransform != null)
            cachedRectTransform.anchoredPosition = initialAnchoredPosition;

        if (animator == null)
            return;

        // 화면 전환 도중 비활성화되더라도 애니메이션이 남긴 Transform 값을 기본 상태로 되돌린다.
        animator.Rebind();
        animator.Update(0f);

        transform.localScale = initialLocalScale;
        transform.localPosition = initialLocalPosition;

        if (cachedRectTransform != null)
            cachedRectTransform.anchoredPosition = initialAnchoredPosition;
    }
}