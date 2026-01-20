using System;

public interface ICooldownTimer
{
    float Duration { get; }
    float Elapsed { get; }
    bool IsRunning { get; }
    bool IsPaused { get; }

    // progress01: 0 -> just started, 1 -> ready
    event Action<float> ProgressChanged;
    event Action Completed;

    void Configure(float durationSeconds);
    void StartOrResume();
    void Pause();
    void ResetTimer();
}
