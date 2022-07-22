using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class FirstAidKit : MonoBehaviour
{
    [Header("Features")]
    [SerializeField] private int _healPercent = 50;
    [SerializeField] private float _healInterval = 1f;
    [SerializeField] private int _healTime = 5;

    [Header("Links")]
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private Transform _healZoneTransform;
    [SerializeField] private ParticleSystem _particle;

    private float _duration = 0.2f;
    private float _nextOffset = 1.5f;
    private float _endOffset = 0f;

    private Vector3 _nextScale;
    private Vector3 _endScale;

    private GameObject _gameObject;
    private Animator _animator;

    private List<Coroutine> _healRoutinePool = new List<Coroutine>();
    private List<Character> _endHealPool = new List<Character>();
    private Coroutine _routine;

    private Tweener _jellyScaleTween;

    private bool _isHealing = false;

    private void Awake()
    {
        _gameObject = gameObject;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _healZoneTransform.localScale = Vector3.zero;

        _nextScale = new Vector3(_nextOffset, _nextOffset, _nextOffset);
        _endScale = new Vector3(_endOffset, _endOffset, _endOffset);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character character))
        {
            if (character.Health == character.MaxHealth)
                return;

            CollapseWithJelly();

            if (_isHealing == false)
            {
                StartCoroutine(StopHealing());
                _healZoneTransform.DOScale(Vector3.one, 2f);
                _particle.Play();
                _isHealing = true;
            }

            _healRoutinePool.Add(StartCoroutine(Healing(character)));
            _endHealPool.Add(character);
            _animator.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Character character))
        {
            for (int i = 0; i < _endHealPool.Count; i++)
            {
                if (_endHealPool[i] == character)
                {
                    _routine = _healRoutinePool[i];
                    _endHealPool[i].EndHeal();
                    StopCoroutine(_routine);
                }
            }
        }
    }

    private void CollapseWithJelly()
    {
        if (_jellyScaleTween == null)
            _jellyScaleTween = _modelTransform.DOScale(_nextScale, _duration).OnComplete(EndCollapseWithJelly);
    }

    private void EndCollapseWithJelly()
    {
        _modelTransform.DOScale(_endScale, _duration).SetEase(Ease.Linear);
    }

    private IEnumerator Healing(Character character)
    {
        int maxHealth = character.GetMaxHealth();
        float percent = _healPercent / 100f;
        int heal = (int)(maxHealth * percent);

        while (_isHealing == true)
        {
            character.ApplyHeal(heal);
            yield return new WaitForSeconds(_healInterval);
        }
    }

    private IEnumerator StopHealing()
    {
        yield return new WaitForSeconds(_healTime);

        for (int i = 0; i < _endHealPool.Count; i++)
            _endHealPool[i].EndHeal();

        _isHealing = false;
        _particle.Stop();
        _gameObject.SetActive(false);
    }
}
