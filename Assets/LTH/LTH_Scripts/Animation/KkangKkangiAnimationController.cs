using System.Collections;
using UnityEngine;

public class KkangKkangiAnimationController : CharacterAnimationController
{
    private Coroutine blinkCoroutine;

    [SerializeField] private GameObject hammerHead;

    private ParticleSystem currentHammerHitEffect;

    protected override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        if (animator == null) animator = GetComponent<Animator>();
        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    private void OnDisable()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
    }

    public void SetHammerHitEffect(ParticleSystem hammerHitEffect)
    {
        currentHammerHitEffect = hammerHitEffect;
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

        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);

        if (hammerHead != null) hammerHead.SetActive(false);

        animator.SetTrigger("Craft");
        StartCoroutine(CraftRoutine());
    }

    public void PlayHammerHitEffect()
    {
        if (!gameObject.activeInHierarchy || currentHammerHitEffect == null)
            return;

        currentHammerHitEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        currentHammerHitEffect.Play();
    }

    private IEnumerator CraftRoutine()
    {
        yield return new WaitForSeconds(1f);

        if (hammerHead != null) hammerHead.SetActive(true);

        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }
}
