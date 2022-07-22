using UnityEngine;

[CreateAssetMenu(fileName = "Rank", menuName = "Ranks/Rank", order = 1)]
public class RankSO : ScriptableObject
{
    [Header("Features")]
    [SerializeField] internal int _MarineHealth;
    [SerializeField] internal int _SniperHealth;
    [SerializeField] internal int _HeavyHealth;
    [SerializeField] internal int _MedicHealth;
    [SerializeField] internal int _GrenadierHealth;

    [Header("Details")]
    [SerializeField] internal string _name;
    [SerializeField] internal int _priceOfRevival;
    [SerializeField] internal int _requiredExp;
    [SerializeField] internal Sprite _sprite;
}
