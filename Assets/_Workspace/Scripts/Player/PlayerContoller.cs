using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class PlayerContoller : MonoBehaviour, IGetMoveAnimation, IGetTargetPosition
{
    public static System.Action OnMoveStarted;
    public System.Action OnMoveStopped;
    public System.Action OnFingerUpped;

    public CharacterController Controller;
    public float MoveSpeed;
    public float AngularSpeed;
    public float StickOffset;
    private bool _updateRotation = true;
    private Transform _transform;
    private Character _character;
    private Leader _lead;
    private NavMeshAgent _navMeshAgent;

    public Transform SelfTransform { get => transform; }

    private bool _controll = false;
    public bool HandControll() => _controll;
    public bool IsAlive => true;

    private float _startStickOffset;
    private float _targetStickOffset;

    private UnityEvent<bool> _moveEvent = new UnityEvent<bool>();

    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (_isActive == false)
            {
                _movementDirection = Vector3.zero;
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    _movementDirection = Vector3.zero;
                    OnMoveStarted?.Invoke();
                }
            }
        }
    }


    private Vector3 _startMousePosition;
    private Vector3 _newMousePosition;
    private Vector3 _movementDirection;

    private float _normalizedMagnitude;
    private bool _isActive = false;

    private void DeactivateControl()
    {
        IsActive = false;
    }

    private void Awake()
    {
        _transform = transform;
        _character = GetComponent<Character>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _lead = GetComponent<Leader>();
    }

    private void Start()
    {
        _startStickOffset = StickOffset;
        _targetStickOffset = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startMousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            OnMoveStarted?.Invoke();
            _controll = true;
        }

        if (Input.GetMouseButton(0))
        {
            _newMousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            _movementDirection = new Vector3(_newMousePosition.x - _startMousePosition.x, 0.0f,
                _newMousePosition.y - _startMousePosition.y);

            _normalizedMagnitude = _movementDirection.magnitude / 100f;
            _normalizedMagnitude = Mathf.Clamp01(_normalizedMagnitude);

            _movementDirection = _movementDirection.normalized * _normalizedMagnitude;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _controll = false;
            _movementDirection = Vector3.zero;
            _normalizedMagnitude = 0;
            OnFingerUpped?.Invoke();
            StickOffset = _startStickOffset;
        }


        if (IsActive == false) return;

        Move();
    }

    private void Move()
    {
        if (IsMove())
        {
            //Controller.Move(_movementDirection * MoveSpeed * Time.deltaTime);
            if (_updateRotation)
                _transform.rotation = Quaternion.Slerp(_transform.rotation, Quaternion.LookRotation(_movementDirection),
                    AngularSpeed * Time.fixedDeltaTime);

            _moveEvent?.Invoke(true);
        }
        else _moveEvent?.Invoke(false);
    }

    private void OnDisable()
    {
        _movementDirection = Vector3.zero;
        OnMoveStopped?.Invoke();
        _controll = false;
    }

    private void OnEnable()
    {
        IsActive = true;
    }

    public void AddMoveEvent(UnityAction<bool> action) =>
        _moveEvent?.AddListener(action);

    public void RemoveMoveEvent(UnityAction<bool> action) =>
        _moveEvent?.RemoveListener(action);

    public Vector3 MovementDirection { get => _movementDirection; }

    public bool IsMove() 
    {
        if (_movementDirection != Vector3.zero && _movementDirection.magnitude > StickOffset)
        {
            StickOffset = _targetStickOffset;
            return true;
        }
        else return false;
    }

    public void SetTargetPosition(Vector3 position)
    {
        _movementDirection = position;
    }
}