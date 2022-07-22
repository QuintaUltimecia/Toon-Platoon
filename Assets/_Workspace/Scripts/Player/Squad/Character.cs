using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ITakeDamage, ITransferMySelf, ITakeHeal
{
    [Header("Features")]
    [SerializeField] private float _health;
    [SerializeField] private bool _isDeath;
    [SerializeField] private bool _isNotCaptive;

    [Header("Features SO")]
    public CharacterClassSO ClassSO;
    public RankSO RankSO;
    public GunSO GunSO;

    [Header("Pool SO")]
    [SerializeField] private RankSO[] _ranksSO;
    [SerializeField] private GunSO[] _gunsSO;

    [Header("Links")]
    [SerializeField] private Gun _gun;
    [SerializeField] private AttackArea _attackArea;
    [SerializeField] private ParticleSystem _healParticle;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

    private int _maxHealth;

    //Cached
    private Transform _transform;
    private GameObject _gameObject;
    private EnemyPool _enemyPool;
    private Follower _follower;
    private VisibilityArea _visibilityArea;
    private PoolObjectContainer _poolObjectContainer;

    private UnityEvent _applyDamage = new UnityEvent();
    private UnityEvent _applyHeal = new UnityEvent();
    private UnityEvent _deathEvent = new UnityEvent();
    private UnityEvent<Character> _deathOnEnemy = new UnityEvent<Character>();
    private UnityEvent<bool> _notCaptiveEvent = new UnityEvent<bool>();
    public UnityEvent<int> OnInitGunEvent = new UnityEvent<int>();

    private SaveManager _saveManager;
    private SaveData _data;

    private void Awake()
    {
        _poolObjectContainer = transform.parent.Find(nameof(PoolObjectContainer)).GetComponent<PoolObjectContainer>();
        _transform = transform;
        _gameObject = gameObject;
        _gun?.InitCharacter(this);
        _follower = GetComponent<Follower>();
        _visibilityArea = GetComponent<VisibilityArea>();

        InitCharacter();
    }

    private void OnEnable()
    {
        AddDeathEvent(OnDeathEvent);
    }

    private void OnDisable()
    {
        RemoveDeathEvent(OnDeathEvent);
    }

    private void Start()
    {
        if (IsNotCaptive == false)
            _notCaptiveEvent?.Invoke(true);
    }

    private void SaveData()
    {
        switch (ClassSO.Class)
        {
            case CharacterClassSO.CurrentClass.Marine:
                _data.IsDeath_1 = _isDeath;
                break;
            case CharacterClassSO.CurrentClass.Sniper:
                _data.IsNotCaptive_2 = IsNotCaptive;
                _data.IsDeath_2 = _isDeath;
                break;
            case CharacterClassSO.CurrentClass.Heavy:
                _data.IsNotCaptive_3 = IsNotCaptive;
                _data.IsDeath_3 = _isDeath;
                break;
            case CharacterClassSO.CurrentClass.Medic:
                _data.IsNotCaptive_4 = IsNotCaptive;
                _data.IsDeath_4 = _isDeath;
                break;
            case CharacterClassSO.CurrentClass.Grenadier:
                _data.IsNotCaptive_5 = IsNotCaptive;
                _data.IsDeath_5 = _isDeath;
                break;
            default:
                break;
        }

        _saveManager.Save(_data);

        print("Save data");
    }

    public void InitCharacter()
    {
        _saveManager = new SaveManager();

        _data = _saveManager.Load();

        switch (ClassSO.Class)
        {
            case CharacterClassSO.CurrentClass.Marine:
                RankSO = _ranksSO[_data.RankNumber_1];
                _health = RankSO._MarineHealth;
                GunNumber = _data.GunNumber_1;
                GunSO = _gunsSO[_data.GunNumber_1];
                _gun.InitGun(_data.GunNumber_1);
                _isDeath = _data.IsDeath_1;
                _isNotCaptive = true;
                break;
            case CharacterClassSO.CurrentClass.Sniper:
                RankSO = _ranksSO[_data.RankNumber_2];
                _health = RankSO._SniperHealth;
                GunNumber = _data.GunNumber_2;
                GunSO = _gunsSO[_data.GunNumber_2];
                _gun.InitGun(_data.GunNumber_2);
                _isDeath = _data.IsDeath_2;
                _isNotCaptive = _data.IsNotCaptive_2;
                break;
            case CharacterClassSO.CurrentClass.Heavy:
                RankSO = _ranksSO[_data.RankNumber_3];
                _health = RankSO._HeavyHealth;
                GunNumber = _data.GunNumber_3;
                GunSO = _gunsSO[_data.GunNumber_3];
                _gun.InitGun(_data.GunNumber_3);
                _isDeath = _data.IsDeath_3;
                _isNotCaptive = _data.IsNotCaptive_3;
                break;
            case CharacterClassSO.CurrentClass.Medic:
                RankSO = _ranksSO[_data.RankNumber_4];
                _health = RankSO._MedicHealth;
                GunNumber = _data.GunNumber_4;
                GunSO = _gunsSO[_data.GunNumber_4];
                _gun.InitGun(_data.GunNumber_4);
                _isDeath = _data.IsDeath_4;
                _isNotCaptive = _data.IsNotCaptive_4;
                break;
            case CharacterClassSO.CurrentClass.Grenadier:
                RankSO = _ranksSO[_data.RankNumber_5];
                _health = RankSO._GrenadierHealth;
                GunNumber = _data.GunNumber_5;
                GunSO = _gunsSO[_data.GunNumber_5];
                _gun.InitGun(_data.GunNumber_5);
                _isDeath = _data.IsDeath_5;
                _isNotCaptive = _data.IsNotCaptive_5;
                break;
            default:
                break;
        }


        OnInitGunEvent?.Invoke(GunNumber);
        _maxHealth = (int)_health;
    }

    private void OnDeathEvent()
    {
        _data = _saveManager.Load();

        _enemyPool.OnDeathUnit(_attackArea.RemoveEnemy);
        _attackArea.gameObject.SetActive(false);
        _follower.enabled = false;
        _gun.enabled = false;
        _follower.Leader.UpdateLeader();
        _isDeath = true;
        FaceActive(false);

        SaveData();

        enabled = false;
    }

    public void InitEnemyPool(EnemyPool value) =>
        _enemyPool = value;

    public void GunActivate(bool isActive)
    {
        _gun.enabled = isActive;
    }

    public Component GetThis()
    {
        return this;
    }

    [ContextMenu(nameof(AddDamage))]
    private void AddDamage()
    {
        Health -= _maxHealth - 1;

        _applyDamage?.Invoke();
    }

    public void ApplyDamage(int damage)
    {
        _applyDamage?.Invoke();

        Health -= damage;

        if (_health == 0)
        {
            _deathEvent?.Invoke();
            _deathOnEnemy?.Invoke(this);
        }
    }

    public void ApplyHeal(float heal)
    {
        _applyHeal?.Invoke();

        if (_healParticle.isPlaying == false)
            _healParticle.Play();

        Health += heal;

        if (Health == MaxHealth)
            EndHeal();
    }

    public void EndHeal()
    {
        _healParticle.Stop();
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    public void SetCaptive(bool value)
    {
        _data = _saveManager.Load();

        _isNotCaptive = value;
        _notCaptiveEvent.Invoke(!value);
        _gun.VisualizeGun(GunNumber, value);

        SaveData();
    }

    public void FaceActive(bool isActive)
    {
        if(isActive is true)
            _skinnedMeshRenderer.materials[3].color = Color.white;
        else
            _skinnedMeshRenderer.materials[3].color = new Color(1, 1, 1, 0);

    }

    public void AddApplyDamageEvent(UnityAction action) =>
        _applyDamage?.AddListener(action);

    public void RemoveApplyDamageEvent(UnityAction action) =>
        _applyDamage?.RemoveListener(action);

    public void AddApplyHealEvent(UnityAction action) =>
        _applyHeal?.AddListener(action);

    public void RemoveApplyHealEvent(UnityAction action) =>
        _applyHeal?.RemoveListener(action);

    public void AddDeathEvent(UnityAction action) =>
        _deathEvent?.AddListener(action);

    public void RemoveDeathEvent(UnityAction action) =>
        _deathEvent?.RemoveListener(action);

    public void AddDeathOnEnemyEvent(UnityAction<Character> action) =>
        _deathOnEnemy?.AddListener(action);

    public void RemoveDeathOnEnemyEvent(UnityAction<Character> action) =>
        _deathOnEnemy?.RemoveListener(action);

    public void AddCaptiveEvent(UnityAction<bool> action) =>
        _notCaptiveEvent?.AddListener(action);

    public void RemoveCaptiveEvent(UnityAction<bool> action) =>
        _notCaptiveEvent?.RemoveListener(action);

    public float MoveSpeed { get => ClassSO._moveSpeed; }
    public Gun Gun { get => _gun; }
    public EnemyPool EnemyPool { get => _enemyPool; }
    public AttackArea AttackArea { get => _attackArea; }
    public Transform Transform { get => _transform; }
    public float Health { get => _health; private set => _health = Mathf.Min(Mathf.Max(0, value), MaxHealth); }
    public int MaxHealth { get => _maxHealth; }
    public VisibilityArea VisibilityArea { get => _visibilityArea; }
    public int GunNumber { get; set; }
    public PoolObjectContainer PoolObjectContainer { get => _poolObjectContainer; }
    public Follower Follower { get => _follower; }
    public GameObject GameObject { get => _gameObject; }
    public bool IsDeath { get => _isDeath; }
    public bool IsNotCaptive { get => _isNotCaptive; }
}