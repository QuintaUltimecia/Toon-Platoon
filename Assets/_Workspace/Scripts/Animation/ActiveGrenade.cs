using UnityEngine;

public class ActiveGrenade : MonoBehaviour
{
    [SerializeField] 
    private PlayerAnimationController _playerAnimationController;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        _playerAnimationController.ActiveGunEvent?.AddListener(ActiveMesh);
    }

    private void OnDisable()
    {
        _playerAnimationController.ActiveGunEvent?.RemoveListener(ActiveMesh);
    }

    private void ActiveMesh(bool isActive)
    {
        _meshRenderer.enabled = isActive;
    }
}
