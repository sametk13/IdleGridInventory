using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Definition", fileName = "WD_Weapon")]
public sealed class WeaponDefinitionSO : ItemDefinitionSO
{
    [Header("Stats")]
    [Min(0)]
    [SerializeField] private int damage = 1;


    public int Damage => damage;
}
