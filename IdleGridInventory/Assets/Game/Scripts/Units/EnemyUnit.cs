using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(Health))]
public sealed class EnemyUnit : UnitBase
{
    [SerializeField] private Health health;

    public override IHealth Health => health;

    private void Reset()
    {
        // Auto-assign in editor when component added
        health = GetComponent<Health>();
        if (health == null) health = gameObject.AddComponent<Health>();
    }

#if UNITY_EDITOR
    [ContextMenu("DEBUG: Kill")]
    private void DebugKill() => health.TakeDamage(9999);
#endif
}
