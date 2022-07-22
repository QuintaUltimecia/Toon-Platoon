using UnityEngine;
using UnityEngine.UI;

public class ExpUI : MonoBehaviour
{
    private CharacterMenu _characterMenu;

    [SerializeField] private Slider _slider;
    [SerializeField] private Text _text;
    [SerializeField] private Image _rank;

    private RectTransform _rectTransform;
    
    public GameObject GameObject { get; set; }

    private void Awake()
    {
        _characterMenu = transform.parent.GetComponent<CharacterMenu>();
        _rectTransform = GetComponent<RectTransform>();
        GameObject = gameObject;
    }

    private void OnEnable()
    {
        _characterMenu._expEvent.AddListener(UpdateExpUI);
    }

    private void OnDisable()
    {
        _characterMenu._expEvent.RemoveListener(UpdateExpUI);
    }

    private void UpdateExpUI(int value)
    {
        _slider.minValue = 0;
        _slider.maxValue = _characterMenu.TargetExp;
        _slider.value = value;

        if (_characterMenu.TargetExp != 0)
            _text.text = $"{_slider.value}/{_slider.maxValue}";
        else _text.text = "MAX";
    }

    public void UpdateSprite(Sprite sprite)
    {
        _rank.sprite = sprite;
    }

    public RectTransform RectTransform { get => _rectTransform; }
}
