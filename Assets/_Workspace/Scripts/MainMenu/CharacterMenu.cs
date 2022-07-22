using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Image _image;
    [SerializeField] private ExpUI _expUI;
    [SerializeField] private FeaturesUI _featuresUI;
    [SerializeField] private RevivalUI _revivalUI;
    [SerializeField] private UnitsPanel _unitsPanel;
    [SerializeField] private PlatformUnit _platformUnit;
    [SerializeField] private GunSO[] _guns;
    [SerializeField] private RankSO[] _ranks;

    [Header("Features")]
    [SerializeField] private int _exp;
    [SerializeField] private GunSO _currentGun;
    [SerializeField] private RankSO _currentRank;
    [SerializeField] private CharacterClassSO _currentClass;

    private int _targetExp;

    private int _gunNumber = 0;
    private int _rankNumber = 0;
    [SerializeField] private bool _isDeath;
    private bool _isNotCaptive;

    private RectTransform _rectTransform;
    private CustomButton _customButton;

    [HideInInspector] public UnityEvent<int> _expEvent = new UnityEvent<int>();
    [HideInInspector] public UnityEvent _selectEvent = new UnityEvent();
    [HideInInspector] public UnityEvent<int> _selectIdleEvent = new UnityEvent<int>();
    [HideInInspector] public UnityEvent<bool> _deathEvent = new UnityEvent<bool>();

    private SaveManager _saveManager;
    private SaveData _data;

    private void Awake()
    {
        _saveManager = new SaveManager();
        LoadData();
        SetCurrentGun(_gunNumber);
        SetCurrentRank(_rankNumber);
        InitTargetExp();

        _rectTransform = GetComponent<RectTransform>();
        _platformUnit.UnitAnimationController.InitCharacterMenu(this);
        _customButton = GetComponent<CustomButton>();
    }

    private void Start()
    {
        _expEvent.Invoke(_exp);
        Death();
    }

    private void SaveData()
    {
        switch (_currentClass.Class)
        {
            case CharacterClassSO.CurrentClass.Marine:
                _data.GunNumber_1 = _gunNumber;
                _data.RankNumber_1 = _rankNumber;
                _data.ExpUnit_1 = _exp;
                _data.IsDeath_1 = _isDeath;
                break;
            case CharacterClassSO.CurrentClass.Sniper:
                _data.GunNumber_2 = _gunNumber;
                _data.RankNumber_2 = _rankNumber;
                _data.ExpUnit_2 = _exp;
                _data.IsDeath_2 = _isDeath;
                break;
            case CharacterClassSO.CurrentClass.Heavy:
                _data.GunNumber_3 = _gunNumber;
                _data.RankNumber_3 = _rankNumber;
                _data.ExpUnit_3 = _exp;
                _data.IsDeath_3 = _isDeath;
                break;
            case CharacterClassSO.CurrentClass.Medic:
                _data.GunNumber_4 = _gunNumber;
                _data.RankNumber_4 = _rankNumber;
                _data.ExpUnit_4 = _exp;
                _data.IsDeath_4 = _isDeath;
                break;
            case CharacterClassSO.CurrentClass.Grenadier:
                _data.GunNumber_5 = _gunNumber;
                _data.RankNumber_5 = _rankNumber;
                _data.ExpUnit_5 = _exp;
                _data.IsDeath_5 = _isDeath;
                break;
        }

        _saveManager.Save(_data);
        print("Save data");
    }

    private void LoadData()
    {
        _data = _saveManager.Load();

        switch (_currentClass.Class)
        {
            case CharacterClassSO.CurrentClass.Marine:
                _isDeath = _data.IsDeath_1;
                _gunNumber = _data.GunNumber_1;
                _rankNumber = _data.RankNumber_1;
                CollectExp(_data.ExpUnit_1);
                _isNotCaptive = true;
                break;
            case CharacterClassSO.CurrentClass.Sniper:
                _isDeath = _data.IsDeath_2;
                _gunNumber = _data.GunNumber_2;
                _rankNumber = _data.RankNumber_2;
                CollectExp(_data.ExpUnit_2);
                _isNotCaptive = _data.IsNotCaptive_2;
                break;
            case CharacterClassSO.CurrentClass.Heavy:
                _isDeath = _data.IsDeath_3;
                _gunNumber = _data.GunNumber_3;
                _rankNumber = _data.RankNumber_3;
                CollectExp(_data.ExpUnit_3);
                _isNotCaptive = _data.IsNotCaptive_3;
                break;
            case CharacterClassSO.CurrentClass.Medic:
                _isDeath = _data.IsDeath_4;
                _gunNumber = _data.GunNumber_4;
                _rankNumber = _data.RankNumber_4;
                CollectExp(_data.ExpUnit_4);
                _isNotCaptive = _data.IsNotCaptive_4;
                break;
            case CharacterClassSO.CurrentClass.Grenadier:
                _isDeath = _data.IsDeath_5;
                _gunNumber = _data.GunNumber_5;
                _rankNumber = _data.RankNumber_5;
                CollectExp(_data.ExpUnit_5);
                _isNotCaptive = _data.IsNotCaptive_5;
                break;
        }

        print("Load data");
    }

    private void InitTargetExp()
    {
        if (GetNextRank() != null)
            _targetExp = GetNextRank()._requiredExp;
        else _targetExp = 0;
    }

    public void OnCharacterFeatures()
    {
        _unitsPanel.InitCharacter(this, _currentGun, _currentRank);

        int health = 0;

        switch (_currentClass.Class)
        {
            case CharacterClassSO.CurrentClass.Marine:
                health = _currentRank._MarineHealth;
                break;
            case CharacterClassSO.CurrentClass.Sniper:
                health = _currentRank._SniperHealth;
                break;
            case CharacterClassSO.CurrentClass.Heavy:
                health = _currentRank._HeavyHealth;
                break;
            case CharacterClassSO.CurrentClass.Medic:
                health = _currentRank._MedicHealth;
                break;
            case CharacterClassSO.CurrentClass.Grenadier:
                health = _currentRank._GrenadierHealth;
                break;
        }

        _featuresUI.GetFeatures(health: health, damage: _currentGun._damage);

        Vector3 defaultPosition = ExpUI.RectTransform.localPosition;
        ExpUI.RectTransform.SetParent(_unitsPanel.RankTransform);
        ExpUI.RectTransform.localPosition = defaultPosition;

        _selectEvent?.Invoke();
        _selectIdleEvent?.Invoke(_gunNumber + 1);
        _platformUnit.SetSelection(true);
    }

    public void RemoveExpUIParent()
    {
        Vector3 defaultPosition = _expUI.RectTransform.localPosition;
        _expUI.RectTransform.SetParent(_rectTransform);
        _expUI.RectTransform.localPosition = defaultPosition;

        if (_unitsPanel.CurrentCharacter == this)
        {
            defaultPosition = _expUI.RectTransform.localPosition;
            _expUI.RectTransform.SetParent(_unitsPanel.RankTransform);
            _expUI.RectTransform.localPosition = defaultPosition;
        }
        else
        {
            _featuresUI.ResetFeatures();
            _platformUnit.SetSelection(false);
        }
    }

    public GunSO GetNextGun()
    {
        int value = _gunNumber + 1;

        if (value >= _guns.Length) return null;
        else return _guns[value];
    }

    public RankSO GetNextRank()
    {
        int value = _rankNumber + 1;

        if (value >= _ranks.Length) return null;
        else return _ranks[value];
    }

    public void SetCurrentRank(int value)
    {
        _currentRank = _ranks[value];
        _expUI.UpdateSprite(_currentRank._sprite);
    }

    public void SetCurrentGun(int value)
    {
        _platformUnit.ReplaceWeaponModel.ActiveWeapon(value);
        _currentGun = _guns[value];
    }

    public void CollectExp(int value)
    {
        _exp += value;

        SaveData();
    }

    public void ResetExp()
    {
        _exp = 0;

        _expEvent?.Invoke(_exp);

        SaveData();
    }

    public void BuyRank()
    {
        RankNumber += 1;

        SetCurrentRank(RankNumber);
        OnCharacterFeatures();
        ResetExp();
        InitTargetExp();

        SaveData();
    }

    public void BuyWeapon()
    {
        GunNumber += 1;

        SetCurrentGun(GunNumber);
        OnCharacterFeatures();

        SaveData();
    }

    public void Revival()
    {
        _data = _saveManager.Load();

        if (_unitsPanel.Wallet.CoinAmount >= _currentRank._priceOfRevival)
        {
            _unitsPanel.Wallet.SpendingCoin(_currentRank._priceOfRevival);
            _isDeath = false;
            Death();

            _customButton.GetActions();
            SaveData();
        }

        if (AllDeath == true)
        {
            _isDeath = false;
            Death();

            _customButton.GetActions();
            SaveData();
        }
    }

    public void Death()
    {
        if (_isDeath == true)
        {
            _image.color = Color.gray;
        }
        else _image.color = Color.white;

        _deathEvent?.Invoke(_isDeath);
        _customButton.enabled = !_isDeath;
        _revivalUI.GameObject.SetActive(_isDeath);
        _revivalUI.Text.text = _currentRank._priceOfRevival.ToString();
        _revivalUI.WalletActive(true);
        _expUI.GameObject.SetActive(!_isDeath);
        _featuresUI.GameObject.SetActive(!_isDeath);
    }

    public PlatformUnit PlatformUnit { get => _platformUnit; }
    public GunSO[] Guns { get => _guns; }
    public int GunNumber { get => _gunNumber; set => _gunNumber = Mathf.Min(value, 2); }
    public RankSO[] Runks { get => _ranks; }
    public int RankNumber { get => _rankNumber; set => _rankNumber = Mathf.Min(value, 5); }
    public int Exp { get => _exp; }
    public int TargetExp { get => _targetExp; }
    public ExpUI ExpUI { get => _expUI; }
    public RectTransform RectTransform { get => _rectTransform; set => _rectTransform = value; }
    public bool IsDeath { get => _isDeath; }
    public bool IsNotCaptive { get => _isNotCaptive; }
    public CharacterClassSO Class { get => _currentClass; }
    public bool AllDeath { get; set; }
    public RevivalUI RevivalUI { get => _revivalUI; }
    public UnitsPanel UnitsPanel { get => _unitsPanel; }
    public RankSO CurrentRank { get => _currentRank; }
}
