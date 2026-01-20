using System;

public interface IHealth : IDamageable
{
    int MaxHealth { get; }
    int CurrentHealth { get; }

    bool IsDead { get; }

    event Action<int, int> HealthChanged;  // current, max
    event Action Died;

    void Heal(int amount);
    void ResetHealth();
}
