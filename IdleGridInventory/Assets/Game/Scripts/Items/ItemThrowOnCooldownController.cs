using UnityEngine;

[DisallowMultipleComponent]
public sealed class ItemThrowOnCooldownController : MonoBehaviour, IBrainModule
{
    [Header("Dependencies")]
    [SerializeField] private ThrownItemProjectile2D projectilePrefab;

    private DraggableItem draggableItem;
    private PlacedItemCooldownController cooldownController;

    public void Inject(IBrain brain)
    {
        draggableItem = brain.Get<DraggableItem>();
        cooldownController = brain.Get<PlacedItemCooldownController>();

        if (draggableItem == null)
            Debug.LogError("[ItemThrowOnCooldownController] DraggableItem not found via Brain.", this);

        if (cooldownController == null)
            Debug.LogError("[ItemThrowOnCooldownController] PlacedItemCooldownController not found via Brain.", this);

        if (projectilePrefab == null)
            Debug.LogError("[ItemThrowOnCooldownController] Projectile prefab is not assigned.", this);

        if (cooldownController != null)
            cooldownController.Ready += OnCooldownReady;
    }

    private void OnDisable()
    {
        if (cooldownController != null)
            cooldownController.Ready -= OnCooldownReady;
    }

    private void OnCooldownReady()
    {
        if (CombatSceneRefs.Instance == null)
        {
            Debug.LogError("[ItemThrowOnCooldownController] CombatSceneRefs.Instance is missing in scene.");
            return;
        }

        EnemyRespawnController enemyRespawn = CombatSceneRefs.Instance.EnemyRespawn;
        Transform throwOrigin = CombatSceneRefs.Instance.PlayerThrowOrigin;

        if (enemyRespawn == null || throwOrigin == null)
            return;

        EnemyUnit enemy = enemyRespawn.CurrentEnemy;
        if (enemy == null)
            return; // Requirement: if no enemy, do not throw.

        WeaponDefinitionSO weapon = draggableItem != null ? draggableItem.Definition as WeaponDefinitionSO : null;
        if (weapon == null)
        {
            Debug.LogError("[ItemThrowOnCooldownController] Item definition is not WeaponDefinitionSO.", this);
            return;
        }

        if (enemy.Health == null)
            return;

        Vector3 start = throwOrigin.position;
        Vector3 target = enemy.transform.position;

        ThrownItemProjectile2D proj = Instantiate(projectilePrefab);
        proj.name = $"Projectile_{weapon.ItemId}";

        proj.Launch(
            sprite: weapon.Icon,
            startWorld: start,
            targetWorld: target,
            projectileScale: weapon.ProjectileScale,
            speedUnitsPerSecond: weapon.ThrowSpeed,
            arcHeight: weapon.ArcHeight,
            launchAngleDegrees: weapon.LaunchAngleDegrees,
            spinDegreesPerSecond: weapon.SpinDegreesPerSecond,
            onArrived: () =>
            {
                EnemyUnit current = enemyRespawn.CurrentEnemy;
                if (current != null && current == enemy && current.Health != null)
                {
                    current.Health.TakeDamage(weapon.Damage);
                }
            });
    }
}
