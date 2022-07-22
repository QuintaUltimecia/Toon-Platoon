using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FeaturesUI : MonoBehaviour
{
    [SerializeField] private Text _healthUI;
    [SerializeField] private Text _damageUI;

    private RectTransform _rectTransform;
    public GameObject GameObject { get; set; }

    private Tweener _tweener;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        GameObject = gameObject;

        ResetFeatures();
    }

    public void ResetFeatures()
    {
        _rectTransform.localScale = Vector3.zero;

        if (_tweener != null)
            _tweener.Kill();
    }

    public void GetFeatures(int health, int damage)
    {
        _healthUI.text = $"{health}";
        _damageUI.text = $"{damage}";

        _tweener = _rectTransform.DOScale(1, 1f).SetEase(Ease.Linear);
    }
}
