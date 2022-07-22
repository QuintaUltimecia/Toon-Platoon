using UnityEngine;
using System.Collections.Generic;

public class SceneFeatures : MonoBehaviour
{
    [SerializeField] private Leader _leader;
    [SerializeField] private Transform _enemyesTransform;
    [SerializeField] private GameObject _finishPanel;
    [SerializeField] private GameObject _failedPanel;
    [SerializeField] private Flag _flag;

    [Header("Features")]
    [SerializeField] private int _collectedCoins = 1000;
    [SerializeField] private int _collectedExp = 1000;

    private List<Enemy> _enemyPool = new List<Enemy>();

    private SaveManager _saveManager;
    private SaveData _data;
    private UnitRequirement _unitRequirement;

    private void Awake()
    {
        for (int i = 0; i < _enemyesTransform.childCount; i++)
        {
            Transform child = _enemyesTransform.GetChild(i);

            if (child.TryGetComponent(out Enemy enemy))
            {
                enemy.AddRemoveEnemyEvent(OnDeathEvent);
                _enemyPool.Add(enemy);
            }
        }

        try
        {
            _unitRequirement = FindObjectOfType<UnitRequirement>();
        }
        catch
        {
            _unitRequirement = null;
        }

        _saveManager = new SaveManager();

        _data = _saveManager.Load();

        _data.CollectedCoins = _collectedCoins;
        _data.CollectedExp = _collectedExp;
        _data.KilledEnemy = _enemyPool.Count;

        _saveManager.Save(_data);
    }

    private void OnEnable()
    {
        _flag.AddChangedEvent(OnReplaceFlagEvent);
        _unitRequirement.AddRequirementEvent(OnReplaceFlagEvent);
        _leader.LoseEvent.AddListener(OnLoseEvent);
    }

    private void OnDisable()
    {
        _flag.RemoveChangedEvent(OnReplaceFlagEvent);
        _unitRequirement.RemoveRequirementEvent(OnReplaceFlagEvent);
        _leader.LoseEvent.RemoveListener(OnLoseEvent);
    }

    private void Start()
    {
        _finishPanel.SetActive(false);
        _failedPanel.SetActive(false);
    }

    private void OnDeathEvent(Enemy enemy)
    {
        _enemyPool.Remove(enemy);

        OnReplaceFlagEvent();
    }

    private void OnReplaceFlagEvent()
    {
        if (_enemyPool.Count == 0 && _flag.IsChange == true)
        {
            if (_unitRequirement?.IsRequirement == true)
            {
                _finishPanel.SetActive(true);
                EndLevel();
            }
        }
    }

    private void OnLoseEvent()
    {
        _failedPanel.SetActive(true);
        EndLevel();
    }

    private void EndLevel()
    {
        _data = _saveManager.Load();

        _data.Coins += _collectedCoins;
        _data.Exp += _collectedExp;
        _data.CollectedCoins = _collectedCoins;
        _data.CollectedExp = _collectedExp;

        _saveManager.Save(_data);
    }
}
