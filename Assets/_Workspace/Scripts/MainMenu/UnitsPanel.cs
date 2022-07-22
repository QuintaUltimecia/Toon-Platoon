using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnitsPanel : MonoBehaviour
{
    [SerializeField] private CharacterMenu _currentDefault;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private GameObject _weaponArrow;
    [SerializeField] private GameObject _rankArrow;
    [SerializeField] private Text _needRank;

    [Header("Prefab")]
    [SerializeField] private Exp _expPrefab;
    [SerializeField] private RectTransform _expPooling;

    private RectTransform _rankTransform;

    private CharacterMenu _currentCharacterMenu;
    private UpgradesMenu _upgradesMenu;

    private Tween _needRankColorTween;

    private PoolObjects<Exp> _expObjPool;

    [SerializeField] private int _expCharacter;

    private bool _isBuyRank;

    private void Awake()
    {
        _upgradesMenu = transform.Find("Upgrades").GetComponent<UpgradesMenu>();

        _currentCharacterMenu = _currentDefault;

        _rankTransform = _rankArrow.transform.parent.GetComponent<RectTransform>();
    }

    private void Start()
    {
        _expObjPool = new PoolObjects<Exp>(_expPrefab, 150, _expPooling);
        _expObjPool.AutoExpand = true;

        _needRank.color = new Color(1f, 0.3f, 0.3f, 0);

        if (_currentCharacterMenu.IsDeath != true)
            _currentCharacterMenu.OnCharacterFeatures();
    }

    private void SetArrows()
    {
        if (_currentCharacterMenu.GetNextGun()?._price <= _wallet.CoinAmount && 
            _currentCharacterMenu.RankNumber >= (int)_currentCharacterMenu.GetNextGun()?._targetRank)
            _weaponArrow.SetActive(true);
        else _weaponArrow.SetActive(false);

        if (_currentCharacterMenu.GetNextRank() != null)
        {
            int needExp = _currentCharacterMenu.GetNextRank()._requiredExp - _currentCharacterMenu.Exp;

            if (needExp <= _wallet.ExpAmount)
                _rankArrow.SetActive(true);
            else _rankArrow.SetActive(false);
        }
        else _rankArrow.SetActive(false);
    }

    private void NeedingRank(string value)
    {
        if (_needRankColorTween != null)
            _needRankColorTween.Kill();

        _needRank.color = new Color(1f, 0.3f, 0.3f, 1);
        _needRankColorTween = _needRank.DOColor(new Color(1f, 0.3f, 0.3f, 0), 2f).SetEase(Ease.Linear);
        _needRank.text = $"Need rank: {value}";
    }

    private void CompleteRank()
    {
        if (_isBuyRank == true)
            _currentCharacterMenu._expEvent?.Invoke(_expCharacter += 1);
    }

    public void UpdateExpCharacter()
    {
        _expCharacter = _currentCharacterMenu.Exp;
        _currentCharacterMenu._expEvent?.Invoke(_expCharacter);
    }

    public void OnBuyWeapon()
    {
        if (_currentCharacterMenu.GetNextGun() != null)
        {
            if (_currentCharacterMenu.RankNumber < (int)_currentCharacterMenu.GetNextGun()._targetRank)
                NeedingRank(_currentCharacterMenu.GetNextGun()?._targetRank.ToString());

            print(_currentCharacterMenu.GetNextGun()?._price + " <= " + _wallet.CoinAmount);
            print(_currentCharacterMenu.RankNumber + " >= " + (int)_currentCharacterMenu.GetNextGun()._targetRank);

            if (_currentCharacterMenu.GetNextGun()?._price <= _wallet.CoinAmount &&
                _currentCharacterMenu.RankNumber >= (int)_currentCharacterMenu.GetNextGun()._targetRank)
            {
                _wallet.SpendingCoin(_currentCharacterMenu.GetNextGun()._price);

                _currentCharacterMenu.BuyWeapon();
            }

            SetArrows();
        }
    }

    public void OnBuyRank()
    {
        if (_currentCharacterMenu.GetNextRank() != null)
        {
            if (_wallet.ExpAmount != 0)
            {
                _wallet.SpendingExp(1);
                _currentCharacterMenu.CollectExp(1);
                CreateExp(_currentCharacterMenu.ExpUI.RectTransform, CompleteRank);
                _isBuyRank = true;
            }

            if (_currentCharacterMenu.GetNextRank()._requiredExp <= _currentCharacterMenu.Exp)
            {
                _currentCharacterMenu.BuyRank();
                UpdateExpCharacter();

                for (int i = 0; i < _expObjPool.Pool.Count; i++)
                    if (_expObjPool.Pool[i].isActiveAndEnabled) _expObjPool.Pool[i].CompleteOnRank();
            }

            SetArrows();
        }
    }

    private void CreateExp(RectTransform currentExpBar, TweenCallback tween)
    {
        Exp exp = _expObjPool.GetFreeElement(_expPooling.position);
        exp.TargetTransform = currentExpBar;
        exp.AddTween(tween);
        exp.Move();
    }

    public void InitCharacter(CharacterMenu value, GunSO gun, RankSO rank)
    {
        _currentCharacterMenu = value;

        string priceWeapon;
        string priceRank;

        if (_currentCharacterMenu.GunNumber == _currentCharacterMenu.Guns.Length - 1)
            priceWeapon = "MAX. LV.";
        else priceWeapon = gun._price.ToString();

        if (_currentCharacterMenu.RankNumber == _currentCharacterMenu.Runks.Length - 1)
            priceRank = "MAX. LV.";
        else priceRank = rank._requiredExp.ToString();

        _upgradesMenu.GetWeapon(
            sprite: gun._sprite,
            title: gun._name,
            price: priceWeapon);

        _upgradesMenu.GetRank(
            sprite: rank._sprite,
            title: rank._name,
            price: priceRank);

        SetArrows();
        UpdateExpCharacter();
        _isBuyRank = false;
    }

    public CharacterMenu CurrentCharacter { get => _currentCharacterMenu; }
    public RectTransform RankTransform { get => _rankTransform; }
    public Wallet Wallet { get => _wallet; }
}
