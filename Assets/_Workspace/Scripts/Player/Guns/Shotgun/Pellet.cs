using UnityEngine;

public class Pellet : Bullet
{
    private Vector3 _offset;

    private void OnEnable()
    {
        _offset = new Vector3(
            x: Random.Range(-0.1f, 0.1f),
            y: Random.Range(-0.1f, 0.1f),
            z: Random.Range(-0.1f, 0.1f));

        _force = Random.Range(5, 10);
    }

    public override void Force()
    {
        _transform.position = _bulletSpawnTransform.position;
        _transform.rotation = _bulletSpawnTransform.rotation;

        _rigidbody.AddForce((_transform.TransformDirection(Vector3.forward) + _offset) * _force, ForceMode.Impulse);
    }
}
