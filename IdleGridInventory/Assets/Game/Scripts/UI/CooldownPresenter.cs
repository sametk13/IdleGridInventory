using UnityEngine;

public sealed class CooldownPresenter : MonoBehaviour, IBrainModule
{
    private CooldownOverlayView view;
    private IBrain brain;

    private ICooldownTimer timer;

    private bool injected;
    private bool hooked;

    private void OnEnable()
    {
        TryHook();
    }

    private void OnDisable()
    {
        Unhook();
    }

    public void Inject(IBrain brain)
    {
        this.brain = brain;

        timer = brain.Get<ICooldownTimer>();
        view = brain.Get<CooldownOverlayView>();

        if (view == null)
            Debug.LogError("[CooldownPresenter] CooldownOverlayView reference is null.", this);

        if (timer == null)
            Debug.LogError("[CooldownPresenter] ICooldownTimer reference is null.", this);

        injected = true;
        TryHook();
    }

    private void TryHook()
    {
        if (!injected) return;
        if (hooked) return;
        if (!isActiveAndEnabled) return;
        if (timer == null || view == null) return;

        timer.ProgressChanged += OnProgressChanged;
        timer.Completed += OnCompleted;

        hooked = true;

        // Init state
        OnProgressChanged(timer.Duration <= 0f ? 0f : Mathf.Clamp01(timer.Elapsed / timer.Duration));
    }

    private void Unhook()
    {
        if (!hooked) return;
        if (timer == null) return;

        timer.ProgressChanged -= OnProgressChanged;
        timer.Completed -= OnCompleted;

        hooked = false;
    }

    private void OnProgressChanged(float progress01)
    {
        // If timer is not running, we consider this "idle" state (e.g., in queue).
        // Requirement: overlay must be hidden and reset in idle.
        if (!timer.IsRunning)
        {
            view.SetCooldownFill01(0f);
            view.SetVisible(false);
            return;
        }

        // progress01: 0 -> started, 1 -> ready
        // requirement: overlay fillAmount 1 -> 0
        float overlayFill = 1f - progress01;
        view.SetCooldownFill01(overlayFill);
        view.SetVisible(true);
    }

    private void OnCompleted()
    {
        // When ready, overlay should be 0
        view.SetCooldownFill01(0f);
        view.SetVisible(true);
    }
}
