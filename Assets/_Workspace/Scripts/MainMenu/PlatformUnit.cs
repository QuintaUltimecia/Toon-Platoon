using UnityEngine;
using DG.Tweening;

public class PlatformUnit : MonoBehaviour
{
    [SerializeField] private ReplaceWeaponModel _replaceWeaponModel;
    [SerializeField] private UnitAnimationController _unitAnimationController;
    [SerializeField] private Transform _selection;

    private Tweener _tween;

    private void Awake()
    {
        SetSelection(false);
    }

    public void SetSelection(bool isActive)
    {
        if (isActive == true)
        {
            _tween = _selection.DOScale(Vector3.one, 0.3f).SetEase(Ease.Linear);
        }
        else
        {
            _tween.Kill();
            _selection.localScale = new Vector3(1, 0, 1);
        }
    }

    public ReplaceWeaponModel ReplaceWeaponModel { get => _replaceWeaponModel; }
    public UnitAnimationController UnitAnimationController { get => _unitAnimationController; }
}
