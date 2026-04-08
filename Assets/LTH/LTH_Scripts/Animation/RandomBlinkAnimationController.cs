using System.Collections;
using UnityEngine;

public class RandomBlinkAnimationController : CharacterAnimationController
{
    [SerializeField] private string blinkTriggerName = "Blink";
    [SerializeField] private float minBlinkInterval = 4f;
    [SerializeField] private float maxBlinkInterval = 8f;

    private Coroutine blinkCoroutine;
    private Coroutine resetCoroutine;

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
        ResetAnimationState();
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            float nextBlinkDelay = Random.Range(minBlinkInterval, maxBlinkInterval);
            yield return new WaitForSeconds(nextBlinkDelay);

            if (animator != null && !string.IsNullOrEmpty(blinkTriggerName))
                animator.SetTrigger(blinkTriggerName);
        }
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
    }

    private IEnumerator ResetAnimationStateNextFrame()
    {
        yield return null;

        if (CanSafelyResetAnimator())
            ResetAnimationState();

        resetCoroutine = null;
    }
}