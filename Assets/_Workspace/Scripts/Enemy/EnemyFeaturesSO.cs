using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/Features", order = 1)]
public class EnemyFeaturesSO : ScriptableObject
{
    [SerializeField] internal int Damage;
    [SerializeField] internal float Health;
    [SerializeField] internal float AttackRadius;
    [SerializeField] internal float AttackSpeed;
    [SerializeField] internal Vector2 LevelRange;
}
