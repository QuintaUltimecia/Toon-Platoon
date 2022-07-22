using UnityEngine;

public class UnitAnimationController : MonoBehaviour
{
    private CharacterMenu _characterMenu;

    private Animator _animator;

    private int IDLE_VARIANT = Animator.StringToHash("Idle");
    private int DEATH = Animator.StringToHash("isDeath");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.Play("Base Layer.BlendTreeIdle", 0, Random.Range(0f, 0.25f));
    }

    private void OnDisable()
    {
        _characterMenu?._selectEvent.RemoveListener(IdleSelectable);
        _characterMenu?._selectIdleEvent.RemoveListener(VariantSelectable);
        _characterMenu._deathEvent.RemoveListener(Death);
    }

    private int IDLE_SELECT = Animator.StringToHash("OnSelect");

    private void IdleSelectable() =>
        _animator.SetTrigger(IDLE_SELECT);

    public void VariantSelectable(int value) =>
        _animator.SetFloat(IDLE_VARIANT, value);

    public void Death(bool value) =>
        _animator.SetBool(DEATH, value);

    public void InitCharacterMenu(CharacterMenu value)
    {
        _characterMenu = value;
        _characterMenu._selectEvent?.AddListener(IdleSelectable);
        _characterMenu._selectIdleEvent?.AddListener(VariantSelectable);
        _characterMenu._deathEvent.AddListener(Death);
    }
}
