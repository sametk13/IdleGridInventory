using UnityEngine;

[DisallowMultipleComponent]
public sealed class EnemyRespawnController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PrefabEnemyFactory factoryBehaviour;

    [Header("Options")]
    [SerializeField] private Transform enemyParent; // optional
    [SerializeField] private float respawnDelay = 0.0f;

    private IUnitFactory<EnemyUnit> factory;
    private EnemyUnit currentEnemy;

    public EnemyUnit CurrentEnemy => currentEnemy;

    private void Awake()
    {
        if (factoryBehaviour == null)
        {
            Debug.LogError("[EnemyRespawnController] FactoryBehaviour is not assigned.");
            enabled = false;
            return;
        }
        factory = factoryBehaviour;
    }

    private void Start()
    {
        SpawnNewEnemy();
    }

    private void OnDisable()
    {
        UnhookCurrentEnemy();
    }

    private void SpawnNewEnemy()
    {
        if (factory == null || spawnPoint == null)
            return;

        if (currentEnemy != null)
        {
            UnhookCurrentEnemy();
            Destroy(currentEnemy.gameObject);
            currentEnemy = null;
        }

        currentEnemy = factory.Create(spawnPoint.position, spawnPoint.rotation, enemyParent);
        HookCurrentEnemy();
    }

    private void HookCurrentEnemy()
    {
        if (currentEnemy == null) return;
        currentEnemy.Death += OnEnemyDeath;
    }

    private void UnhookCurrentEnemy()
    {
        if (currentEnemy == null) return;
        currentEnemy.Death -= OnEnemyDeath;
    }

    private void OnEnemyDeath(UnitBase unit)
    {
        UnhookCurrentEnemy();
        Destroy(currentEnemy.gameObject);
        currentEnemy = null;

        if (respawnDelay <= 0f)
        {
            SpawnNewEnemy();
            return;
        }

        Invoke(nameof(SpawnNewEnemy), respawnDelay);
    }

#if UNITY_EDITOR
    [ContextMenu("DEBUG: Force Respawn")]
    private void DebugForceRespawn() => SpawnNewEnemy();
#endif
}
