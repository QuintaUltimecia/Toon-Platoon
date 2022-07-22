using UnityEngine;
using DG.Tweening;

public class ArrowUI : MonoBehaviour
{
    private RectTransform _rectTransform;

    private float _offset = 10f;

    private Vector3 _nextPosition;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _nextPosition = new Vector3(
            x: _rectTransform.localPosition.x,
            y: _rectTransform.localPosition.y + _offset,
            z: _rectTransform.localPosition.z);

        _rectTransform.DOLocalMove(_nextPosition, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}
