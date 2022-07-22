using UnityEngine;
using DG.Tweening;

public class Exp : MonoBehaviour
{
    public RectTransform TargetTransform { get; set; }

    private RectTransform _transform;
    private GameObject _gameObject;

    private TweenCallback _tween;
    private Tweener _tweener;

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
        _gameObject = gameObject;
    }

    private void OnEnable()
    {
        _tween += Activate;
        _transform.localScale = Vector3.one;
    }

    private void OnDisable()
    {
        _tween = null;
    }

    public void Move()
    {
        if(TargetTransform != null)
            _tweener = _transform.DOMove(TargetTransform.position, 1f).OnComplete(Complete);
    }

    public void CompleteOnRank()
    {
        if (TargetTransform != null)
            _tweener.Kill();

        _tweener = _transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.Linear).OnComplete(Activate);
    }

    private void Complete()
    {
        if (TargetTransform != null)
            _tweener.Kill();

            _tweener = _transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.Linear).OnComplete(_tween);
    }

    private void Activate() =>
        _gameObject.SetActive(false);

    public void AddTween(TweenCallback tween) =>
        _tween += tween;
}
