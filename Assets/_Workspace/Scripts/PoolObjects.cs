using UnityEngine;
using System.Collections.Generic;

public class PoolObjects<T> where T : MonoBehaviour
{
    public T Prefab { get; }
    public bool AutoExpand { get; set; }
    public Transform Container { get; }
    public List<T> Pool { get => _pool; }

    private List<T> _pool;

    public PoolObjects(T prefab, int count, Transform container)
    {
        Prefab = prefab;
        Container = container;

        CreatePool(count);
    }

    public PoolObjects(T prefab, int count)
    {
        Prefab = prefab;

        CreatePool(count);
    }

    private void CreatePool(int count)
    {
        _pool = new List<T>();

        for (int i = 0; i < count; i++)
            CreateObject();
    }

    private T CreateObject(bool isActive = false)
    {
        var createdObject = Object.Instantiate(Prefab, Container);
        createdObject.gameObject.SetActive(isActive);
        _pool.Add(createdObject);

        return createdObject;
    }

    public bool HasFreeElement(out T element)
    {
        foreach (var item in _pool)
            if (item.gameObject.activeInHierarchy == false)
            {
                element = item;

                return true;
            }

        element = null;
        return false;
    }

    public T GetFreeElement(Vector3 position)
    {
        if (HasFreeElement(out var element) == true)
        {
            element.transform.position = position;
            element.gameObject.SetActive(true);

            return element;
        }
        else
        {
            if (AutoExpand == true)
                return CreateObject(true);
        }

        throw new System.Exception($"There is not free elements in pool of type {typeof(T)}");
    }
}
