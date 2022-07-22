using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Character))]
public class Follower : MonoBehaviour, IGetMoveAnimation
{
    [Header("Features")]
    [SerializeField] private float _rotateDuration = 16f;

    private Leader _lead;

    private Transform _transform;
    private Character _character;
    private NavMeshAgent _navMeshAgent;
    private CharacterController _characterController;

    private readonly float _maxDistance = 0.7f;
    private readonly float _nextOffset = 5f;
    private readonly float _maxFollowerDistance = 2.5f;
    private readonly float _maxDistanceForLeader = 5f;

    private float _distance;
    private Vector3 _moveDirection;

    private Vector3 _startTargetPosition;
    private Vector3 _nextTargetPosition;
    private Vector3 _targetPosition;

    private Quaternion _rotateDirection;

    private UnityEvent<bool> _moveEvent = new UnityEvent<bool>();

    private readonly float _moveSpeedMultiplier = 0.5f;
    private readonly float _maxMoveSpeed = 6f;
    private float _moveSpeed;
    private float _defaultMoveSpeed;

    [HideInInspector] public UnityEvent<Leader> GetLeaderEvent = new UnityEvent<Leader>();
    [HideInInspector] public UnityEvent<bool> OnActiveEvent = new UnityEvent<bool>();

    private void Awake()
    {
        _transform = transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _character = GetComponent<Character>();
        _characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        _characterController.enabled = true;
        _navMeshAgent.enabled = true;
        OnActiveEvent.Invoke(true);
    }

    private void OnDisable()
    {
        _characterController.enabled = false;
        _navMeshAgent.enabled = false;
        _moveEvent?.Invoke(false);
        if (_lead != null) _lead.RemoveFollower(this);
        OnActiveEvent.Invoke(false);
    }

    private void Start()
    {
        _moveSpeed = _character.MoveSpeed;
        _defaultMoveSpeed = _moveSpeed;

        UpdateStartTargetPosition();
        UpdateNextTargetPosition();
        _targetPosition = _transform.position;

    }

    private void Update()
    {
        if (_lead != null)
        {
            UpdateStartTargetPosition();
            UpdateNextTargetPosition();
        }

        CalculateDistance();

        Movement();
    }

    private float CalculateDirectionLeader()
    {
        float direction = Vector3.Dot(_transform.TransformDirection(Vector3.forward), _lead.Transform.position);

        if (direction < 0)
            return -1;
        else return 1;
    }

    private void MoveToEnemy()
    {
        if (_character.EnemyPool.VisibleEnemy() == true && _character.VisibilityArea.IsVisible == false)
        {
            if (Vector3.Distance(_character.AttackArea.Enemy.Transform.position, _transform.position) > _character.GunSO._attackRadius)
            {
                SetTargetPosition(_character.AttackArea.Enemy.Transform.position + (_transform.right * 4f));
            }
        }
        else if (_character.VisibilityArea.IsVisible == true)
        {
            if (Vector3.Distance(_character.AttackArea.Enemy.Transform.position, _transform.position) >= _character.GunSO._attackRadius)
            {
                SetTargetPosition(_character.AttackArea.Enemy.Transform.position);
            }
            else
            {
                SetTargetPosition(_transform.position);
            }
        }
    }

    private void Movement()
    {
        if (_distance > _maxDistance)
        {
            Rotate(Quaternion.LookRotation(_targetPosition - _transform.position));
            Run(_transform.forward * _moveSpeed);
        }
        else
        {
            Run(Vector3.zero);
        }

        if (_moveDirection == Vector3.zero)
        {
            if (_character?.AttackArea?.Enemy != null)
            {
                Rotate(Quaternion.LookRotation(_character.AttackArea.Enemy.Transform.position - _transform.position));
            }
            else
            {
                if (_lead != null) Rotate(_lead.Transform.rotation);
            }

            _moveEvent?.Invoke(false);
            _character.GunActivate(true);
        }
        else
        {
            _moveEvent?.Invoke(true);
            _character.GunActivate(false);
        }
    }

    private void Run(Vector3 direction)
    {
        _moveDirection = direction;
        _navMeshAgent.Move(_moveDirection * Time.deltaTime);
    }

    private void Rotate(Quaternion direction)
    {
        _rotateDirection = direction;

        _rotateDirection = new Quaternion(
            x: 0,
            y: _rotateDirection.y,
            z: 0,
            w: _rotateDirection.w);

        Quaternion duration = Quaternion.Slerp(
            a: _transform.rotation,
            b: _rotateDirection,
            t: _rotateDuration * Time.fixedDeltaTime);

        _transform.rotation = duration;
    }

    private void SetTargetPosition(Vector3 position) =>
        _targetPosition = position;

    private void UpdateStartTargetPosition()
    {
        _startTargetPosition = Vector3.Distance(_transform.position, _lead.Transform.position) > _maxFollowerDistance
            ? _lead.Transform.position
            : _transform.position;
    }

    private void UpdateNextTargetPosition()
    {
        if (Vector3.Distance(_lead.Transform.position, _transform.position) > _maxDistanceForLeader
            && _character?.AttackArea?.Enemy == null)
        {
            if (CalculateDirectionLeader() == 1)
            {
                if (_moveSpeed < _maxMoveSpeed)
                    _moveSpeed += _moveSpeedMultiplier * Time.deltaTime;
            }
            else
            {
                if (_moveSpeed > _defaultMoveSpeed)
                    _moveSpeed -= _moveSpeedMultiplier * Time.deltaTime;
            }

            _nextTargetPosition = Vector3.Slerp(_nextTargetPosition, _lead.Transform.position, 1f * Time.deltaTime);
        }
        else
        {
            if (_moveSpeed > _defaultMoveSpeed)
                _moveSpeed -= _moveSpeedMultiplier * Time.deltaTime;

            _nextTargetPosition = _transform.position + (_lead.Transform.forward * _nextOffset);
        }
    }

    private void CalculateDistance() =>
        _distance = Vector3.Distance(
            a: _transform.position,
            b: _targetPosition);

    public void MoveControl(bool isMove)
    {
        if (isMove == true) SetTargetPosition(_nextTargetPosition);
        else
        {
            if (_character?.AttackArea?.Enemy == null) SetTargetPosition(_startTargetPosition);
            else MoveToEnemy();
        }
    }

    public void InitLeader(Leader lead)
    {
        _lead = lead;
        GetLeaderEvent?.Invoke(_lead);
    }

    public void AddMoveEvent(UnityAction<bool> action) =>
        _moveEvent?.AddListener(action);

    public void RemoveMoveEvent(UnityAction<bool> action) =>
        _moveEvent?.RemoveListener(action);

    public Transform Transform { get => _transform; }
    public Character Character { get => _character; }
    public Leader Leader { get => _lead; }
    public Vector3 MoveDirection { get => _moveDirection; }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_targetPosition, 0.5f);
    }
}
