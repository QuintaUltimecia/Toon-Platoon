using UnityEngine;

public class HeavyActiveGun : MonoBehaviour
{
    [SerializeField] private GameObject _secondElement;

    private void OnEnable()
    {
        _secondElement.SetActive(true);
    }

    private void OnDisable()
    {
        _secondElement.SetActive(false);
    }
}
