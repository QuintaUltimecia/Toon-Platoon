using UnityEngine;

public class EnemyRagdollControl : MonoBehaviour
{
    private Animator _animator;

    [SerializeField] private GameObject _enemyRagdoll;
    [SerializeField] private GameObject _enemyDefault;
    [SerializeField] private Rigidbody[] _rigidbodyes;
    [SerializeField] private Transform[] _rigsRagdoll;
    [SerializeField] private Transform[] _rigsDefault;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        ControlRigidbody(true);
    }

    public void ControlRigidbody(bool isActive)
    {
        _enemyRagdoll.SetActive(!isActive);
        _enemyDefault.SetActive(isActive);

        for (int i = 0; i < _rigsRagdoll.Length; i++)
        {
            _rigsRagdoll[i].position = _rigsDefault[i].position;
            _rigsRagdoll[i].rotation = _rigsDefault[i].rotation;
        }

        for (int i = 0; i < _rigidbodyes.Length; i++)
        {
            _rigidbodyes[i].isKinematic = isActive;
        }

        if (isActive == false)
        {
            _animator.enabled = false;
        }
    }
}
