using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Definition", fileName = "WD_Weapon")]
public sealed class WeaponDefinitionSO : ItemDefinitionSO
{
    [Header("Stats")]
    [Min(0)]
    [SerializeField] private int damage = 1;

    [Header("Throw (Runtime)")]
    [Min(0.01f)]
    [SerializeField] private float throwSpeed = 6f;

    [Min(0f)]
    [SerializeField] private float arcHeight = 1.5f;

    [Range(-80f, 80f)]
    [SerializeField] private float launchAngleDegrees = 15f;

    [Min(0f)]
    [SerializeField] private float spinDegreesPerSecond = 720f;

    [Header("Projectile Visual")]
    [Tooltip("Local scale applied to spawned projectile.")]
    [SerializeField] private Vector3 projectileScale = Vector3.one;

    public int Damage => damage;
    public float ThrowSpeed => throwSpeed;
    public float ArcHeight => arcHeight;
    public float LaunchAngleDegrees => launchAngleDegrees;
    public float SpinDegreesPerSecond => spinDegreesPerSecond;
    public Vector3 ProjectileScale => projectileScale;
}
