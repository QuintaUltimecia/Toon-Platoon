using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Turret : MonoBehaviour
{
    [Header("Features")]
    private int _damage;
    private float _attackDuration;
    [SerializeField] private float _rotateDuration = 4f;
    [Header("Links")]
    [SerializeField] private TurretAttackArea _attackArea;
    [SerializeField] private Transform _bulletSpawn;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private ParticleSystem _sleeveParticle;
    [Header("Components")]
    [SerializeField] private GameObject _turretGun;
    [SerializeField] private GameObject _turret;
    [SerializeField] private GameObject _turretGunDestroyed;
    [SerializeField] private GameObject _turretDestroyed;

    [Header("Prefab")]
    [SerializeField] private Bullet _bulletPrefab;

    private Transform _turretGunTransform;
    private Rigidbody _turrerGunDestroyedRigidbody;
    private CharacterController _characterController;

    private VisibilityArea _visibilityArea;
    private Enemy _enemy;

    private int _poolCount = 9;
    private PoolObjects<Bullet> _bulletPool;
    private bool _autoExpand = true;

    private Transform _playerTransform;
    private Transform _transform;

    private Transform _particleTransform;

    private UnityEvent<bool> _attackEvent = new UnityEvent<bool>();

    public Character Character { get; set; }

    private Coroutine _attackRoutine;

    private void Awake()
    {
        _transform = transform;
        _visibilityArea = GetComponent<VisibilityArea>();
        _particleTransform = _particleSystem.transform;
        _enemy = GetComponent<Enemy>();
        _turretGunTransform = _turretGun.transform;
        _turrerGunDestroyedRigidbody = _turretGunDestroyed.GetComponent<Rigidbody>();
        _turrerGunDestroyedRigidbody.isKinematic = true;
        _characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        _enemy.AddDeathEvent(OnDeath);
    }

    private void OnDisable()
    {
        _enemy.RemoveDeathEvent(OnDeath);
    }

    private void Start()
    {
        _bulletPool = new PoolObjects<Bullet>(_bulletPrefab, _poolCount, _bulletSpawn);
        _bulletPool.AutoExpand = _autoExpand;
        _attackDuration = _enemy.Features.AttackSpeed;
        _damage = _enemy.Features.Damage;
        _attackArea.Radius = _enemy.Features.AttackRadius;
        DestroyEffect(false);
    }

    private void Update()
    {
        if (Character != null)
        {
            Vector3 direction = _visibilityArea.LastPoint - _transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            Rotate(rotation);
        }
    }

    private void Rotate(Quaternion direction)
    {
        direction = new Quaternion(
            x: 0,
            y: direction.y,
            z: 0,
            w: direction.w);

        Quaternion duration = Quaternion.Slerp(
            a: _turretGunTransform.rotation,
            b: direction,
            t: _rotateDuration * Time.fixedDeltaTime);

        _turretGunTransform.rotation = duration;
    }

    private void CreateBullet()
    {
        var bullet = _bulletPool.GetFreeElement(_bulletSpawn.position);
        bullet.TargetTransform = _playerTransform;
        bullet.BulletSpawnTransform = _bulletSpawn;
        bullet.Damage = _damage;
        bullet.Character = Character;
        bullet.Shoot();
    }

    public void AttackLogic()
    {
        CreateBullet();
        SetParticle(true);
    }

    public IEnumerator Attack()
    {
        while(_playerTransform != null)
        {
            if (Vector3.Distance(_playerTransform.position, _transform.position) > _enemy.Features.AttackRadius)
            {
                SetParticle(false);
                _sleeveParticle.Stop();
            }
            else
            {
                if (_visibilityArea.RaycastTarget(Character.Transform) == true)
                    AttackLogic();
            }

            yield return new WaitForSeconds(_attackDuration);
        }
    }

    public void SetTarget(Character character)
    {
        if (character != null)
        {
            _playerTransform = character.Transform;
            Character = character;

            _attackEvent?.Invoke(true);
            _attackRoutine = StartCoroutine(Attack());
            _sleeveParticle.Play();
        }
        else
        {
            _playerTransform = null;
            Character = null;
            SetParticle(false);
            _attackEvent?.Invoke(false);
            StopCoroutine(_attackRoutine);
            _sleeveParticle.Stop();
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

    private void OnDeath()
    {
        SetTarget(null);

        _attackArea.enabled = false;
        _attackArea.gameObject.SetActive(false);
        _characterController.enabled = false;

        DestroyEffect(true);

        _turretGunDestroyed.transform.rotation = _turretGunTransform.rotation;
        _turrerGunDestroyedRigidbody.isKinematic = false;
        _turrerGunDestroyedRigidbody.AddForce(_turrerGunDestroyedRigidbody.transform.TransformDirection(Vector3.up) * 3f, ForceMode.Impulse);
        _turrerGunDestroyedRigidbody.AddForce(_turrerGunDestroyedRigidbody.transform.TransformDirection(Vector3.left) * 1.5f, ForceMode.Impulse);
    }

    private void DestroyEffect(bool isActive)
    {
        if (isActive == true)
        {
            _turretGun.SetActive(false);
            _turret.SetActive(false);
            _turretDestroyed.SetActive(true);
            _turretGunDestroyed.SetActive(true);
        }
        else
        {
            _turretGun.SetActive(true);
            _turret.SetActive(true);
            _turretDestroyed.SetActive(false);
            _turretGunDestroyed.SetActive(false);
        }
    }

    public void PlayerDeathEvent(Character character)
    {
        _attackArea.RemoveCharacterPool(character);

        if (_attackArea.GetCurrentCharacter() == null)
        {
            SetTarget(null);
        }
        else
        {
            SetTarget(_attackArea.GetCurrentCharacter());
        }

        print("Убрать юнита");
    }

    public void AddAttackEvent(UnityAction<bool> action) =>
        _attackEvent.AddListener(action);

    public void RemoveAttackEvent(UnityAction<bool> action) =>
        _attackEvent.RemoveListener(action);

    public Transform PlayerTransform { get => _playerTransform; set => _playerTransform = value; }
}

