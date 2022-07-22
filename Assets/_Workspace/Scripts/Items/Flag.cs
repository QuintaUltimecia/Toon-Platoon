using UnityEngine;
using UnityEngine.Events;

public class Flag : MonoBehaviour
{
    [SerializeField] private GameObject _enemyFlag;
    [SerializeField] private bool _isChange = false;

    private Animator _animator;
    private SphereCollider _sphereCollider;

    private int CHANGE = Animator.StringToHash("isChange");

    private UnityEvent _changedEvent = new UnityEvent();

    private bool _isRotate = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        _enemyFlag.SetActive(true);
    }

    private void FlagChange()
    {
        _animator.SetTrigger(CHANGE);
        _sphereCollider.enabled = false;
    }

    public void FlagReplace()
    {
        _enemyFlag.SetActive(false);
    }

    public void FlagChangeEvent()
    {
        _isChange = true;
        _changedEvent?.Invoke();
    }

    public void AddChangedEvent(UnityAction action) =>
        _changedEvent?.AddListener(action);

    public void RemoveChangedEvent(UnityAction action) =>
        _changedEvent?.RemoveListener(action);

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character character))
        {
            FlagChange();
            if (_isRotate == false)
            {
                character.Follower.Leader.RotateAllCharacterToCamera();
                _isRotate = true;
            }
        }
    }

    public bool IsChange { get => _isChange; }
}
