using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, ITakeDamage, ITransferMySelf
{
    private int _health;
    private int _damage;
    [SerializeField] private EnemyFeaturesSO[] _arrayFeatures;
    [SerializeField] private EnemyFeaturesSO _features;
    public Entity EntityValue;

    private int _maxHealth;

    private bool _isDeath = false;

    private Transform _transform;
    private CharacterController _characterController;

    private UnityEvent _deathEvent = new UnityEvent();
    private UnityEvent<Enemy> _removeEnemy = new UnityEvent<Enemy>();
    private UnityEvent _applyDamage = new UnityEvent();
    private int _triggerCount;

    private EnemyAI _enemyAI;
    private VisibilityArea _visibilityArea;

    private SaveManager _saveManager;
    private SaveData _data;

    public enum Entity
    {
        Unit,
        Barrel,
        Turret
    }

    private void Awake()
    {
        _transform = transform;
        _characterController = GetComponent<CharacterController>();
        _maxHealth = _health;

        _enemyAI = GetComponent<EnemyAI>();
        _visibilityArea = GetComponent<VisibilityArea>();

        _saveManager = new SaveManager();

        GetFeatures();
    }

    private void OnEnable()
    {
        if (_features != null)
            AddDeathEvent(DeathEvent);
    }

    private void OnDisable()
    {
        _removeEnemy.RemoveAllListeners();
        _deathEvent.RemoveAllListeners();
    }

    private void GetFeatures()
    {
        _data = _saveManager.Load();

        for (int i = 0; i < _arrayFeatures.Length; i++)
        {
            if (_data.CurrentLevel >=_arrayFeatures[i].LevelRange.x && _data.CurrentLevel <= _arrayFeatures[i].LevelRange.y)
                _features = _arrayFeatures[i];
        }
    }

    public void ApplyDamage(int damage)
    {
        TakedDamage = damage;

        Health -= damage;
        _applyDamage?.Invoke();

        if (Health == 0)
        {
            _isDeath = true;
            _removeEnemy?.Invoke(this);
            _deathEvent?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Bullet bullet)
            && _enemyAI?.Character == null
            && _isDeath == false)
        {
            if (_enemyAI == null)
                return;

            if (_enemyAI.Character?.Health != 0)
                _enemyAI.Gun.SetTarget(_enemyAI.Character);
        }
    }

    private void DeathEvent()
    {
        if (_enemyAI != null)
        {
            _enemyAI.Gun.enabled = false;
            _characterController.enabled = false;
        }

        enabled = false;
    }

    public void AddApplyDamageEvent(UnityAction action) =>
        _applyDamage?.AddListener(action);

    public void RemoveApplyDamageEvent(UnityAction action) =>
        _applyDamage?.RemoveListener(action);

    public void AddDeathEvent(UnityAction action) =>
        _deathEvent?.AddListener(action);

    public void RemoveDeathEvent(UnityAction action) =>
        _deathEvent?.RemoveListener(action);

    public void AddRemoveEnemyEvent(UnityAction<Enemy> action)
    {
        _removeEnemy?.AddListener(action);

        if(_triggerCount < 6)
            _triggerCount++;
    }

    public void RemoveEnemyEvent(UnityAction<Enemy> action)
    {
        try
        {
            _removeEnemy?.RemoveListener(action);
        }
        catch
        {
            print("net iventa");
        }

        if (_triggerCount > 0)
            _triggerCount--;

        if (_triggerCount == 0)
            EnemyPool.RemoveEnemy(this);

    }

    public Component GetThis()
    {
        return this;
    }

    public EnemyFeaturesSO Features { get => _features; }
    public Transform Transform { get => _transform; }
    public int Health { get => _health; private set => _health = Mathf.Max(value, 0); }
    public int Damage { get => _damage; }
    public int MaxHealth
    {
        get => _maxHealth;
    }
    public CharacterController CharacterController { get => _characterController; }
    public EnemyAI EnemyAI { get => _enemyAI; }
    public int TakedDamage { get; private set; }
    public VisibilityArea VisibilityArea { get => _visibilityArea; }
    public EnemyPool EnemyPool { get; set; }
}
