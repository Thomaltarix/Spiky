using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _attackCooldown = 5f;

    [SerializeField] private Vector3 attackBoxSize = new Vector3(1f, 10f, 1.5f);
    [SerializeField] private float attackBoxForwardOffset = 1f;
    [SerializeField] private float attackBoxHeightOffset = 1.5f;
    [SerializeField] private LayerMask playerLayer;

    private float _attackTimeout;

    private GameObject _player;
    private PlayerStatManager _statManager;
    private NavMeshAgent _agent;
    private Animator _animator;

    private int _animIDSpeed;
    private int _animIDMotionSpeed;
    private float _animationBlend;
    private int _animIDAttack;
    private int _animIDMove;

    private const float _speedChangeRate = 10f;

    private void Start()
    {
        _attackTimeout = _attackCooldown;

        _animator = GetComponent<Animator>();
        _animIDSpeed = Animator.StringToHash("Speed");

        _animator = GetComponent<Animator>();
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        _animIDAttack = Animator.StringToHash("attack");
        _animIDMove = Animator.StringToHash("move");

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;
        _agent.updateRotation = false;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            _player = p;
            _statManager = p.GetComponent<PlayerStatManager>();
        }
    }

    public void OnFootstep(AnimationEvent evt)
    {
       //empty
    }

    public void PLayMoveAnimation(float rawSpeed) 
    {
        _animationBlend = Mathf.Lerp(_animationBlend, rawSpeed, Time.deltaTime * _speedChangeRate);

        _animator.SetFloat(_animIDSpeed, _animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, 1f);
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (_player.transform.position - transform.position);
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * 10f
        );
    }

    private void Update()
    {
        if (_player == null || _agent == null || _animator == null)
            return;

        float stopDistance = 1f;

        Vector3 dir = (_player.transform.position - transform.position).normalized;
        Vector3 targetPos = _player.transform.position - dir * stopDistance;

        _agent.SetDestination(targetPos);

        PLayMoveAnimation(_agent.velocity.magnitude);
        RotateTowardsPlayer();

        if (_agent.remainingDistance <= _attackRange && _attackTimeout <= 0) 
        {
            _animator.SetTrigger(_animIDAttack);
            _attackTimeout = _attackCooldown;
        }
        if (_attackTimeout > 0) { _attackTimeout -= Time.deltaTime; }
        
    }

    public void StartDealDamage()
    {
        Vector3 boxCenter =
            transform.position
            + transform.forward * attackBoxForwardOffset
            + transform.up * attackBoxHeightOffset;

        Collider[] hits = Physics.OverlapBox(
            boxCenter,
            attackBoxSize * 0.5f,
            transform.rotation,
            playerLayer
        );

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                _statManager.TakeDamage(10f);
                Debug.Log("HIT BOX !");
            }
        }
    }

    // draw attack hitbox
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 boxCenter =
            transform.position
            + transform.forward * attackBoxForwardOffset
            + transform.up * attackBoxHeightOffset;

        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
    }


    public void EndDealDamage() { }

    public void OnAttackFinished()
    {
        _animator.SetTrigger(_animIDMove);
    }
}
