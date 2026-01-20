using UnityEngine;

public sealed class PrefabEnemyFactory : MonoBehaviour, IUnitFactory<EnemyUnit>
{
    [Header("Enemy Prefabs")]
    [SerializeField] private EnemyUnit[] enemyPrefabs;

    public EnemyUnit Create(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("[PrefabEnemyFactory] No enemy prefabs assigned.");
            return null;
        }

        var prefab = PickPrefab();
        return Instantiate(prefab, position, rotation, parent);
    }

    private EnemyUnit PickPrefab()
    {
        if (enemyPrefabs.Length == 1)
            return enemyPrefabs[0];

        int index = Random.Range(0, enemyPrefabs.Length);
        return enemyPrefabs[index];
    }
}
