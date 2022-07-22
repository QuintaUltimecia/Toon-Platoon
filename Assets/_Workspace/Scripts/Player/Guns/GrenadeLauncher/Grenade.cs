using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Grenade : Bullet
{
    [SerializeField] private int _damagePercent = 50;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private ParticleSystem _explosionEffect;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _jumpForce = 3f;
    [SerializeField] private float _explosionForce = 48f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private bool _isRotating = false;

    private Tween _rotateTween;

    private float _damageRadius;

    private void Start()
    {
        _targetLayer = ~_targetLayer;
    }

    public override void Force()
    {
        _transform.position = _bulletSpawnTransform.position;
        _transform.rotation = Quaternion.LookRotation(_targetTransform.position - _transform.position);

        _damageRadius = Random.Range(0.1f, 2f);
        Vector3 direction = _targetTransform.position + (_targetTransform.forward * _damageRadius);
        _transform.DOJump(direction, _jumpForce, 1, 1f).SetEase(Ease.Linear);
        if (_isRotating == true) _rotateTween = _transform.DORotate(new Vector3(0, 0, 180), 2f);
    }

    public override void Shoot()
    {
        if (_targetTransform != null)
        {
            Force();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Ground ground))
        {
            DestroyLogic();
            _rotateTween.Kill();
        }
    }

    public override void DestroyLogic()
    {
        Collider[] colliders = Physics.OverlapSphere(_transform.position, _explosionRadius);

        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                Vector3 enemyPosition = new Vector3(enemy.Transform.position.x, _transform.position.y, enemy.Transform.position.z);

                float distance = Mathf.Min(Vector3.Distance(enemyPosition, _transform.position), _explosionRadius);
                float damagePercent = 100 - _damagePercent;
                float onePercent;
                try
                {
                    onePercent = damagePercent / _explosionRadius;
                }
                catch (System.Exception)
                {
                    onePercent = 0;
                }
                float percent = 100 - (onePercent * distance);
                print("distance : " + distance + " percent: " + percent + " onePercent" + onePercent);
                percent /= 100;

                int damage = (int)(_damage * percent);

                enemy.ApplyDamage(damage);

                if (enemy.Health == 0)
                {
                    enemy.EnemyAI?.EnemyRagdollControl.ControlRigidbody(false);

                    Collider[] colliders2 = Physics.OverlapSphere(_transform.position, _explosionRadius, _targetLayer);

                    foreach (Collider hit2 in colliders2)
                    {
                        if (hit2.TryGetComponent(out Rigidbody rigidbody))
                            rigidbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, 0, ForceMode.Impulse);
                    }

                    _onDamageEvent?.Invoke(enemy);
                }
            }
        }

        _rigidbody.isKinematic = true;
        _meshRenderer.enabled = false;
        _transform.rotation = new Quaternion(0, 0, 0, 0);
        _explosionEffect.Play();
        StartCoroutine(Activate());

    }

    private IEnumerator Activate()
    {
        yield return new WaitForSeconds(5f);
        _meshRenderer.enabled = true;
        _gameObject.SetActive(false);
        _rigidbody.isKinematic = false;
    }
}
