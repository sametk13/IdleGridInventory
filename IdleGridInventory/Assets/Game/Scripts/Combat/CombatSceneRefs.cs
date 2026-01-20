using OmniGameTemplate.Core;
using UnityEngine;


[DisallowMultipleComponent]
public sealed class CombatSceneRefs : MonoSingleton<CombatSceneRefs>
{
    [Header("Scene References")]
    [SerializeField] private EnemyRespawnController enemyRespawn;
    [SerializeField] private Transform playerThrowOrigin;

    public EnemyRespawnController EnemyRespawn => enemyRespawn;
    public Transform PlayerThrowOrigin => playerThrowOrigin;

    private void Awake()
    {
        if (enemyRespawn == null)
            Debug.LogError("[CombatSceneRefs] EnemyRespawnController is not assigned.", this);

        if (playerThrowOrigin == null)
            Debug.LogError("[CombatSceneRefs] PlayerThrowOrigin is not assigned.", this);
    }
}
