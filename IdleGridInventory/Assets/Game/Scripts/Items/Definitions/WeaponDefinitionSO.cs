using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Definition", fileName = "WD_Weapon")]
public sealed class WeaponDefinitionSO : ItemDefinitionSO
{
    [Header("Stats")]
    [Min(0)]
    [SerializeField] private int damage = 1;

    [Min(0.05f)]
    [SerializeField] private float cooldownSeconds = 1f;

    public int Damage => damage;
    public float CooldownSeconds => cooldownSeconds;
}
