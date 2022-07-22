using UnityEngine;

public class ExpUIParenter : MonoBehaviour
{
    [SerializeField] private CharacterMenu[] _characterMenus;

    public void OnRemoveParent()
    {
        for (int i = 0; i < _characterMenus.Length; i++)
        {
            if (_characterMenus[i].gameObject.activeInHierarchy)
                _characterMenus[i].RemoveExpUIParent();
        }
    }
}
