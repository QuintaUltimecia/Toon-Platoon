using UnityEngine;

public class EnemyBullet : Bullet
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character character))
        {
            character.ApplyDamage(_damage);
        }

        _gameObject.SetActive(false);
    }
}
