using UnityEngine;
using UnityEngine.Events;

public class EnemyGun : MonoBehaviour
{
    [SerializeField] private GameObject _gun;
    [Header("Prefab")]
    [SerializeField] private Bullet _bulletPrefab;
    public Spread Spread;

    private int _poolCount = 9;
    private PoolObjects<Bullet> _bulletPool;
    private bool _autoExpand = true;
    private PoolObjectContainer _container;

    private int _damage;

    private EnemyAI _enemyAI;
    private Transform _playerTransform;
    private Transform _bulletSpawn;

    private ParticleSystem _particleSystem;
    private Transform _particleTransform;
    private Transform _particleParent;
    private Transform _characterParent;

    private UnityEvent<bool> _attackEvent = new UnityEvent<bool>();

    private void Start()
    {
        _bulletPool = new PoolObjects<Bullet>(_bulletPrefab, _poolCount, _container.Transform);
        _bulletPool.AutoExpand = _autoExpand;
        _damage = _enemyAI.Enemy.Features.Damage;
        Spread.Rig.weight = 0;
    }

    private void CreateBullet()
    {
        var bullet = _bulletPool.GetFreeElement(_bulletSpawn.position);
        bullet.TargetTransform = _playerTransform;
        bullet.BulletSpawnTransform = _bulletSpawn;
        bullet.Damage = _damage;
        bullet.Character = _enemyAI.Enemy;
        bullet.Shoot();
    }

    public void AttackLogic()
    {
        CreateBullet();
        SetParticle(true);
    }

    public void Attack()
    {
        if (_playerTransform != null && enabled == true)
        {
            if (Vector3.Distance(_playerTransform.position, _enemyAI.Transform.position) > _enemyAI.Enemy.Features.AttackRadius)
            {
                if (_enemyAI.IsWaitForShoot == true)
                {
                    AttackLogic();
                }
                else
                {
                    SetParticle(false);
                }
            }
            else
            {
                AttackLogic();
            }
        }
    }

    public void SetTarget(Character character)
    {
        if (character != null)
        {
            if (_enemyAI.Enemy.VisibilityArea.RaycastTarget(character.Transform) == true)
            {
                _playerTransform = character.Transform;
                _enemyAI.Character = character;

                _attackEvent?.Invoke(true);
                Spread.Rig.weight = 1;
                Spread.EnableSpread();
            }
        }
        else
        {
            _playerTransform = null;
            if (_enemyAI != null) _enemyAI.Character = null;
            SetParticle(false);
            Spread.DisableSpread();
            Spread.Rig.weight = 0;
            _attackEvent?.Invoke(false);
        }
    }

    private void OnEnable()
    {
        Character character;

        try { character = _enemyAI.EnemyAttackArea.GetCurrentCharacter(); }
        catch { character = null; }

        if (character != null)
        {
            SetTarget(character);
        }
        else
        {
            SetTarget(null);
        }
    }

    private void OnDisable()
    {
        SetParticle(false);
        Spread.DisableSpread();
        Spread.Rig.weight = 0;
        _attackEvent?.Invoke(false);
    }

    private void SetParticle(bool isActive)
    {
        if (_particleSystem == null)
            return;

        if (isActive == true)
        {
            _particleTransform.position = _bulletSpawn.position;
            _particleTransform.rotation = _bulletSpawn.rotation;
            _particleSystem.Play();
        }
        else
        {
            _particleSystem.Stop();
        }
    }

    public void AddAttackEvent(UnityAction<bool> action) =>
        _attackEvent.AddListener(action);

    public void RemoveAttackEvent(UnityAction<bool> action) =>
        _attackEvent.RemoveListener(action);

    public void InitCharacter(EnemyAI value)
    {
        _enemyAI = value;
        _container = _enemyAI.PoolObjectContainer;
        _bulletSpawn = _gun.transform.Find("BulletSpawn");

        if (_bulletSpawn.childCount != 0)
        {
            _particleParent = _bulletSpawn.GetChild(0);
            _particleSystem = _particleParent.GetComponent<ParticleSystem>();
            _particleTransform = _particleSystem.transform;
            _characterParent = _enemyAI.Transform.parent;

            _particleTransform.SetParent(_characterParent);
        }
    }

    public Transform PlayerTransform { get => _playerTransform; set => _playerTransform = value; }
}
