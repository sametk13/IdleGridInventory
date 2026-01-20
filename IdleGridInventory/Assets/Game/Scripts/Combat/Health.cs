using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class Health : MonoBehaviour, IHealth
{
    [SerializeField, Min(1)] private int maxHealth = 30;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get => _currentHealth; private set => _currentHealth = value; }
    [SerializeField] private int _currentHealth;
    public bool IsDead => CurrentHealth <= 0;

    public event Action<int, int> HealthChanged;
    public event Action Died;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        RaiseHealthChanged();
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        RaiseHealthChanged();

        if (IsDead)
            Died?.Invoke();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        RaiseHealthChanged();
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        RaiseHealthChanged();
    }

    private void RaiseHealthChanged()
    {
        HealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

#if UNITY_EDITOR
    [ContextMenu("DEBUG: Damage 5")]
    private void DebugDamage5() => TakeDamage(5);
#endif
}
