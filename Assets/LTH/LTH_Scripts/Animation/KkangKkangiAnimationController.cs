using System.Collections;
using UnityEngine;

public class KkangKkangiAnimationController : CharacterAnimationController
{
    private Coroutine blinkCoroutine;
    private Coroutine craftCoroutine;
    private Coroutine resetCoroutine;

    [SerializeField] private GameObject hammerHead;

    private ParticleSystem[] currentHammerHitEffects;

    protected override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        EnsureAnimator();
        if (CanSafelyResetAnimator())
        {
            ResetAnimationState();
        }
        else
        {
            resetCoroutine = StartCoroutine(ResetAnimationStateNextFrame());
        }

        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    private void OnDisable()
    {
        StopRunningCoroutines();

        if (hammerHead != null)
            hammerHead.SetActive(true);

        ResetAnimationState();
    }

    public void SetHammerHitEffects(ParticleSystem[] hammerHitEffects)
    {
        currentHammerHitEffects = hammerHitEffects;
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(4f, 8f));

            if (animator != null) animator.SetTrigger("Blink");
        }
    }

    public void PlayCraftAnimation()
    {
        if (!gameObject.activeInHierarchy) return;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (hammerHead != null) hammerHead.SetActive(false);

        animator.SetTrigger("Craft");
        if (craftCoroutine != null)
            StopCoroutine(craftCoroutine);

        craftCoroutine = StartCoroutine(CraftRoutine());
    }

    public void PlayHammerHitEffect()
    {
        if (!gameObject.activeInHierarchy || currentHammerHitEffects == null || currentHammerHitEffects.Length == 0)
            return;

        for (int i = 0; i < currentHammerHitEffects.Length; i++)
        {
            ParticleSystem effect = currentHammerHitEffects[i];
            if (effect == null)
                continue;

            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            effect.Play();
        }
    }

    private IEnumerator CraftRoutine()
    {
        yield return new WaitForSeconds(1f);

        if (hammerHead != null) hammerHead.SetActive(true);

        craftCoroutine = null;
        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    private void StopRunningCoroutines()
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (craftCoroutine != null)
        {
            StopCoroutine(craftCoroutine);
            craftCoroutine = null;
        }
    }

    private IEnumerator ResetAnimationStateNextFrame()
    {
        yield return null;

        if (CanSafelyResetAnimator())
            ResetAnimationState();

        resetCoroutine = null;
    }
}