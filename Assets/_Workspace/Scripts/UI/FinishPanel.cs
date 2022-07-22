using UnityEngine;

public class FinishPanel : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private UnlockedSoldier _unlockedSoldier;

    [Header("Features")]
    [SerializeField] private RequirementSystemSO[] _arrayReqSustem;
    [SerializeField] private RequirementSystemSO _ReqSystem;

    private SaveManager _saveManager;
    private SaveData _data;

    private void Start()
    {
        _saveManager = new SaveManager();
        _data = _saveManager.Load();

        for (int i = 0; i < _arrayReqSustem.Length; i++)
        {
            if (_data.CurrentLevel >= _arrayReqSustem[i]._levelRange.x)
                _ReqSystem = _arrayReqSustem[i];
        }

        _unlockedSoldier.SoldierFace.sprite = _ReqSystem._unitFace;
        float count = _ReqSystem._levelRange.y - _ReqSystem._levelRange.x + 1;

        float remind = _ReqSystem._levelRange.y - _data.CurrentLevel;

        for (int i = 0, d = (int)(count - remind); i < count; i++)
        {
            GameObject point = Instantiate(_unlockedSoldier.Point, _unlockedSoldier.ProgressTransform);

            if (d != 0)
            {
                point.transform.GetChild(0).gameObject.SetActive(true);
                d--;
            }
        }

        if (_data.CurrentLevel != _ReqSystem._levelRange.y)
        {
            _unlockedSoldier.BackUnlocked.enabled = false;
            _unlockedSoldier.BackLocked.enabled = true;
        }
        else
        {
            _unlockedSoldier.BackUnlocked.enabled = true;
            _unlockedSoldier.BackLocked.enabled = false;
        }
    }
}
