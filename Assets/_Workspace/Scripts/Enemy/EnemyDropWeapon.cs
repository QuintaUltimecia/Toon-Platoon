using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyDropWeapon : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    private float _reshadeDuration = 1f;

    private Rigidbody _rigidbody;
    private Collider _collider;
    private Transform _transform;

    private MeshRenderer _meshRenderer;

    private Shader _shader;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _transform = transform;
        _meshRenderer = GetComponent<MeshRenderer>();
        _shader = Shader.Find("4U Games/Toon Fade");
    }

    private void OnEnable() =>
        _enemy.AddDeathEvent(OnDeathEvent);

    private void OnDisable() =>
        _enemy.RemoveDeathEvent(OnDeathEvent);

    private void Start()
    {
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
    }

    private void OnDeathEvent()
    {
        _transform.parent = null;
        _collider.enabled = true;
        _rigidbody.isKinematic = false;

        StartCoroutine(DisabledWeapon());
    }

    private IEnumerator DisabledWeapon()
    {
        yield return new WaitForSeconds(5f);
        Reshade();
    }

    private void Reshade()
    {
        for (int i = 0; i < _meshRenderer.materials.Length; i++)
        {
            _meshRenderer.materials[i].shader = _shader;
            Color color = _meshRenderer.materials[i].color;
            _meshRenderer.materials[i].DOColor(new Color(color.r, color.g, color.b, 0), _reshadeDuration).SetEase(Ease.Linear).OnComplete(DestroyObject);
        }
    }

    private void DestroyObject() =>
        Destroy(gameObject);
}
