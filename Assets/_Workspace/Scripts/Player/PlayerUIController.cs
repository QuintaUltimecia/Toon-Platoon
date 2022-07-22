using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject _healthUI;

    private Transform _safeArea;
    private Transform _transform;

    private Image _healthSlider;
    private Image _overtakeSlider;

    private Character _character;

    private bool _disableIsActive = false;

    private void Awake()
    {
        _safeArea = FindObjectOfType<Canvas>().transform.Find(nameof(SafeArea));
        _transform = transform;
        _character = _transform.parent.GetComponent<Character>();
        _healthSlider = _healthUI.transform.Find("Slider").GetComponent<Image>();
        _overtakeSlider = _healthUI.transform.Find("SliderUnder").GetComponent<Image>();
    }

    private void OnEnable()
    {
        _character.AddApplyDamageEvent(UpdateHealthSlider);
        _character.AddApplyHealEvent(UpdateHealthSliderOnHeal);
        _character.AddDeathEvent(DisableHealthSlider);
    }

    private void OnDisable()
    {
        _character.RemoveApplyDamageEvent(UpdateHealthSlider);
        _character.RemoveApplyHealEvent(UpdateHealthSliderOnHeal);
        _character.RemoveDeathEvent(DisableHealthSlider);
    }

    private void Start()
    {
        UpdateHealthSlider();
        _healthUI.SetActive(false);
    }

    private float HealthCast()
    {
        float value;

        try
        {
            value = (float)_character.Health / (float)_character.MaxHealth;
        }
        catch
        {
            value = 0;
        }

        return value;
    }

    private IEnumerator UpdateOvertakeSlider()
    {
        yield return new WaitForSeconds(0.5f);
        _overtakeSlider.DOFillAmount(_healthSlider.fillAmount, 1f).SetEase(Ease.Linear);
    }

    private void UpdateHealthSlider()
    {
        if (_disableIsActive == false)
        {
            StartCoroutine(DisableHealthSliderOnTime());
            StartCoroutine(UpdateOvertakeSlider());
        }
        else
        {
            StopAllCoroutines();
            _disableIsActive = false;
            StartCoroutine(DisableHealthSliderOnTime());
            StartCoroutine(UpdateOvertakeSlider());
        }

        _healthUI.SetActive(true);
        _healthUI.transform.SetParent(_safeArea.transform);
        _healthUI.transform.rotation = new Quaternion(0, 0, 0, 0);

        _healthSlider.fillAmount = HealthCast();
    }

    private IEnumerator UpdateOvertakeSliderOnHeal()
    {
        yield return new WaitForSeconds(0.5f);
        _healthSlider.DOFillAmount(_overtakeSlider.fillAmount, 1f).SetEase(Ease.Linear);
    }

    private void UpdateHealthSliderOnHeal()
    {
        if (_disableIsActive == false)
        {
            StartCoroutine(DisableHealthSliderOnTime());
            StartCoroutine(UpdateOvertakeSliderOnHeal());
        }
        else
        {
            StopAllCoroutines();
            _disableIsActive = false;
            StartCoroutine(DisableHealthSliderOnTime());
            StartCoroutine(UpdateOvertakeSliderOnHeal());
        }

        _healthUI.SetActive(true);
        _healthUI.transform.SetParent(_safeArea.transform);
        _healthUI.transform.rotation = new Quaternion(0, 0, 0, 0);

        _overtakeSlider.fillAmount = HealthCast();
    }

    private IEnumerator DisableHealthSliderOnTime()
    {
        _disableIsActive = true;
        yield return new WaitForSeconds(3f);

        DisableHealthSlider();
    }

    private void DisableHealthSlider()
    {
        _healthUI.SetActive(false);
        _disableIsActive = false;
    }

    public Transform Transform { get => _transform; }
}
