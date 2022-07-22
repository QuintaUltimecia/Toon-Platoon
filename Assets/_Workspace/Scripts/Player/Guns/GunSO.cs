using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 1)]
public class GunSO : ScriptableObject
{
    [Header("Features")]
    [SerializeField] internal int _damage;
    [SerializeField] internal float _attackRadius;
    [SerializeField] internal float _attackDuration;

    [Header("Details")]
    [SerializeField] internal string _name;
    [SerializeField] internal TargetRanks _targetRank;
    [SerializeField] internal int _price;
    [SerializeField] internal Sprite _sprite;

    internal enum TargetRanks
    {
        Private,
        Corporal,
        Sergeant,
        Lieutenant,
        Captain,
        Major
    }
}
