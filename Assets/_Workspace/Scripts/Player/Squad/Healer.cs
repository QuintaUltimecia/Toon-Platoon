using UnityEngine;
using System.Collections;

public class Healer : MonoBehaviour
{
    [Header("Features")]
    [SerializeField] private float _healPercent = 0.05f;
    [SerializeField] private float _healInterval = 0.05f;

    private Leader _leader;
    private Follower _follower;
    private Character _character;

    private Coroutine _healRoutine;

    private void Awake()
    {
        _character = GetComponent<Character>();
        _follower = GetComponent<Follower>();
    }

    private void OnEnable()
    {
        _character.AddDeathEvent(DeathEvent);
        _follower.GetLeaderEvent.AddListener(InitLeader);
        _follower.OnActiveEvent.AddListener(Activity);
    }

    private void OnDisable()
    {
        _character.RemoveDeathEvent(DeathEvent);
        _follower.GetLeaderEvent.RemoveListener(InitLeader);
        _follower.OnActiveEvent.RemoveListener(Activity);
    }

    private void InitLeader(Leader lead) =>
        _leader = lead;

    private void Update()
    {
        if (_leader?.IsMove == false && _leader.FollowerLeader.Character.EnemyPool.IsEmptyEnemy == true)
        {
            if (_healRoutine == null)
                _healRoutine = StartCoroutine(Healing());
        }
        else
        {
            if (_healRoutine != null)
            {
                StopCoroutine(_healRoutine);
                EndHealing();
                _healRoutine = null;
            }
        }
    }

    private void DeathEvent()
    {
        if (_healRoutine != null)
            StopCoroutine(_healRoutine);

        EndHealing();
        enabled = false;
    }

    private float CalculateHeal(int maxHealth)
    {
        float percent = _healPercent * Time.deltaTime;
        float heal = maxHealth * percent;

        return heal;
    }

    private void EndHealing()
    {
        for (int i = 0; i < _leader.Followers.Count; i++)
            _leader.Followers[i].Character.EndHeal();
    }

    private IEnumerator Healing()
    {
        while (true)
        {
            yield return new WaitForSeconds(_healInterval * Time.deltaTime);

            for (int i = 0; i < _leader.Followers.Count; i++)
            {
                if (_leader.Followers[i].Character.Health != _leader.Followers[i].Character.MaxHealth && _leader.Followers[i].MoveDirection == Vector3.zero)
                {
                    _leader.Followers[i].Character.ApplyHeal(CalculateHeal(_leader.Followers[i].Character.MaxHealth));
                }
            }
        }
    }

    private void Activity(bool isActive) =>
        enabled = isActive;
}
