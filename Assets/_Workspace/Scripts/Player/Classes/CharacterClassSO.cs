using UnityEngine;

[CreateAssetMenu(fileName = "Class", menuName = "Classes/Class", order = 1)]
public class CharacterClassSO : ScriptableObject
{
    [SerializeField] internal CurrentClass Class;
    [SerializeField] internal float _moveSpeed;
    [SerializeField] internal float _spreadOffset;
    [SerializeField] internal float _spreadDuration;

    public enum CurrentClass
    {
        Marine,
        Sniper,
        Heavy,
        Medic,
        Grenadier
    }
}
