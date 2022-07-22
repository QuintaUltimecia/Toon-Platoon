using UnityEngine;

public class GreanadeLauncher : Gun 
{
    [SerializeField] private Grenade _grenade2;

    private SaveManager _saveManager;
    private SaveData _data;

    private void Awake()
    {
        _saveManager = new SaveManager();

        _data = _saveManager.Load();

        if (_data.GunNumber_5 > 0)
            _bulletPrefab = _grenade2;
    }
}
