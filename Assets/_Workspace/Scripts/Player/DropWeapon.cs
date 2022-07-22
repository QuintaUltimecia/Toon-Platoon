using UnityEngine;

public class DropWeapon : MonoBehaviour
{
    [SerializeField] private Character _character;

    private Rigidbody _rigidbody;
    private Collider _collider;
    private Transform _transform;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _transform = transform;
    }

    private void OnEnable() =>
        _character.AddDeathEvent(OnDeathEvent);

    private void OnDisable() =>
        _character.RemoveDeathEvent(OnDeathEvent);

    private void Start()
    {
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
    }

    private void OnDeathEvent()
    {
        _transform.parent = null;
        _collider.enabled = true;
        _rigidbody.isKinematic = false;
    }
}
