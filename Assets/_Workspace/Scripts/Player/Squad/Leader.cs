using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;

public class Leader : MonoBehaviour
{
    private List<Follower> _followers = new List<Follower>();

    private Follower _leader;

    private PlayerContoller _playerController;
    private Transform _transform;
    private float _largestSpeed;
    private EnemyPool _enemyPool;

    [HideInInspector] public UnityEvent LoseEvent = new UnityEvent();

    private void Awake()
    {
        _transform = transform;
        _playerController = GetComponent<PlayerContoller>();
    }

    private void Start()
    {
        _playerController.MoveSpeed = 4f;
    }

    private void Update()
    {
        if (_leader != null)
        {
            _transform.position = _leader.Transform.position;
            _transform.rotation = _leader.Transform.rotation;
        }

        SetFollowPosition(_playerController.IsMove());
    }

    private void SetFollowPosition(bool isMove)
    {
        for (int i = 0; i < _followers.Count; i++)
            _followers[i].MoveControl(isMove);
    }

    public void UpdateLeader()
    {
        _largestSpeed = 0;

        for (int i = 0; i < _followers.Count; i++)
        {
            if (_largestSpeed < _followers[i].Character.MoveSpeed)
                _largestSpeed = _followers[i].Character.MoveSpeed;
        }

        for (int i = 0; i < _followers.Count; i++)
        {
            if (_largestSpeed == _followers[i].Character.MoveSpeed)
                _leader = _followers[i];
        }
    }

    public void InitFollower(Follower follower)
    {
        _followers.Add(follower);
    }

    public void RemoveFollower(Follower follower)
    {
        _followers.Remove(follower);

        if (_followers.Count == 0)
            LoseEvent?.Invoke();

    }

    public void RotateAllCharacterToCamera()
    {
        _leader.Transform.DORotate(new Vector3(0, 180, 0), 1f).SetEase(Ease.Linear);
    }

    public void InitLeader()
    {
        _enemyPool = GetComponent<EnemyPool>();

        for (int i = 0; i < _followers.Count; i++)
        {
            _followers[i].InitLeader(this);
            _followers[i].Character.InitEnemyPool(_enemyPool);
        }

        UpdateLeader();
    }

    public bool IsMove { get => _playerController.IsMove(); }
    public Transform Transform { get => _transform; }
    public Follower FollowerLeader { get => _leader; }
    public List<Follower> Followers { get => _followers; }
    public int FollowerCount { get => _followers.Count; }
}
