using UnityEngine;
using UnityEngine.UI;

public class RevivalUI : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private GameObject _wallet;

    private void Awake()
    {
        GameObject = gameObject;
    }

    public void WalletActive(bool isActive) =>
        _wallet.SetActive(isActive);

    public Text Text { get => _text; }
    public GameObject GameObject { get; set; }
}
