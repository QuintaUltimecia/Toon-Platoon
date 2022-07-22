using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DamageUI : MonoBehaviour
{
    [SerializeField] private Transform _textTransform;
    private RectTransform _rectTransform;
    private EnemyUIController _enemyUIController;

    private TextMeshProUGUI _text;
    private Camera _camera;
    private Transform _parent;
    private Vector3 _startPosition;
    private Vector3 _offset;

    private void Awake()
    {
        _text = _textTransform.GetComponent<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
        _camera = Camera.main;
        _parent = _rectTransform.parent;

        Text = _textTransform.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        SetPosition();
    }

    private void SetPosition()
    {
        _rectTransform.position = _camera.WorldToScreenPoint(_enemyUIController.Transform.position);
    }

    public void InitEnemyUIController(EnemyUIController enemyUIController)
    {
        _enemyUIController = enemyUIController;
    }

    public void CompleteDrop()
    {
        _textTransform.localPosition = Vector3.zero;
        _rectTransform.SetParent(_parent);
        gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        _text.text = text;

        _textTransform.localPosition = Vector3.zero;
        _rectTransform.rotation = new Quaternion(0, 0, 0, 0);
        _offset = new Vector3(Random.Range(-100, 100), Random.Range(0, 100), 0);
        _textTransform.DOLocalMove(_offset, 0.5f).OnComplete(CompleteDrop).SetEase(Ease.Linear);
    }

    public TextMeshProUGUI Text { get; set; }
}
