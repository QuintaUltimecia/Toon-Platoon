using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Events;
using DG.Tweening;

public class EnemyAI : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private EnemyGun _gun;
    [SerializeField] private EnemyAttackArea _enemyAttackArea;
    [SerializeField] private EnemyRagdollControl _enemyRagdollControl;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

    [Header("Features")]
    [SerializeField] private float _reshadeDuration = 1f;
    [SerializeField] private float _timeToReshade = 5f;

    private PoolObjectContainer _poolObjectContainer;

    private readonly float _moveSpeed = 3f;
    private readonly float _rotateDuration = 8f;

    private float _distance;
    private float _distanceToPlayer;
    private bool _isMove = true;

    private Character _character;
    private Transform _transform;
    private NavMeshAgent _navMeshAgent;
    private Enemy _enemy;
    private GameObject _gameObject;

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private UnityEvent<bool> _moveEvent = new UnityEvent<bool>();

    private bool _isShooting = false;
    private bool _isStartTimer = false;
    private bool _isWaitForShoot = false;

    private Coroutine _timerRoutine;
    private Coroutine _waitForShootRoutine;

    private Material[] _materials;
    private Shader _shader;

    private void Awake()
    {
        _poolObjectContainer = transform.parent.Find(nameof(PoolObjectContainer)).GetComponent<PoolObjectContainer>();
        _enemyAttackArea.InitEnemyAI(this);
        _transform = transform;
        _gameObject = gameObject;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _enemy = GetComponent<Enemy>();
        _gun.InitCharacter(this);

        _materials = new Material[_skinnedMeshRenderer.materials.Length];

        for (int i = 0; i < _materials.Length; i++)
            _materials[i] = _skinnedMeshRenderer.materials[i];

        _shader = Shader.Find("4U Games/Toon Fade");
    }

    private void OnEnable()
    {
        _enemy.AddDeathEvent(DeathEvent);
        _moveEvent.AddListener(GunEnabled);
    }

    private void OnDisable()
    {
        _enemy.RemoveDeathEvent(DeathEvent);
        _moveEvent.RemoveListener(GunEnabled);
    }

    private void Start()
    {
        _startPosition = _transform.position;
        _startRotation = _transform.rotation;
    }

    private void Update()
    {
        if (Character != null)
        {
            if (_enemy.VisibilityArea.RaycastTarget(Character.Transform) == true)
                if (_isMove == true) MoveToCharacter();
            else
            {
                if (_enemy.VisibilityArea.LastPoint != Vector3.zero)
                    if (_isMove == true) MoveToLastPoint();
                else
                    RemoveCharacter();
            }
        }
        else if (_isMove == true) MoveToStartPosition();

        CalculateDistance();

        if (Character != null)
            CalculateDistanceToPlayer();
    }

    private void Rotate(Quaternion direction)
    {
        direction = new Quaternion(
            x: 0,
            y: direction.y,
            z: 0,
            w: direction.w);

        Quaternion duration = Quaternion.Slerp(
            a: _transform.rotation,
            b: direction,
            t: _rotateDuration * Time.fixedDeltaTime);

        _transform.rotation = duration;
    }

    private void CalculateDistance() =>
        _distance = Vector3.Distance(_transform.position, _startPosition);

    private void CalculateDistanceToPlayer() =>
        _distanceToPlayer = Vector3.Distance(_transform.position, Character.transform.position);

    private void MoveToStartPosition()
    {
        if (_distance > 0.5f)
        {
            _navMeshAgent.Move(_transform.forward * _moveSpeed * Time.deltaTime);
            Rotate(Quaternion.LookRotation(_startPosition - _transform.position));
            _moveEvent?.Invoke(true);
        }
        else
        {
            _moveEvent?.Invoke(false);
            Rotate(_startRotation);
        }
    }

    private void MoveToLastPoint()
    {
        Vector3 direction = _enemy.VisibilityArea.LastPoint - _transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Rotate(rotation);

        _navMeshAgent.Move(_transform.forward * _moveSpeed * Time.deltaTime);
        _moveEvent?.Invoke(true);
    }

    private void MoveToCharacter()
    {
        Vector3 direction = Character.Transform.position - _transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Rotate(rotation);

        if (_distanceToPlayer > Enemy.Features.AttackRadius)
        {
            if (_isStartTimer == false)
            {
                _isStartTimer = true;
                _timerRoutine = StartCoroutine(Timer());
            }

            if (_isWaitForShoot == false && _isShooting == true)
            {
                _isWaitForShoot = true;
                _waitForShootRoutine = StartCoroutine(WaitForTheShooting());
            }

            if (_isShooting == false)
            {
                _navMeshAgent.Move(_transform.forward * _moveSpeed * Time.deltaTime);
                _moveEvent?.Invoke(true);
            }
        }
        else
        {
            if (_timerRoutine != null)
            {
                StopCoroutine(_timerRoutine);
                _timerRoutine = null;
                _isStartTimer = false;
            }

            if (_waitForShootRoutine != null)
            {
                StopCoroutine(_waitForShootRoutine);
                _waitForShootRoutine = null;
                _isWaitForShoot = false;
            }

            _moveEvent?.Invoke(false);
            _isShooting = true;
        }
    }

    private IEnumerator WaitForTheShooting()
    {
        yield return new WaitForSeconds(1f);
        _isShooting = false;
        _isWaitForShoot = false;
    }

    private void GunEnabled(bool isActive) =>
        _gun.enabled = !isActive;

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(5f);
        RemoveCharacter();
    }

    private void RemoveCharacter()
    {
        _isStartTimer = false;
        _isShooting = false;

        EnemyAttackArea.RemoveCharacterPool(_character);
        _character = null;
        _enemy.VisibilityArea.LastPoint = Vector3.zero;
    }

    public void PlayerDeathEvent(Character character)
    {
        _isShooting = false;
        _enemyAttackArea.RemoveCharacterPool(character);

        if (_enemyAttackArea.GetCurrentCharacter() == null)
        {
            Gun.SetTarget(null);
        }
        else
        {
            Gun.SetTarget(EnemyAttackArea.GetCurrentCharacter());
        }

        print("Убрать юнита");
    }

    private void DeathEvent()
    {
        _isShooting = false;
        _character = null;
        _enemyAttackArea.RemoveAllCharacter();
        GunEnabled(false);
        _enemyAttackArea.GameObject.SetActive(false);
        _navMeshAgent.enabled = false;
        _isMove = false;

        StartCoroutine(ToReshadeRoutine());
    }

    private IEnumerator ToReshadeRoutine()
    {
        yield return new WaitForSeconds(_timeToReshade);
        Reshade();
    }

    private void DestroyObject() =>
        Destroy(_gameObject);

    private void Reshade()
    {
        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i].shader = _shader;
            Color color = _materials[i].color;
            _materials[i].DOColor(new Color(color.r, color.g, color.b, 0), _reshadeDuration).SetEase(Ease.Linear).OnComplete(DestroyObject);
        }
    }

    public void AddMoveEvent(UnityAction<bool> action) =>
        _moveEvent?.AddListener(action);

    public void RemoveMoveEvent(UnityAction<bool> action) =>
        _moveEvent?.RemoveListener(action);

    public Character Character
    {
        get => _character;
        set => _character = value;
    }

    public EnemyGun Gun { get => _gun; }
    public Transform Transform { get => _transform; }
    public EnemyRagdollControl EnemyRagdollControl { get => _enemyRagdollControl; }
    public Enemy Enemy { get => _enemy; }
    public bool IsShooting { get => _isShooting; }
    public EnemyAttackArea EnemyAttackArea { get => _enemyAttackArea; }
    public PoolObjectContainer PoolObjectContainer { get => _poolObjectContainer; }
    public bool IsWaitForShoot { get => _isWaitForShoot; }
}
