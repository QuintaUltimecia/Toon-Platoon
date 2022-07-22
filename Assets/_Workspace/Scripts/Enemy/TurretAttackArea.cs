using UnityEngine;
using System.Collections.Generic;

public class TurretAttackArea : MonoBehaviour
{
    private GameObject _gameObject;
    private SphereCollider _collider;

    [SerializeField] private Turret _turret;

    [SerializeField] private List<Character> _characterPool = new List<Character>();

    private void Awake()
    {
        _gameObject = gameObject;
        _collider = GetComponent<SphereCollider>();
    }

    public void InitEnemyAI(Turret value) =>
        _turret = value;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character character))
        {
            AddCharacterPool(character);
            character.AddDeathOnEnemyEvent(_turret.PlayerDeathEvent);

            _turret.SetTarget(character);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Character character))
        {
            RemoveCharacterPool(character);
            character.RemoveDeathOnEnemyEvent(_turret.PlayerDeathEvent);
            _turret.SetTarget(GetCurrentCharacter());
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
            _turret.Character = null;
    }

    public GameObject GameObject { get => _gameObject; }
    public float Radius { set => _collider.radius = value; }
}
