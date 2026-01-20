using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CooldownReadyPulse : MonoBehaviour, IBrainModule
{
    [Header("Target")]
    [Tooltip("If null, this GameObject's transform will be used.")]
    [SerializeField] private Transform target;

    [Header("Pulse Settings")]
    [Tooltip("Scale multiplier applied on pulse (e.g., 1.10 = 10% bigger).")]
    [Min(1f)]
    [SerializeField] private float punchMultiplier = 1.10f;

    [Tooltip("Total pulse duration (seconds).")]
    [Min(0.01f)]
    [SerializeField] private float duration = 0.18f;

    [Tooltip("If true, the pulse will be blocked while dragging (cooldown pause).")]
    [SerializeField] private bool ignoreWhileDragging = true;


    private PlacedItemCooldownController cooldownController;
    private DraggableItem draggableItem;

    private Vector3 baseScale;
    private Tween pulseTween;

    public void Inject(IBrain brain)
    {
        cooldownController = brain.Get<PlacedItemCooldownController>();
        draggableItem = brain.Get<DraggableItem>();

        if (cooldownController == null)
        {
            Debug.LogError("[CooldownReadyPulse] PlacedItemCooldownController not found via Brain.", this);
            return;
        }

        if (target == null)
            target = transform;

        baseScale = target.localScale;

        cooldownController.Ready += OnCooldownReady;
    }

    private void OnDisable()
    {
        if (cooldownController != null)
            cooldownController.Ready -= OnCooldownReady;

        KillPulse();
    }

    private void OnDestroy()
    {
        KillPulse();
    }

    private void OnCooldownReady()
    {
        if (!isActiveAndEnabled) return;
        if (target == null) return;

        if (ignoreWhileDragging && IsDragging())
            return;

        PlayPulse();
    }

    private bool IsDragging()
    {
        if (draggableItem == null) return false;

        return false;
    }

    private void PlayPulse()
    {
        KillPulse();

        target.localScale = baseScale;

        float up = duration * 0.5f;
        float down = duration - up;

        Vector3 peak = baseScale * punchMultiplier;

        pulseTween = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(target.DOScale(peak, up).SetEase(Ease.OutQuad))
            .Append(target.DOScale(baseScale, down).SetEase(Ease.InQuad))
            .OnKill(() =>
            {
                if (target != null)
                    target.localScale = baseScale;
            });
    }

    private void KillPulse()
    {
        if (pulseTween != null)
        {
            pulseTween.Kill();
            pulseTween = null;
        }
    }
}
