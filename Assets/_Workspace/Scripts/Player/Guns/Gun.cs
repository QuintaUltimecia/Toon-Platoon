using UnityEngine;
using UnityEngine.Events;

public abstract class Gun : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private GameObject[] _guns;
    public Spread Spread;

    [Header("Prefab")]
    [SerializeField] protected Bullet _bulletPrefab;

    private int _poolCount = 9;
    private PoolObjects<Bullet> _bulletPool;
    private PoolObjectContainer _container;

    private int _damage;

    protected Character _character;
    private Enemy _enemy;
    private Transform _bulletSpawn;

    private ParticleSystem _particleSystem;
    private Transform _particleTransform;
    private Transform _particleParent;
    private Transform _characterParent;

    private UnityEvent<bool> _attackEvent = new UnityEvent<bool>();

    private void Start()
    {
        _bulletPool = new PoolObjects<Bullet>(_bulletPrefab, _poolCount, _container.Transform);
        _bulletPool.AutoExpand = true;
        Spread.Rig.weight = 0;
        Spread.SetOffset(_character.ClassSO._spreadOffset);
        Spread.SetDuration(_character.ClassSO._spreadDuration);
    }

    public void VisualizeGun(int number, bool isActive)
    {
        _guns[number].SetActive(isActive);
    }

    public void InitGun(int value)
    {
        for (int i = 0; i < _guns.Length; i++)
        {
            if (i == value)
            {
                _guns[i].SetActive(true);
                _bulletSpawn = _guns[i].transform.Find("BulletSpawn");
            }
            else _guns[i].SetActive(false);
        }

        if (_bulletSpawn.childCount != 0)
        {
            _particleParent = _bulletSpawn.GetChild(0);
            _particleSystem = _particleParent.GetComponent<ParticleSystem>();
            _particleTransform = _particleSystem.transform;
            _characterParent = _character.Transform.parent;

            _particleTransform.SetParent(_characterParent);
        }

        _damage = _character.GunSO._damage;
    }

    private void CreateBullet()
    {
        var bullet = _bulletPool.GetFreeElement(_bulletSpawn.position);
        bullet.TargetTransform = _enemy.Transform;
        bullet.BulletSpawnTransform = _bulletSpawn;
        bullet.Damage = _damage;
        bullet.Character = _character;
        bullet._onDamageEvent.AddListener(_character.AttackArea.RemoveEnemy);
        bullet.Shoot();
    }

    public virtual void AttackLogic()
    {
        CreateBullet();
    }

    public void Attack()
    {
        if (_enemy != null && enabled == true)
        {
            if (Vector3.Distance(_enemy.Transform.position, _character.Transform.position) > _character.GunSO._attackRadius) 
            {
                SetParticle(false);
            }
            else
            {
                AttackLogic();

                SetParticle(true);
            }
        }
    }

    public void SetTarget(Enemy enemy)
    {
        if (enemy != null && enabled == true)
        {
            if (_character.VisibilityArea.RaycastTarget(enemy.Transform) == true
                || _character.EnemyPool.VisibleEnemy() == true)
            {
                _enemy = enemy;
                _character.AttackArea.Enemy = enemy;
                enemy.AddRemoveEnemyEvent(_character.AttackArea.RemoveEnemy);

                _attackEvent?.Invoke(true);
                if (_character.ClassSO._spreadOffset != 0) Spread.Rig.weight = 1;
                Spread.EnableSpread();
                _character?.FaceActive(false);
            }
        }
        else if (enabled == true)
        {
            _enemy = null;

            SetParticle(false);
            Spread.DisableSpread();
            Spread.Rig.weight = 0;
            _attackEvent?.Invoke(false);
            _character?.FaceActive(true);
        }
    }

    private void OnEnable()
    {
        Enemy enemy;

        if (_character?.AttackArea?.Enemy != null && _character.EnemyPool?.Target() != null)
        {
            enemy = _character.AttackArea.Enemy;
        }
        else
        {
            try { enemy = _character.EnemyPool.Target(); }
            catch { enemy = null; }
        }

        if (enemy != null)
        {
            SetTarget(enemy);
        }
        else
        {
            SetTarget(null);
        }
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

    private void OnDisable()
    {
        SetParticle(false);
        Spread.DisableSpread();
        Spread.Rig.weight = 0;
        _attackEvent?.Invoke(false);
    }

    public void AddAttackEvent(UnityAction<bool> action) =>
        _attackEvent.AddListener(action);

    public void RemoveAttackEvent(UnityAction<bool> action) =>
        _attackEvent.RemoveListener(action);

    public void InitCharacter(Character value)
    {
        _character = value;
        _container = _character.PoolObjectContainer;
    }

    public Enemy Enemy{ get => _enemy; set => _enemy = value; }
}
