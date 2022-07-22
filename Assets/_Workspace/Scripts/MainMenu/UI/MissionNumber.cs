using UnityEngine;
using UnityEngine.UI;

public class MissionNumber : MonoBehaviour
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

        _text.text = $"Mission {_data.CurrentLevel}";
    }
}