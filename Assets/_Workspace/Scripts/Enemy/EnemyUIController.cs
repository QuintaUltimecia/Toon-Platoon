using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyUIController : MonoBehaviour
{
    [SerializeField] private DamageUI _damageUIPrefab;
    [SerializeField] private GameObject _healthUI;

    private Transform _safeArea;
    private Transform _transform;

    private Image _healthSlider;
    private Image _overtakeSlider;
    private Enemy _enemy;

    private int _poolCount = 6;
    private bool _autoExpand = true;
    private PoolObjects<DamageUI> _damageUIPool;

    private bool _disableIsActive = false;

    private void Awake()
    {
        _safeArea = FindObjectOfType<Canvas>().transform.Find(nameof(SafeArea));
        _transform = transform;
        _enemy = _transform.parent.GetComponent<Enemy>();
        _healthSlider = _healthUI.transform.Find("Slider").GetComponent<Image>();
        _overtakeSlider = _healthUI.transform.Find("SliderUnder").GetComponent<Image>();
    }

    private void OnEnable()
    {
        _enemy.AddApplyDamageEvent(OnDamageUI);
        _enemy.AddApplyDamageEvent(UpdateHealthSlider);
        _enemy.AddDeathEvent(DisableHealthSlider);
    }

    private void OnDisable()
    {
        _enemy.RemoveApplyDamageEvent(OnDamageUI);
        _enemy.RemoveApplyDamageEvent(UpdateHealthSlider);
        _enemy.RemoveDeathEvent(DisableHealthSlider);
    }

    private void Start()
    {
        UpdateHealthSlider();
        _healthUI.SetActive(false);

        _damageUIPool = new PoolObjects<DamageUI>(_damageUIPrefab, _poolCount, _transform);
        _damageUIPool.AutoExpand = _autoExpand;
    }

    private void CreateDamageUI()
    {
        var damageUI = _damageUIPool.GetFreeElement(Vector3.zero);

        damageUI.InitEnemyUIController(this);
        damageUI.transform.SetParent(_safeArea.transform);
        damageUI.SetText(_enemy.TakedDamage.ToString());
    }

    private float HealthCast()
    {
        float value;

        try
        {
            value = (float)_enemy.Health / (float)_enemy.MaxHealth;
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

    private void OnDamageUI()
    {
        CreateDamageUI();
    }

    public Transform Transform { get => _transform; }
}
