using UnityEngine;
using UnityEngine.Events;

public class Wallet : MonoBehaviour
{
    private int _coinAmount;
    private int _expAmount;

    [HideInInspector] public UnityEvent<int> CoinEvent = new UnityEvent<int>();
    [HideInInspector] public UnityEvent<int> ExpEvent = new UnityEvent<int>();

    private SaveManager _saveManager;
    private SaveData _data;

    private void Start()
    {
        _saveManager = new SaveManager();
        _data = _saveManager.Load();
        _coinAmount = _data.Coins;
        _expAmount = _data.Exp;

        CoinEvent?.Invoke(_coinAmount);
        ExpEvent?.Invoke(_expAmount);
    }

    public void CollectCoin(int value)
    {
        _data = _saveManager.Load();

        _coinAmount += value;
        CoinEvent?.Invoke(_coinAmount);

        _data.Coins = _coinAmount;
        _saveManager.Save(_data);
    }

    public void SpendingCoin(int value)
    {
        _data = _saveManager.Load();

        _coinAmount -= Mathf.Max(value, 0);
        CoinEvent?.Invoke(_coinAmount);

        _data.Coins = _coinAmount;
        _saveManager.Save(_data);
    }

    public void CollectExp(int value)
    {
        _data = _saveManager.Load();

        _expAmount += value;
        ExpEvent?.Invoke(_expAmount);

        _data.Exp = _expAmount;
        _saveManager.Save(_data);
    }

    public void SpendingExp(int value)
    {
        _data = _saveManager.Load();

        _expAmount -= value;
        ExpEvent?.Invoke(_expAmount);

        _data.Exp = _expAmount;
        _saveManager.Save(_data);
    }

    public int CoinAmount { get => _coinAmount; }
    public int ExpAmount { get => _expAmount; }
}
