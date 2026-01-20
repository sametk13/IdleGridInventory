using UnityEngine;

public sealed class HealthBarPresenter : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private EnemyUnit targetEnemy;
    [SerializeField] private HealthBarView view;

    private IHealth health;

    private void Awake()
    {
        if(targetEnemy == null)
        {
            Debug.LogError("[HealthBarPresenter] targetEnemy null");
        }
        if (targetEnemy == null)
        {
            Debug.LogError("[HealthBarPresenter] view null");
        }

        Bind(targetEnemy);
    }

    public void Bind(EnemyUnit enemy)
    {
        Unbind();

        targetEnemy = enemy;
        if (targetEnemy == null || view == null) return;
        health = targetEnemy.Health;
        if (health == null) return;
        health.HealthChanged += OnHealthChanged;
        health.Died += OnDied;

        // Init
        OnHealthChanged(health.CurrentHealth, health.MaxHealth);
        view.SetVisible(true);
    }

    private void Unbind()
    {
        if (health == null) return;
        health.HealthChanged -= OnHealthChanged;
        health.Died -= OnDied;
        health = null;
    }

    private void OnDestroy()
    {
        Unbind();
    }

    private void OnHealthChanged(int current, int max)
    {
        float n = max <= 0 ? 0f : (float)current / max;
        view.SetNormalized(n);
    }

    private void OnDied()
    {
        view.SetVisible(false);
    }
}
