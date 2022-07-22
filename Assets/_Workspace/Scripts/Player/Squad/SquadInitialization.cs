using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class SquadInitialization : MonoBehaviour
{
    [SerializeField] private Leader _leader;
    [SerializeField] private List<Character> _characters = new List<Character>();
    [SerializeField] private SquadPointsInGame[] _squadPoints;

    [HideInInspector] public UnityEvent AfterInitCharacter = new UnityEvent();

    private int _unitCount;

    private void Start()
    {
        InitUnits();
        SetPosition(_unitCount);
    }

    public void InitUnit(Character character)
    {
        _leader.InitFollower(character.Follower);
        _leader.InitLeader();
    }

    private void InitUnits()
    {
        _unitCount = 0;

        for (int i = 0; i < _characters.Count; i++)
        {
            if (_characters[i].IsDeath == false && _characters[i].IsNotCaptive == true)
            {
                _characters[i].gameObject.SetActive(true);
                _leader.InitFollower(_characters[i].Follower);

                _unitCount++;
            }
            else
            {
                _characters[i].gameObject.SetActive(false);
            }
        }

        _leader.InitLeader();
    }

    private void SetPosition(int count)
    {
        for (int i = 0, p = 0; i < _characters.Count; i++)
        {
            if (_characters[i].IsDeath == false && _characters[i].IsNotCaptive == true)
            {
                _characters[i].Follower.Transform.position = _squadPoints[count-1].Points[p].position;
                p++;
                _characters[i].Follower.enabled = true;
            }
        }

        AfterInitCharacter?.Invoke();
    }

    public List<Character> Characters { get => _characters; }
}
