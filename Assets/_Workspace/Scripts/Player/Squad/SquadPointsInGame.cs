using UnityEngine;

public class SquadPointsInGame : MonoBehaviour
{
    [SerializeField] private Transform[] _points;

    public Transform[] Points { get => _points; }
}
