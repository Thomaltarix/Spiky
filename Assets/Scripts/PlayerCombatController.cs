using StarterAssets;
using System.Collections;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    private PlayerInputHandler _input;
    private PlayerAnimationController _anim;
    private PlayerStatManager _statManager;

    private float _attackCooldown = 0.8f;

    private float _inCombatCooldown = 0.75f;
    private float _inCombatTimeout = 0f;

    private bool _inCombat = false;
    private bool _attacking = false;
    private bool _queued = false;

    private bool _haveAttacked = false;

    private void Awake()
    {
        _statManager = GetComponent<PlayerStatManager>();
        _input = GetComponent<PlayerInputHandler>();
        _anim = GetComponent<PlayerAnimationController>();
        _inCombatTimeout = _inCombatCooldown;
    }

    private void Update()
    {


        if (_input.ToggleCombatPressed && _inCombatTimeout <= 0f)
        {
            _inCombat = !_inCombat;
            _inCombatTimeout = _inCombatCooldown;
            Debug.Log(_inCombat);
            _anim.ToggleCombat(_inCombat);
        }

        if (_inCombatTimeout > 0)
        {
            _inCombatTimeout -= Time.deltaTime;
        }

        if (!_inCombat) return;

        if (_input.AttackPressed)
        {
            if (!_attacking)
                StartCoroutine(AttackRoutine());
            else
                _queued = true;
        }
        if (_haveAttacked && !_input.AttackPressed)
        {
            if (!_attacking)
            {
                _haveAttacked = false;
                _anim.ReturnToMove();
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        _attacking = true;
        _haveAttacked = true;
        _anim.TriggerAttack(_statManager.attackSpeed.Value);

        yield return new WaitForSeconds(_attackCooldown / _statManager.attackSpeed.Value);

        if (_queued)
        {
            _queued = false;
            Debug.Log("queue");
            StartCoroutine(AttackRoutine());
            yield break;
        }
        Debug.Log("finish the attack");
        _attacking = false;
    }

    // keep empty (used for animation)
    public void DealDamageEvent() { }
}
