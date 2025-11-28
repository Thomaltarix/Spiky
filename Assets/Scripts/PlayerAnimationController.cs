using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator _anim;

    public float CurrentAttackDuration { get; private set; } = 0.6f;

    private int SpeedID;
    private int GroundedID;
    private int JumpID;
    private int FreeFallID;
    private int AttackID;
    private int MoveID;
    private int DrawID;
    private int SheathID;
    private int AttackSpeedID;

    private void Awake()
    {
        _anim = GetComponent<Animator>();

        SpeedID = Animator.StringToHash("Speed");
        GroundedID = Animator.StringToHash("Grounded");
        JumpID = Animator.StringToHash("Jump");
        FreeFallID = Animator.StringToHash("FreeFall");
        AttackID = Animator.StringToHash("attack");
        AttackSpeedID = Animator.StringToHash("AttackSpeed");
        MoveID = Animator.StringToHash("move");
        DrawID = Animator.StringToHash("drawWeapon");
        SheathID = Animator.StringToHash("sheathWeapon");
    }

    public void SetSpeed(float v) {
        _anim.SetFloat(SpeedID, v);
    }
    public void SetGrounded(bool v) => _anim.SetBool(GroundedID, v);

    public void TriggerJump()  => _anim.SetBool(JumpID, true);

    public void TriggerFreeFall() 
    {
        _anim.SetBool(JumpID, false);
        _anim.SetBool(FreeFallID, true);
    }

    //todo:change this bugged thing
    public void ToggleCombat(bool on)
    {
        _anim.SetTrigger(on ? DrawID : SheathID);
    }

    public void TriggerAttack(float attackSpeed)
    {
        _anim.SetTrigger(AttackID);
        _anim.SetFloat(AttackSpeedID, attackSpeed);

        // optional: find current clip length
        var clip = _anim.GetCurrentAnimatorClipInfo(0);
        if (clip.Length > 0)
            CurrentAttackDuration = clip[0].clip.length;
    }

    public void ReturnToMove()
    {
        Debug.Log("reutnr move");
        _anim.SetTrigger(MoveID);
    }

    public void OnLand()
    {
        SetGrounded(true);
        _anim.SetBool(FreeFallID, false);
        _anim.SetBool(JumpID, false);
    }

    public void OnAttackFinished()
    {
        //do nothing
    }

    public void OnFootstep()
    {
        //do nothing
    }
}
