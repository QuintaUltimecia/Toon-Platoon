using UnityEngine;
using UnityEngine.UI;

public class WalletUI : MonoBehaviour
{
    [SerializeField] private Wallet _wallet;

    [SerializeField] private Text _coinAmount;
    [SerializeField] private Text _expAmount;

    private void OnEnable()
    {
        _wallet?.CoinEvent.AddListener(UpdateCoinAmount);
        _wallet?.ExpEvent.AddListener(UpdateExpAmount);
    }

    private void OnDisable()
    {
        _wallet?.CoinEvent.RemoveListener(UpdateCoinAmount);
        _wallet?.ExpEvent.RemoveListener(UpdateExpAmount);
    }

    public void UpdateCoinAmount(int value)
    {
        _coinAmount.text = value.ToString();
    }

    public void UpdateExpAmount(int value)
    {
        _expAmount.text = value.ToString();
    }
}
