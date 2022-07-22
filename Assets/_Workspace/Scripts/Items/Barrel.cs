using UnityEngine;
using UnityEngine.AI;

public class Barrel : MonoBehaviour
{
    [Header("Features")]
    [SerializeField] private int _health = 1;
    [SerializeField] private int _damage = 75;
    [SerializeField] private int _damagePercent = 50;
    [SerializeField] private float _explosionRadius = 5;
    [SerializeField] private float _explosionForce = 48;
    [SerializeField] private LayerMask _targetLayer;

    [Header("Links")]
    [SerializeField] private GameObject _barrel;
    [SerializeField] private GameObject _barrelCell;
    [SerializeField] private ParticleSystem _explosionEffect;

    private Rigidbody _rigidbody;
    private NavMeshObstacle _navMeshObstacle;
    private Enemy _enemy;
    private Rigidbody[] _debris;
    private BoxCollider[] _boxColliders;
    private Transform _transform;
    private BoxCollider _boxCollider;

    private void Awake()
    {
        _transform = transform;
        _boxCollider = GetComponent<BoxCollider>();
        _enemy = GetComponent<Enemy>();
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _rigidbody = GetComponent<Rigidbody>();

        int count = _barrelCell.transform.childCount;

        _debris = new Rigidbody[count];
        _boxColliders = new BoxCollider[count];

        for (int i = 0; i < count; i++)
        {
            Transform child = _barrelCell.transform.GetChild(i);

            _debris[i] = child.GetComponent<Rigidbody>();
            _boxColliders[i] = child.GetComponent<BoxCollider>();
        }
    }

    private void OnEnable()
    {
        _enemy.AddDeathEvent(ExplosionEvent);
    }

    private void OnDisable()
    {
        _enemy.RemoveDeathEvent(ExplosionEvent);
    }

    private void Start()
    {
        _targetLayer = ~_targetLayer;

        Replace(false);
    }

    private void ExplosionEvent()
    {
        Replace(true);
        _explosionEffect.Play();
        Explosion();
    }

    private void Replace(bool isActive)
    {
        _barrelCell.SetActive(isActive);
        _barrel.SetActive(!isActive);
        _boxCollider.enabled = !isActive;
        _navMeshObstacle.enabled = !isActive;
        _rigidbody.isKinematic = true;

        for (int i = 0; i < _debris.Length; i++)
        {
            _debris[i].isKinematic = !isActive;
            _boxColliders[i].isTrigger = !isActive;
        }
    }

    private void Explosion()
    {
        Collider[] colliders = Physics.OverlapSphere(_transform.position, _explosionRadius);

        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                Vector3 enemyPosition = new Vector3(enemy.Transform.position.x, _transform.position.y, enemy.Transform.position.z);

                float distance = Mathf.Min(Vector3.Distance(enemyPosition, _transform.position), _explosionRadius);
                float damagePercent = 100 - _damagePercent;
                float onePercent;
                try
                {
                    onePercent = damagePercent / _explosionRadius;
                }
                catch (System.Exception)
                {
                    onePercent = 0;
                }
                float percent = 100 - (onePercent * distance);
                percent /= 100;

                int damage = (int)(_damage * percent);

                enemy.ApplyDamage(damage);

                if (enemy.Health == 0)
                {
                    enemy.EnemyAI?.EnemyRagdollControl.ControlRigidbody(false);

                    Collider[] colliders2 = Physics.OverlapSphere(_transform.position, _explosionRadius, _targetLayer);

                    foreach (Collider hit2 in colliders2)
                    {
                        if (hit2.TryGetComponent(out Rigidbody rigidbody))
                            rigidbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, 0, ForceMode.Impulse);
                    }
                }
            }
        }
    }

    public int Health { get => _health; private set => _health = Mathf.Max(value, 0); }
}
