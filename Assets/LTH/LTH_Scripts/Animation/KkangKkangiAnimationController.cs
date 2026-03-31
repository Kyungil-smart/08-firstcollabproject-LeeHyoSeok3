using UnityEngine;
using System.Collections;

public class KkangKkangiAnimationController : CharacterAnimationController
{
    private Coroutine blinkCoroutine;

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

    // 눈 깜빡임 애니메이션을 주기적으로 재생하는 코루틴
    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            animator.SetTrigger("Blink");
        }
    }

    // 크래프팅 애니메이션을 재생하는 메서드
    public void PlayCraftAnimation()
    {
        StopCoroutine(blinkCoroutine);
        animator.SetTrigger("Craft");
        StartCoroutine(CraftRoutine());
    }

    // 크래프팅 애니메이션이 끝난 후 다시 눈 깜빡임 루틴을 시작하는 코루틴
    IEnumerator CraftRoutine()
    {
        yield return new WaitForSeconds(1f);
        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }
}