using System;
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

    /// <summary>
    /// Fired when cooldown reaches ready state (completed).
    /// </summary>
    public event Action Ready;

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

    public void SetCooldownSeconds(float seconds)
    {
        cooldownSeconds = Mathf.Max(0.05f, seconds);
    }

    public void StartCooldown()
    {
        if (timer == null) return;

        hasStartedOnce = true;

        timer.Configure(cooldownSeconds);
        timer.StartOrResume();
    }

    public void PauseCooldown()
    {
        timer?.Pause();
    }

    public void ResumeCooldown()
    {
        if (timer == null) return;

        // If it was never started, Resume is meaningless; keep safe behavior.
        if (!hasStartedOnce)
        {
            StartCooldown();
            return;
        }

        timer.StartOrResume();
    }

    public void ResetToQueue()
    {
        if (timer == null) return;

        hasStartedOnce = false;
        timer.ResetTimer();
    }

    private void OnCooldownCompleted()
    {
        Ready?.Invoke();

        // Requirement: keep the cooldown looping automatically.
        if (timer == null) return;

        timer.ResetTimer();
        timer.Configure(cooldownSeconds);
        timer.StartOrResume();
    }
}
