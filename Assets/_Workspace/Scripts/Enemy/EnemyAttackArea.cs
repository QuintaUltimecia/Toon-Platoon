using UnityEngine;
using System.Collections.Generic;

public class EnemyAttackArea : MonoBehaviour
{
    private GameObject _gameObject;
    private SphereCollider _collider;

    private EnemyAI _enemyAI;

    [SerializeField] private List<Character> _characterPool = new List<Character>();

    private void Awake()
    {
        _gameObject = gameObject;
        _collider = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        _collider.radius = _enemyAI.Enemy.Features.AttackRadius;
    }

    public void InitEnemyAI(EnemyAI value) =>
        _enemyAI = value;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character character))
        {
            AddCharacterPool(character);
            character.AddDeathOnEnemyEvent(_enemyAI.PlayerDeathEvent);

            if (_enemyAI.Gun.enabled == true)
            {
                _enemyAI.Gun.SetTarget(character);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Character character))
        {
            character.RemoveDeathOnEnemyEvent(_enemyAI.PlayerDeathEvent);
        }
    }

    public Character GetCurrentCharacter()
    {
        if (_characterPool.Count > 0)
            return _characterPool[0];

        return null;
    }

    private void AddCharacterPool(Character value)
    {
        int count = 0;

        for (int i = 0; i < _characterPool.Count; i++)
        {
            if (value == _characterPool[i])
                count++;
        }

        if (count == 0) _characterPool.Add(value);
    }

    public void RemoveAllCharacter()
    {
        _characterPool.Clear();
    }

    public void RemoveCharacterPool(Character value)
    {
        if (value != null)
            _characterPool.Remove(value);

        if (_characterPool.Count == 0)
            _enemyAI.Character = null;
    }

    public GameObject GameObject { get => _gameObject; }
}
