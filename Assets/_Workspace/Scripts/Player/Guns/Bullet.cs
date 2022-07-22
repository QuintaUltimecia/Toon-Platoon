using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField] protected float _force = 5f;

    private float _duration = 1.5f;
    protected int _damage;
    //[SerializeField] private float _spreadOffset = 0.5f;

    protected Transform _transform;
    protected Rigidbody _rigidbody;
    protected GameObject _gameObject;
    protected Transform _bulletSpawnTransform;

    protected Transform _targetTransform;

    [HideInInspector] public UnityEvent<Enemy> _onDamageEvent = new UnityEvent<Enemy>();

    private void Awake()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        _gameObject = gameObject;
    }

    public virtual void Force()
    {
        _transform.position = _bulletSpawnTransform.position;
        _transform.rotation = _bulletSpawnTransform.rotation;

        _rigidbody.AddForce(_transform.TransformDirection(Vector3.forward) * _force, ForceMode.Impulse);
    }

    public virtual void Shoot()
    {
        if (_targetTransform != null)
        {
            Force();
        }

        StartCoroutine(DestroyOnTime());
    }

    private void OnEnable()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _onDamageEvent.RemoveAllListeners();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.ApplyDamage(_damage);

            if (enemy.Health == 0)
                _onDamageEvent?.Invoke(enemy);
        }

        _gameObject.SetActive(false);
    }

    public virtual void DestroyLogic()
    {
        _gameObject.SetActive(false);
    }

    private IEnumerator DestroyOnTime()
    {
        yield return new WaitForSeconds(_duration);
        DestroyLogic();
    }

    public Transform TargetTransform { set => _targetTransform = value; }
    public int Damage { get => _damage; set => _damage = value; }
    public Transform BulletSpawnTransform { get => _bulletSpawnTransform; set => _bulletSpawnTransform = value; }
    public ITransferMySelf Character { get; set; }
}
