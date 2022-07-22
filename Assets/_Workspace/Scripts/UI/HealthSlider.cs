using UnityEngine;

public class HealthSlider : MonoBehaviour
{
    private Transform _transform;

    private Camera _camera;

    private Transform _enemyUIController;

    private void Awake()
    {
        _transform = transform;
        _camera = Camera.main;
        _enemyUIController = _transform.parent;
    }

    private void Update()
    {
        SetPosition();
    }

    private void SetPosition()
    {
        _transform.position = _camera.WorldToScreenPoint(_enemyUIController.position);
    }
}
