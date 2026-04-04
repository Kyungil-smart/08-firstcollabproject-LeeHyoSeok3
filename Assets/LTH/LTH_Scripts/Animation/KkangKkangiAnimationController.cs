using System.Collections;
using UnityEngine;

public class KkangKkangiAnimationController : CharacterAnimationController
{
    private Coroutine blinkCoroutine;

    [SerializeField] private GameObject hammerHead;

    private ParticleSystem[] currentHammerHitEffects;

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

        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);

        if (hammerHead != null) hammerHead.SetActive(false);

        animator.SetTrigger("Craft");
        StartCoroutine(CraftRoutine());
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

        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }
}
