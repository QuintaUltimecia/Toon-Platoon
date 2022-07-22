using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _offset;
    [SerializeField] private float _duration;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update() =>
        Move();

    private void Move()
    {
        Vector3 direction = new Vector3(
            x: _playerTransform.position.x,
            y: _transform.position.y,
            z: _playerTransform.position.z - _offset);

        _transform.position = Vector3.Slerp(
            a: _transform.position,
            b: direction,
            t: _duration * Time.deltaTime);
    }
}
