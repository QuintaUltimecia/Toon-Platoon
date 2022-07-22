using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int CurrentLevel
    {
        get
        {
            if (IsTestLaunch)
                return _currentTestLevel;
            else if (PlayerPrefs.HasKey(SAVE_LEVEL_KEY))
                return PlayerPrefs.GetInt(SAVE_LEVEL_KEY);
            else
                return 1;
        }
    }
    public int CurrentPoint
    {
        get
        {
            if (IsTestLaunch)
                return _currentTestPoint;
            else if (PlayerPrefs.HasKey(SAVE_POINT_KEY))
                return PlayerPrefs.GetInt(SAVE_POINT_KEY);
            else
                return 0;
        }
    }

    private bool IsTestLaunch => _loadSpecificLevel > 0;

    private const string SAVE_LEVEL_KEY = "CurrentLevel";
    private const string SAVE_POINT_KEY = "CurrentPoint";

    [Tooltip("Ноль - обычный запуск. Меньше нуля - будет загружен первый уровень и сделан сброс прогресса. Больше нуля - загрузка указанного уровня без сброса.")]
    [SerializeField] private int _loadSpecificLevel = 0;
    [Tooltip("Сцены под индексами от 1 до заданного числа (включая) не будут рандомиться. Ноль - без ограничений.")]
    [SerializeField] private int _nonRandomizableRange = 3;

    private int _currentTestLevel = 0;
    private int _currentTestPoint = 0;

    [ContextMenu("Reset all progress")]
    private void ResetAll()
    {
        PlayerPrefs.SetInt(SAVE_LEVEL_KEY, 1);
        PlayerPrefs.SetInt(SAVE_POINT_KEY, 0);
        print("All progres has been reset");
    }

    [ContextMenu("Show progress")]
    private void ShowProgress()
    {
        var level =  PlayerPrefs.GetInt(SAVE_LEVEL_KEY);
        var checkpoint = PlayerPrefs.GetInt(SAVE_POINT_KEY);
        print("Current level - " + level + " Current checkPoint - " + checkpoint);
    }

    public void LoadLevel()
    {
        if (_loadSpecificLevel >= 0)
        {
            SceneManager.LoadScene(SelectLevel(CurrentLevel));
        }
        else
        {
            PlayerPrefs.SetInt(SAVE_LEVEL_KEY, 1);
            PlayerPrefs.SetInt(SAVE_POINT_KEY, 0);
            SceneManager.LoadScene(SelectLevel(CurrentLevel));
        }
    }

    public void SaveProgress()
    {
        if (!IsTestLaunch)
            PlayerPrefs.SetInt(SAVE_LEVEL_KEY, CurrentLevel + 1);
        else
            _currentTestLevel++;
    }

    public int LoadPoint()
    {
        if (_loadSpecificLevel < 0)
        {
            PlayerPrefs.SetInt(SAVE_POINT_KEY, 0);
        }

        return CurrentPoint;
    }
    public void SavePoint(int point)
    {
        if (!IsTestLaunch)
            PlayerPrefs.SetInt(SAVE_POINT_KEY, point);
    }


    private void OnValidate()
    {
        _nonRandomizableRange = Mathf.Clamp(_nonRandomizableRange, 0, SceneManager.sceneCountInBuildSettings - 2);
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (IsTestLaunch)
            _currentTestLevel = _loadSpecificLevel;

        LoadLevel();
    }

    private int SelectLevel(int levelNumber)
    {
        int gameScenesCount = SceneManager.sceneCountInBuildSettings - 1; //Минус один, что бы исключить левел менеджер

        if (levelNumber > gameScenesCount)
        {
            Random.InitState(levelNumber - gameScenesCount); //Что бы у повторяющихся уровней был другой рандом
            int randomLevel = Random.Range(_nonRandomizableRange + 1, gameScenesCount + 1);
            return randomLevel;
        }
        else
        {
            return levelNumber;
        }
    }
}
