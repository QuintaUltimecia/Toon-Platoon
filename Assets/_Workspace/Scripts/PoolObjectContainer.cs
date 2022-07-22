using UnityEngine;

public class PoolObjectContainer : MonoBehaviour
{
    private void Awake()
    {
        Transform = transform;
    }

    public Transform Transform { get; private set; }
}
