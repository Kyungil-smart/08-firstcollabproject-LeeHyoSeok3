using UnityEngine;
using UnityEngine.UI;

public class HammerAnimationOverrideController : MonoBehaviour
{
    [System.Serializable]
    public class HammerAnimSet
    {
        public int minLevel;
        public int maxLevel;
        public AnimationClip craftClip;
        public Sprite idleSprite;
    }

    [SerializeField] private UpgradeSystem hammerUpgradeSystem;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip baseCraftClip;
    [SerializeField] private HammerAnimSet[] hammerAnimSets;
    [SerializeField] private Image hammerImage;

    private AnimatorOverrideController runtimeOverrideController;


    private void Awake()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            runtimeOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = runtimeOverrideController;
        }
    }

    private void Start()
    {
        ApplyHammerAnimationByLevel();
    }

    private void OnEnable()
    {
        GameDataController.OnGameLoaded += HandleGameLoaded;
    }

    private void OnDisable()
    {
        GameDataController.OnGameLoaded -= HandleGameLoaded;
    }

    private void HandleGameLoaded()
    {
        Debug.Log("[HAMMER_ANIM] 게임 로드 완료 후 재적용");
        ApplyHammerAnimationByLevel();
    }

    public void ApplyHammerAnimationByLevel()
    {
        if (hammerUpgradeSystem == null || animator == null || runtimeOverrideController == null || baseCraftClip == null)
        {
            Debug.LogWarning($"[HAMMER_ANIM] 참조 누락 - {gameObject.name}");
            return;
        }

        int level = hammerUpgradeSystem.CurrentLevel;
        Debug.Log($"[HAMMER_ANIM] 현재 레벨 = {level}, baseCraftClip = {baseCraftClip.name}");

        for (int i = 0; i < hammerAnimSets.Length; i++)
        {
            HammerAnimSet set = hammerAnimSets[i];
            Debug.Log($"[HAMMER_ANIM] 현재 레벨 = {level}, 대상 = {gameObject.name}");

            if (level >= set.minLevel && level <= set.maxLevel)
            {
                if (set.craftClip != null)
                {
                    runtimeOverrideController[baseCraftClip.name] = set.craftClip;
                    Debug.Log($"[HAMMER_ANIM] 적용됨: {set.craftClip.name} / 대상 = {gameObject.name}");
                }

                if (hammerImage != null && set.idleSprite != null)
                {
                    hammerImage.sprite = set.idleSprite;
                }

                return;
            }
        }
    }
}
