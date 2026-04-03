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
        public ParticleSystem hitEffect;
    }

    [SerializeField] private UpgradeSystem hammerUpgradeSystem;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip baseCraftClip;
    [SerializeField] private HammerAnimSet[] hammerAnimSets;
    [SerializeField] private Image hammerImage;
    [SerializeField] private KkangKkangiAnimationController kkangKkangiAnimationController;

    private AnimatorOverrideController runtimeOverrideController;

    private void Awake()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            runtimeOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = runtimeOverrideController;
        }

        if (kkangKkangiAnimationController == null)
            kkangKkangiAnimationController = GetComponent<KkangKkangiAnimationController>();
    }

    private void Start()
    {
        ApplyHammerAnimationByLevel();
    }

    private void OnEnable()
    {
        GameDataController.OnGameLoaded += HandleGameLoaded;

        if (hammerUpgradeSystem != null)
            hammerUpgradeSystem.OnLevelChanged += HandleHammerLevelChanged;

        if (GameDataController.Instance != null && GameDataController.Instance.IsLoaded)
            ApplyHammerAnimationByLevel();
    }

    private void OnDisable()
    {
        GameDataController.OnGameLoaded -= HandleGameLoaded;

        if (hammerUpgradeSystem != null)
            hammerUpgradeSystem.OnLevelChanged -= HandleHammerLevelChanged;
    }

    private void HandleGameLoaded()
    {
        ApplyHammerAnimationByLevel();
    }

    private void HandleHammerLevelChanged(int level)
    {
        ApplyHammerAnimationByLevel();
    }

    public void ApplyHammerAnimationByLevel()
    {
        if (hammerUpgradeSystem == null || animator == null || runtimeOverrideController == null || baseCraftClip == null)
        {
            Debug.LogWarning($"[HAMMER_ANIM] Missing reference on {gameObject.name}");
            return;
        }

        int level = hammerUpgradeSystem.CurrentLevel;

        for (int i = 0; i < hammerAnimSets.Length; i++)
        {
            HammerAnimSet set = hammerAnimSets[i];

            if (level < set.minLevel || level > set.maxLevel)
                continue;

            if (set.craftClip != null)
                runtimeOverrideController[baseCraftClip.name] = set.craftClip;

            if (hammerImage != null && set.idleSprite != null)
                hammerImage.sprite = set.idleSprite;

            if (kkangKkangiAnimationController != null)
                kkangKkangiAnimationController.SetHammerHitEffect(set.hitEffect);

            return;
        }

        if (kkangKkangiAnimationController != null)
            kkangKkangiAnimationController.SetHammerHitEffect(null);
    }
}
