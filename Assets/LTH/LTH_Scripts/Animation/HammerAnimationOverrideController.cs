using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class HammerAnimationOverrideController : MonoBehaviour
{
    [System.Serializable]
    public class HammerAnimSet
    {
        public int minLevel;
        public int maxLevel;
        public AnimationClip idleClip;
        public AnimationClip blinkClip;
        public AnimationClip craftClip;
        public Sprite idleSprite;
    }

    [System.Serializable]
    public class MasteryHitEffectSet
    {
        public int minLevel;
        public List<ParticleSystem> hitEffects;
    }

    [SerializeField] private UpgradeSystem hammerUpgradeSystem;
    [SerializeField] private UpgradeSystem masteryUpgradeSystem;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip baseIdleClip;
    [SerializeField] private AnimationClip baseBlinkClip;
    [SerializeField] private AnimationClip baseCraftClip;
    [SerializeField] private HammerAnimSet[] hammerAnimSets;
    [SerializeField] private MasteryHitEffectSet[] masteryHitEffectSets;
    [FormerlySerializedAs("hammerImage")]
    [SerializeField] private Image characterImage;
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

        BindUpgradeSystemsIfNeeded();
    }

    private void Start()
    {
        ApplyHammerAnimationByLevel();
        ApplyHammerHitEffectByMasteryLevel();
    }

    private void OnEnable()
    {
        BindUpgradeSystemsIfNeeded();
        GameDataController.OnGameLoaded += HandleGameLoaded;

        if (hammerUpgradeSystem != null)
            hammerUpgradeSystem.OnLevelChanged += HandleHammerLevelChanged;

        if (masteryUpgradeSystem != null)
            masteryUpgradeSystem.OnLevelChanged += HandleMasteryLevelChanged;

        if (GameDataController.Instance != null && GameDataController.Instance.IsLoaded)
        {
            ApplyHammerAnimationByLevel();
            ApplyHammerHitEffectByMasteryLevel();
        }
    }

    private void OnDisable()
    {
        GameDataController.OnGameLoaded -= HandleGameLoaded;

        if (hammerUpgradeSystem != null)
            hammerUpgradeSystem.OnLevelChanged -= HandleHammerLevelChanged;

        if (masteryUpgradeSystem != null)
            masteryUpgradeSystem.OnLevelChanged -= HandleMasteryLevelChanged;
    }

    private void HandleGameLoaded()
    {
        ApplyHammerAnimationByLevel();
        ApplyHammerHitEffectByMasteryLevel();
    }

    private void HandleHammerLevelChanged(int level)
    {
        ApplyHammerAnimationByLevel();
    }

    private void HandleMasteryLevelChanged(int level)
    {
        ApplyHammerHitEffectByMasteryLevel();
    }

    public void ApplyHammerAnimationByLevel()
    {
        if (hammerUpgradeSystem == null || animator == null || runtimeOverrideController == null)
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

            ApplyClipOverride(baseIdleClip, set.idleClip);
            ApplyClipOverride(baseBlinkClip, set.blinkClip);
            ApplyClipOverride(baseCraftClip, set.craftClip);

            if (characterImage != null && set.idleSprite != null)
                characterImage.sprite = set.idleSprite;

            return;
        }
    }

    public void ApplyHammerHitEffectByMasteryLevel()
    {
        if (kkangKkangiAnimationController == null)
            return;

        if (masteryUpgradeSystem == null || masteryHitEffectSets == null || masteryHitEffectSets.Length == 0)
        {
            kkangKkangiAnimationController.SetHammerHitEffects(null);
            return;
        }

        int masteryLevel = masteryUpgradeSystem.CurrentLevel;
        ParticleSystem[] selectedEffects = null;
        int bestMinLevel = int.MinValue;

        for (int i = 0; i < masteryHitEffectSets.Length; i++)
        {
            MasteryHitEffectSet set = masteryHitEffectSets[i];
            if (set == null || set.hitEffects == null || set.hitEffects.Count == 0)
                continue;

            if (masteryLevel < set.minLevel || set.minLevel < bestMinLevel)
                continue;

            bestMinLevel = set.minLevel;
            selectedEffects = set.hitEffects.FindAll(effect => effect != null).ToArray();
        }

        kkangKkangiAnimationController.SetHammerHitEffects(selectedEffects);
    }

    private void BindUpgradeSystemsIfNeeded()
    {
        if (hammerUpgradeSystem != null && masteryUpgradeSystem != null)
            return;

        UpgradeSystem[] upgradeSystems = FindObjectsByType<UpgradeSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < upgradeSystems.Length; i++)
        {
            UpgradeSystem upgradeSystem = upgradeSystems[i];
            if (upgradeSystem == null)
                continue;

            if (hammerUpgradeSystem == null && upgradeSystem.SaveId == "hammer")
                hammerUpgradeSystem = upgradeSystem;

            if (masteryUpgradeSystem == null && upgradeSystem.SaveId == "mastery")
                masteryUpgradeSystem = upgradeSystem;
        }
    }

    private void ApplyClipOverride(AnimationClip baseClip, AnimationClip overrideClip)
    {
        if (baseClip == null || overrideClip == null)
            return;

        runtimeOverrideController[baseClip.name] = overrideClip;
    }
}