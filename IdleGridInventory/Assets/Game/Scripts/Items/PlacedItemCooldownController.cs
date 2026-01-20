using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlacedItemCooldownController : MonoBehaviour, IBrainModule
{
    private IBrain brain;

    [Header("Dependencies")]
    private float cooldownSeconds;

    private ICooldownTimer timer;

    private bool injected;
    private bool hooked;

    private bool hasStartedOnce;

    public bool HasStartedOnce => hasStartedOnce;

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

        if (timer == null)
        {
            Debug.LogError("[PlacedItemCooldownController] ICooldownTimer not found via Brain.", this);
            injected = true;
            return;
        }

        injected = true;
        TryHook();
    }

    private void TryHook()
    {
        if (!injected) return;
        if (hooked) return;
        if (!isActiveAndEnabled) return;
        if (timer == null) return;

        timer.Completed += OnCooldownCompleted;
        hooked = true;
    }

    private void Unhook()
    {
        if (!hooked) return;
        if (timer == null) return;

        timer.Completed -= OnCooldownCompleted;
        hooked = false;
    }

    // Call this when item is placed into grid for the first time.
    // This will start from 0.
    public void StartCooldown()
    {
        if (timer == null) return;

        hasStartedOnce = true;

        timer.Configure(cooldownSeconds);
        timer.StartOrResume();
    }

    // Call this when user starts dragging the item.
    public void PauseCooldown()
    {
        timer?.Pause();
    }

    // Call this when user drops the item back into grid.
    // Requirement: resume from where it left off (do NOT restart).
    public void ResumeCooldown()
    {
        timer?.StartOrResume();
    }

    // Call this when item is returned to queue area.
    // Requirement: reset cooldown, do not start, overlay should be hidden.
    public void ResetToQueue()
    {
        if (timer == null) return;

        hasStartedOnce = false;

        // ResetTimer will set IsRunning = false and raise ProgressChanged(0).
        // Presenter will hide overlay when IsRunning == false.
        timer.ResetTimer();
    }

    public void SetCooldownSeconds(float seconds)
    {
        cooldownSeconds = Mathf.Max(0.05f, seconds);
    }

    private void OnCooldownCompleted()
    {
        // Looping behavior while item is placed on grid.
        timer.ResetTimer();
        timer.Configure(cooldownSeconds);
        timer.StartOrResume();
    }
}
