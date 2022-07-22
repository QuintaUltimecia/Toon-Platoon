
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] Transform _box;
    private Rigidbody[] _debris;
    private Enemy _enemy;

    private void Awake()
    {
        int count = _box.childCount;

        _debris = new Rigidbody[count];

        for (int i = 0; i < count; i++)
        {
            Transform child = _box.GetChild(i);

            _debris[i] = child.GetComponent<Rigidbody>();
        }

        _enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        _enemy.AddDeathEvent(ExplosionEvent);
    }

    private void OnDisable()
    {
        _enemy.RemoveDeathEvent(ExplosionEvent);
    }

    private void Start()
    {
        Replace(false);
    }

    private void ExplosionEvent()
    {
        Replace(true);
    }

    private void Replace(bool isActive)
    {
        for (int i = 0; i < _debris.Length; i++)
        {
            _debris[i].isKinematic = !isActive;
        }
    }
}
