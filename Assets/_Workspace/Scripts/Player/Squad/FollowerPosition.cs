using UnityEngine;

public class FollowerPosition : MonoBehaviour
{
    [SerializeField] private Transform _follower;

    public Transform Follower { get => _follower; }
}
