using UnityEngine;

public class ReplaceWeaponModel : MonoBehaviour
{
    [SerializeField] private GameObject[] _weapons;

    public void ActiveWeapon(int value)
    {
        _weapons[value].SetActive(true);

        for (int i = 0; i < _weapons.Length; i++)
        {
            if (value == i)
                _weapons[i].SetActive(true);
            else _weapons[i].SetActive(false);
        }
    }
}
