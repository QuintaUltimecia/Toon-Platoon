using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private Sprite _clickedSprite;
    [SerializeField] private float _pressingDepth = 6f;

    [Space(15f)]
    [SerializeField] private UnityEvent _actions;
    [SerializeField] private UnityEvent _actionsUp;
    [SerializeField] private UnityEvent _actionsDown;
    private float _durationOnUp = 0.25f;
    private float _durationDefault;
    private float _durationMultiplier = 0.01f;

    private Image _buttonImage;
    private Sprite _defaultSprite;
    private RectTransform _selfRTransform;

    private Animator _animator;

    private Coroutine _pointerUpCoroutine;

    private void Awake()
    {
        _selfRTransform = GetComponent<RectTransform>();
        _buttonImage = GetComponent<Image>();
        _defaultSprite = _buttonImage.sprite;
        _animator = GetComponent<Animator>();

        _durationDefault = _durationOnUp;
    }

    public void GetActions()
    {
        _actions?.Invoke();
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        _actions?.Invoke();
        _actionsUp?.Invoke();
    }

    public void OnPointerDown(PointerEventData pointerData)
    {
        if (_animator != null)
            _animator.enabled = false;

        //_selfRTransform.anchoredPosition = new Vector2(_selfRTransform.anchoredPosition.x, _selfRTransform.anchoredPosition.y - _pressingDepth);
        _selfRTransform.Translate(Vector2.down * _pressingDepth);

        if (_clickedSprite != null)
            _buttonImage.sprite = _clickedSprite;

        _durationOnUp = _durationDefault;
        _pointerUpCoroutine = StartCoroutine(PointerUp());
        _actionsDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData pointerData)
    {
        if (_animator != null)
            _animator.enabled = true;

        //_selfRTransform.anchoredPosition = new Vector2(_selfRTransform.anchoredPosition.x, _selfRTransform.anchoredPosition.y + _pressingDepth);
        _selfRTransform.Translate(Vector2.up * _pressingDepth);

        if (_clickedSprite != null)
            _buttonImage.sprite = _defaultSprite;

        StopCoroutine(_pointerUpCoroutine);
    }

    private IEnumerator PointerUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(_durationOnUp);
            _durationOnUp -= _durationMultiplier;
            _actionsUp?.Invoke();
        }
    }
}
