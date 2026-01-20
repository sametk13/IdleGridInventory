using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CooldownTimer : MonoBehaviour, ICooldownTimer, IBrainModule
{
    private IBrain brain;
    public float Duration { get; private set; }
    public float Elapsed { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }

    public event Action<float> ProgressChanged;
    public event Action Completed;
    public void Inject(IBrain brain)
    {
        this.brain = brain;
    }
    public void Configure(float durationSeconds)
    {
        Duration = Mathf.Max(0.01f, durationSeconds);
        ResetTimer();
    }

    public void StartOrResume()
    {
        if (Duration <= 0f) return;

        IsRunning = true;
        IsPaused = false;
    }

    public void Pause()
    {
        if (!IsRunning) return;
        IsPaused = true;
    }

    public void ResetTimer()
    {
        Elapsed = 0f;
        IsRunning = false;
        IsPaused = false;

        RaiseProgressChanged(0f);
    }

    private void Update()
    {
        if (!IsRunning || IsPaused) return;

        Elapsed += Time.deltaTime;

        float progress01 = Mathf.Clamp01(Elapsed / Duration);
        RaiseProgressChanged(progress01);

        if (progress01 >= 1f)
        {
            IsRunning = false;
            Completed?.Invoke();
        }
    }

    private void RaiseProgressChanged(float progress01)
    {
        ProgressChanged?.Invoke(progress01);
    }

}
