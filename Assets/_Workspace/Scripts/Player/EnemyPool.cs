using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class EnemyPool : MonoBehaviour
{
    private List<Enemy> _enemyPool = new List<Enemy>();

    private Leader _leader;

    private bool _isEmptyEnemy = true;

    private void Awake()
    {
        _leader = GetComponent<Leader>();
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (_enemyPool.Count > 0)
            for (int i = 0; i < _enemyPool.Count; i++)
                if (enemy == _enemyPool[i]) _enemyPool.Remove(enemy);

        if (_enemyPool.Count == 0)
            _isEmptyEnemy = true;
    }

    public bool CheckEnemy(Enemy enemy)
    {
        int count = 0;

        for (int i = 0; i < _enemyPool.Count; i++)
        {
            if (enemy == _enemyPool[i]) count++;
        }

        if (count == 0)
            return true;
        else return false;
    }

    public void AddEnemy(Enemy enemy)
    {
        if (enemy != null && enemy.enabled != false)
            if (CheckEnemy(enemy) == true)
            {
                _enemyPool.Add(enemy);
            }

        for (int i = 0; i < _leader.FollowerCount; i++)
        {
            if (_leader.Followers[i].Character.Gun.enabled == true)
            {
                _leader.Followers[i].Character.Gun.SetTarget(enemy);
                _leader.Followers[i].Character.Gun.enabled = false;
            }
        }

        _isEmptyEnemy = false;
    }

    public void OnDeathUnit(UnityAction<Enemy> action)
    {
        if (_enemyPool.Count > 0)
            for (int i = 0; i < _enemyPool.Count; i++)
                _enemyPool[i].RemoveEnemyEvent(action);
    }

    public Enemy Target()
    {
        if (_enemyPool.Count != 0)
        {
            for (int i = 0; i < _enemyPool.Count; i++)
            {
                if (_enemyPool[i].EntityValue == Enemy.Entity.Barrel)
                    return _enemyPool[i];
            }

            return _enemyPool[Random.Range(0, _enemyPool.Count)];
        }
        else return null;
    }

    public bool VisibleEnemy()
    {
        int count = 0;

        for (int i = 0; i < _leader.FollowerCount; i++)
        {
            if (_leader.Followers[i].Character.VisibilityArea.IsVisible == true)
                count++;
        }

        if (count == 0) return false;
        else return true;
    }

    public bool IsEmptyEnemy { get => _isEmptyEnemy; }
}
