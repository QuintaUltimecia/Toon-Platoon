using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    private SaveManager _saveManager;
    private SaveData _data;

    private void Awake()
    {
        CurrentScene = SceneManager.GetActiveScene().buildIndex;
    }

    private void Start()
    {
        _saveManager = new SaveManager();
    }

    public void RestartCurrentScene()
    {
        SceneManager.LoadScene(CurrentScene);
    }

    public void ReturnMainMenu()
    {
        _data = _saveManager.Load();
        _data.CurrentLevel++;
        _saveManager.Save(_data);

        SceneManager.LoadScene(0);
    }

    public int CurrentScene { get; private set; }
}
