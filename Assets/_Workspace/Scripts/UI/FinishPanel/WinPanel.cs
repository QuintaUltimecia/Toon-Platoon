using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    private Text _text;

    private SaveManager _saveManager;
    private SaveData _data;

    private void Awake()
    {
        _text = GetComponent<Text>();

        _saveManager = new SaveManager();
        _data = _saveManager.Load();

        _text.text = $"MISSION {_data.CurrentLevel} COMPLETED!";
    }
}
