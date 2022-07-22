using UnityEngine;
using UnityEngine.UI;

public class KilledEnemy : MonoBehaviour
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

        _text.text = $"x{_data.KilledEnemy}";
    }
}
