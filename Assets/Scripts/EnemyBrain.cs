using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _attackCooldown = 1f;

    private float _lastAttackTime = 0f;

    private GameObject _player;
    private NavMeshAgent _agent;
    private Animator _animator;

    private int _animIDSpeed;
    private int _animIDMotionSpeed;
    private float _animationBlend;

    private const float _speedChangeRate = 10f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animIDSpeed = Animator.StringToHash("Speed");

        _animator = GetComponent<Animator>();
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;
        _agent.updateRotation = false;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            _player = p;
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


    //TODO: maybe use the damage dealer script ? (make the damage dealer script generic can be used by both player and enemies)
    public void PerformAttack() 
    {
        Health playerHealth = _player.GetComponent<Health>();

        if (playerHealth)
        {
            playerHealth.TakeDamage(10f);
        }
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

        _agent.SetDestination(_player.transform.position);
        PLayMoveAnimation(_agent.velocity.magnitude);
        RotateTowardsPlayer();

        if (_agent.remainingDistance <= _attackRange && Time.time >= _lastAttackTime + _attackCooldown) 
        {
            PerformAttack();
            _lastAttackTime = Time.time;
        }
        
    }
}
