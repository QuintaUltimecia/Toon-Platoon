using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private EnemyAI _enemyAI;

    private EnemyGun _gun;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _gun = _enemyAI?.Gun;
    }

    private int DEATH = Animator.StringToHash("isDeath");
    private int MOVE = Animator.StringToHash("isMove");
    private int ATTACK = Animator.StringToHash("isAttack");
    private int ATTACK_SPEED = Animator.StringToHash("AttackSpeed");
    private int DEATH_BLEND = Animator.StringToHash("DeathBlend");

    private void Start()
    {
        AttackSpeed(_enemyAI.Enemy.Features.AttackSpeed);

        _animator.Play("Base Layer.Blend_Tree_Idle", 0, Random.Range(0f, 0.25f));
    }

    public void OnAttack()
    {
        _enemyAI.Gun.Attack();
    }

    private void OnEnable()
    {
        if (_enemy != null)
            _enemy.AddDeathEvent(Death);
        if (_enemyAI != null)
        {
            _enemyAI.Gun.AddAttackEvent(Attack);
            _enemyAI.AddMoveEvent(Move);
        }
    }

    private void OnDisable()
    {
        if (_enemy != null)
            _enemy.AddDeathEvent(Death);
        if (_enemyAI != null)
        {
            _enemyAI.Gun.RemoveAttackEvent(Attack);
            _enemyAI.RemoveMoveEvent(Move);
        }
    }

    private void Death()
    {
        _animator.SetFloat(DEATH_BLEND, Random.Range(0, 3));

        _animator.SetBool(DEATH, true);
    }

    private void Attack(bool value) =>
        _animator.SetBool(ATTACK, value);

    private void Move(bool value) =>
        _animator.SetBool(MOVE, value);

    private void AttackSpeed(float value) =>
    _animator.SetFloat(ATTACK_SPEED, value);
}
