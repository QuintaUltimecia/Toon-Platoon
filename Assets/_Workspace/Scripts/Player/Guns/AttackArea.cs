using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AttackArea : MonoBehaviour
{
    private SphereCollider _collider;
    private Character _character;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _character = transform.parent.GetComponent<Character>();
    }

    private void Start()
    {
        _collider.radius = _character.GunSO._attackRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            _character.EnemyPool.AddEnemy(enemy);
            enemy.EnemyPool = _character.EnemyPool;

            if (_character.Gun.enabled == true)
            {
                _character.Gun.SetTarget(enemy);
                _character.Gun.enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            RemoveEnemy(enemy);
            enemy.RemoveEnemyEvent(RemoveEnemy);
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        _character.EnemyPool.RemoveEnemy(enemy);

        if (_character.EnemyPool.Target() == null)
        {
            _character.Gun.SetTarget(null);
            Enemy = null;
        }
        else
        {
            enemy = _character.EnemyPool.Target();
            _character.Gun.SetTarget(enemy);
        }
    }

    public Enemy Enemy { get; set; }
}
