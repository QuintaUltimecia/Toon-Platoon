using UnityEngine;

public class PlayerRiggingControl : MonoBehaviour
{
    [SerializeField] private Transform _rotateRig;

    private Vector3 _startPosition;

    private void Awake()
    {
        _startPosition = _rotateRig.localPosition;
    }

    public void SetPosition(Vector3 position)
    {
        _rotateRig.position = position;
    }

    public void ResetPosition()
    {
        _rotateRig.localPosition = _startPosition;
    }
}
