using UnityEngine;

public class SquadPoint : MonoBehaviour
{
    [SerializeField] private RectTransform[] _points;

    public RectTransform[] Points { get => _points; }
}
