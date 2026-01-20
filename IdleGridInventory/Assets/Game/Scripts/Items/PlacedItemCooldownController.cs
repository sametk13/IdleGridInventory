using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlacedItemCooldownController : MonoBehaviour, IBrainModule
{
    private IBrain brain;

    [Header("Dependencies")]
    private float cooldownSeconds = 2f;

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

    // Call this when item is placed into grid
    public void StartCooldown()
    {
        if (timer == null) return;

        timer.Configure(cooldownSeconds);
        timer.StartOrResume();
    }

    // Call this when user starts dragging the item
    public void PauseCooldown()
    {
        timer?.Pause();
    }

    // Call this when user drops the item back into grid
    public void ResumeCooldown()
    {
        timer?.StartOrResume();
    }

    public void SetCooldownSeconds(float seconds)
    {
        cooldownSeconds = Mathf.Max(0.05f, seconds);
    }

    private void OnCooldownCompleted()
    {
        // This step only handles looping UI timer.
        // Projectile firing will be handled in step 6 (combat/fire system).
        timer.ResetTimer();
        timer.Configure(cooldownSeconds);
        timer.StartOrResume();
    }
}
