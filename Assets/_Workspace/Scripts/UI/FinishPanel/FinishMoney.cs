using UnityEngine;
using UnityEngine.UI;

public class FinishMoney : MonoBehaviour
{
    private Text _text;

    private SaveManager _saveManager;
    private SaveData _data;

    private void Awake()
    {
        _text = GetComponent<Text>();
    }

    private void Start()
    {
        _saveManager = new SaveManager();
        _data = _saveManager.Load();

        _text.text = _data.CollectedCoins.ToString();
    }
}
