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

    /// <summary>
    /// 애니메이터의 상태를 초기값으로 리셋하는 메서드
    /// </summary>
    protected virtual void ResetAnimationState()
    {
        EnsureAnimator();
        CacheInitialTransformState();

        transform.localScale = initialLocalScale;
        transform.localPosition = initialLocalPosition;

        if (cachedRectTransform != null)
            cachedRectTransform.anchoredPosition = initialAnchoredPosition;

        if (!CanSafelyResetAnimator()) return;

        animator.Rebind();
        animator.Update(0f);

        transform.localScale = initialLocalScale;
        transform.localPosition = initialLocalPosition;

        if (cachedRectTransform != null)
            cachedRectTransform.anchoredPosition = initialAnchoredPosition;
    }

    /// <summary>
    /// 애니메이터를 안전하게 리셋할 수 있는지 여부를 판단
    /// </summary>
    /// <returns></returns>
    protected bool CanSafelyResetAnimator()
    {
        return animator != null
            && gameObject.activeInHierarchy
            && isActiveAndEnabled
            && animator.isActiveAndEnabled
            && animator.isInitialized;
    }
}