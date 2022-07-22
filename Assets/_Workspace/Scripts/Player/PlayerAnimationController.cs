using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Follower _follower;
    [SerializeField] private Character _character;

    private Gun _gun;

    private Animator _animator;

    [HideInInspector] public UnityEvent<bool> ActiveGunEvent = new UnityEvent<bool>();

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _gun = _character?.Gun;
    }

    private int MOVE = Animator.StringToHash("isMove");
    private int ATTACK = Animator.StringToHash("isAttack");
    private int ATTACK_SPEED = Animator.StringToHash("AttackSpeed");
    private int DEATH = Animator.StringToHash("isDeath");
    private int BLEND = Animator.StringToHash("Blend");
    private int CAPTIVE = Animator.StringToHash("isCaptive");
    private int DEATH_BLEND = Animator.StringToHash("DeathBlend");

    public void OnAttack()
    {
        _gun.Attack();
    }

    public void SetGun() =>
        ActiveGunEvent.Invoke(true);

    public void ResetGun() =>
        ActiveGunEvent.Invoke(false);

    private void OnEnable()
    {
        if (_follower != null)
            _follower.AddMoveEvent(Move);

        if (_gun != null)
            _gun.AddAttackEvent(Attack);

        if (_character != null)
        {
            _character.AddDeathEvent(Death);
            _character.AddCaptiveEvent(Captive);
            _character.OnInitGunEvent.AddListener(Blend);

            AttackSpeed(_character.GunSO._attackDuration);
        }
    }

    private void OnDisable()
    {
        if (_follower != null)
            _follower.RemoveMoveEvent(Move);

        if (_gun != null)
            _gun.RemoveAttackEvent(Attack);

        if (_character != null)
        {
            _character.RemoveDeathEvent(Death);
            _character.RemoveCaptiveEvent(Captive);
            _character.OnInitGunEvent.RemoveListener(Blend);
        }
    }

    private void Blend(int value) =>
        _animator.SetFloat(BLEND, value);

    private void Move(bool value) =>
        _animator.SetBool(MOVE, value);

    private void Attack(bool value) =>
        _animator.SetBool(ATTACK, value);

    private void AttackSpeed(float value)
    {
        _animator.SetFloat(ATTACK_SPEED, value);
    }

    private void Captive(bool value) =>
        _animator.SetBool(CAPTIVE, value);

    private void Death()
    {
        _animator.SetFloat(DEATH_BLEND, Random.Range(0, 3));

        _animator.SetBool(DEATH, true);
    }
}
