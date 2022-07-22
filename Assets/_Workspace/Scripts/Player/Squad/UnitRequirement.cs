using UnityEngine;
using UnityEngine.Events;

public class UnitRequirement : MonoBehaviour
{
    [Header("Features")]
    private Character _current;
    [SerializeField] private CharacterClassSO.CurrentClass _unit;

    [Header("Links")]
    [SerializeField] private SquadInitialization _initSquad;
    [SerializeField] private ParticleSystem _particle;

    private Transform _transform;
    private SphereCollider _collider;
    private bool _isRequirement = false;

    private UnityEvent _requirementEvent = new UnityEvent();

    private void Awake()
    {
        _transform = transform;
        _collider = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        _initSquad.AfterInitCharacter?.AddListener(InitFollower);
    }

    private void OnDisable()
    {
        _initSquad.AfterInitCharacter?.RemoveListener(InitFollower);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character character) && _isRequirement == false)
        {
            if (_current != null) Release();
        }
    }

    private void Release()
    {
        _current.SetCaptive(true);
        _initSquad.InitUnit(_current);
        _current.Follower.enabled = true;
        _current.AttackArea.gameObject.SetActive(true);

        _isRequirement = true;
        _particle.Play();
        _collider.enabled = false;
        _requirementEvent?.Invoke();
    }

    public void InitFollower()
    {
        for (int i = 0; i < _initSquad.Characters.Count; i++)
        {
            if (_initSquad.Characters[i].ClassSO.Class == _unit && _initSquad.Characters[i].IsNotCaptive == false)
            {
                _current = _initSquad.Characters[i];
                _current.Follower.enabled = false;
                _current.AttackArea.gameObject.SetActive(false);
                _current.GameObject.SetActive(true);
                _current.Transform.position = new Vector3(_transform.position.x, _current.Transform.position.y, _transform.position.z);
                _current.Transform.rotation = new Quaternion(0, 0, 0, 0);
                _current.SetCaptive(false);
                break;
            }
        }
    }

    public void AddRequirementEvent(UnityAction action) =>
        _requirementEvent?.AddListener(action);

    public void RemoveRequirementEvent(UnityAction action) =>
        _requirementEvent?.RemoveListener(action);

    public bool IsRequirement { get => _isRequirement; }
}
