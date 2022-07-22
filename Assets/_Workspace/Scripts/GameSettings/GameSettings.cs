using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private int _targetFPS = 60;

    private void Awake()
    {
        Application.targetFrameRate = _targetFPS;
    }
}
