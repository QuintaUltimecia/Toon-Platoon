using UnityEngine;
using DG.Tweening;
using UnityEngine.Animations.Rigging;

public class Spread : MonoBehaviour
{
    [SerializeField] private float _offset = 0.6f;
    [SerializeField] private float _duration = 0.4f;

    private Transform _transform;

    private Vector3 _startPosition;
    private Vector3 _leftPosition;
    private Vector3 _rightPosition;

    private Tween _spreadTween;

    [SerializeField] private Rig _rig;

    private void Awake()
    {
        _transform = transform;

        _startPosition = new Vector3(
            x: _transform.localPosition.x,
            y: _transform.localPosition.y,
            z: _transform.localPosition.z);
    }

    private void Start()
    {
        _leftPosition = new Vector3(
            x: _transform.localPosition.x + _offset,
            y: _transform.localPosition.y,
            z: _transform.localPosition.z);

        _rightPosition = new Vector3(
            x: _transform.localPosition.x + (_offset * 2),
            y: _transform.localPosition.y,
            z: _transform.localPosition.z);
    }

    public void SetOffset(float value)
    {
        _offset = value;
    }

    public void SetDuration(float value)
    {
        _duration = value;
    }

    public void EnableSpread()
    {
        if (_spreadTween != null && _offset != 0)
            _spreadTween.Kill();

        _spreadTween = _transform.DOLocalMove(_leftPosition, _duration)
            .SetEase(Ease.Linear).OnComplete(NextPosition);
    }

    private void NextPosition()
    {
        if (_spreadTween != null)
            _spreadTween.Kill();

        _spreadTween = _transform.DOLocalMove(_rightPosition, _duration)
            .SetEase(Ease.Linear).OnComplete(EnableSpread);
    }

    public void DisableSpread()
    {
        if (_spreadTween != null)
        {
            _spreadTween.Kill();

            _transform.localPosition = _startPosition;
        }
    }

    public Rig Rig { get => _rig; }
}
