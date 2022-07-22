using UnityEngine;
using System.Collections.Generic;

public class CharacterInitialization : MonoBehaviour
{
    [SerializeField] private List<CharacterMenu> _characters = new List<CharacterMenu>();
    [SerializeField] private SquadPoint[] _squadPoints;

    [Header("Links")]
    [SerializeField] private GameObject _startButtonDisabled;
    [SerializeField] private GameObject _upgradesDisabled;

    private int _unitCount;

    private CharacterMenu _marine;

    private void Awake()
    {
        InitUnits();
        SetPosition(_unitCount);
    }

    private void Start()
    {
        OnAllDeathEvent(true);
    }

    public void InitUnits()
    {
        _unitCount = 0;

        for (int i = 0; i < _characters.Count; i++)
        {
            if (_characters[i].IsNotCaptive == true)
            {
                _characters[i].gameObject.SetActive(true);

                _unitCount++;
            }
            else
            {
                _characters[i].gameObject.SetActive(false);
            }

            _characters[i]._deathEvent.AddListener(OnAllDeathEvent);
        }
    }

    private void SetPosition(int count)
    {
        for (int i = 0, p = 0; i < _characters.Count; i++)
        {
            if (_characters[i].IsNotCaptive == true)
            {
                _characters[i].RectTransform.position = _squadPoints[count-1].Points[p].position;
                p++;
            }
        }
    }

    public void OnAllDeathEvent(bool isActive)
    {
        for (int i = 0, d = 0; i < _characters.Count; i++)
        {
            if (_characters[i].Class.Class == CharacterClassSO.CurrentClass.Marine)
                _marine = _characters[i];

            if (_characters[i].IsDeath == true) d++;

            if (d == _unitCount)
            {
                _startButtonDisabled.SetActive(true);
                _upgradesDisabled.SetActive(true);
                isActive = true;
            }
            else
            {
                _startButtonDisabled.SetActive(false);
                _upgradesDisabled.SetActive(false);
                isActive = false;
            }
        }

        if (isActive == true && _marine.UnitsPanel.Wallet.CoinAmount < _marine.CurrentRank._priceOfRevival)
        {
            _marine.AllDeath = true;
            _marine.RevivalUI.Text.text = "Free";
            _marine.RevivalUI.WalletActive(false);
        }
    }

    public List<CharacterMenu> Characters { get => _characters; }
}
