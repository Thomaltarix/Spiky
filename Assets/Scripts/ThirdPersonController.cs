 using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private bool _inCombat = false;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDDrawWeapon;
        private int _animIDSheathWeapon;
        private int _animIDAttack;
        private bool _isAttacking = false;
        private int _animIDMove;
        private bool _queuedAttack = false;
        // parameter names (kept as strings so we can check existence before using them)
        private string _paramDrawWeapon = "drawWeapon";
        private string _paramSheathWeapon = "sheathWeapon";
        private string _paramAttack = "attack";
        private string _paramMove = "move";
        private int _combatLayerIndex = -1;

        [Header("Combat")]
        [Tooltip("Name of the animator layer used for combat animations (exact name)")]
        public string CombatLayerName = "Combat Layer";
        [Tooltip("Weight to apply to the combat layer when enabled")]
        public float CombatLayerWeight = 1f;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // cache combat layer index if animator exists (we won't change weight here)
            if (_hasAnimator && _animator != null)
            {
                _combatLayerIndex = _animator.GetLayerIndex(CombatLayerName);
            }

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            // Toggle combat mode with C key (draw/sheath weapon and enable combat layer)
            if (Input.GetKeyDown(KeyCode.C))
            {
                _inCombat = !_inCombat;
                if (_hasAnimator && _animator != null)
                {
                    if (_inCombat)
                    {
                        if (AnimatorHasParameter(_paramDrawWeapon))
                            _animator.SetTrigger(_animIDDrawWeapon);
                        else
                        {
                            var ownerName = _animator != null ? _animator.gameObject.name : gameObject.name;
                            Debug.LogWarning($"Animator parameter '{_paramDrawWeapon}' not found on '{ownerName}'");
                        }
                    }
                    else
                    {
                        if (AnimatorHasParameter(_paramSheathWeapon))
                            _animator.SetTrigger(_animIDSheathWeapon);
                        else
                        {
                            var ownerName = _animator != null ? _animator.gameObject.name : gameObject.name;
                            Debug.LogWarning($"Animator parameter '{_paramSheathWeapon}' not found on '{ownerName}'");
                        }
                    }

                    // only trigger animations; do not modify layer weights here
                }
            }

            // Handle attack input when in combat: left mouse
            if (_inCombat && _hasAnimator && _animator != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // ensure the attack parameter exists
                    if (!AnimatorHasParameter(_paramAttack))
                    {
                        var ownerName = _animator != null ? _animator.gameObject.name : gameObject.name;
                        Debug.LogWarning($"Animator parameter '{_paramAttack}' not found on '{ownerName}'");
                    }
                    else
                    {
                        if (!_isAttacking)
                        {
                            StartCoroutine(AttackCoroutine());
                        }
                        else
                        {
                            // queue a follow-up attack so player doesn't need perfect timing
                            _queuedAttack = true;
                        }
                    }
                }
            }

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDDrawWeapon = Animator.StringToHash("drawWeapon");
            _animIDSheathWeapon = Animator.StringToHash("sheathWeapon");
            _animIDAttack = Animator.StringToHash("attack");
            _animIDMove = Animator.StringToHash("move");
        }

        private bool AnimatorHasParameter(string paramName)
        {
            if (_animator == null) return false;
            foreach (var p in _animator.parameters)
            {
                if (p.name == paramName) return true;
            }
            return false;
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // If attacking, block horizontal movement input to avoid sliding or getting stuck visually.
            if (_isAttacking)
            {
                // still allow gravity and vertical motion handling; skip horizontal movement.
                _controller.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
                return;
            }

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private System.Collections.IEnumerator AttackCoroutine()
        {
            _isAttacking = true;

            // capture animator reference locally to avoid MissingReferenceExceptions
            var animator = _animator;

            // enable root motion while attacking (matches the state approach)
            if (animator != null)
            {
                animator.applyRootMotion = true;
                // fire trigger
                animator.SetTrigger(_animIDAttack);
            }
            else
            {
                Debug.LogWarning("Attempted to attack but Animator is missing/destroyed.");
            }

            // wait a frame so animator can transition into the attack state
            yield return null;

            // try to get current clip length by scanning all layers (attack may be on Combat layer)
            float clipLength = 0f;
            if (animator != null)
            {
                int layerCount = animator.layerCount;
                for (int li = 0; li < layerCount; li++)
                {
                    var clips = animator.GetCurrentAnimatorClipInfo(li);
                    if (clips != null && clips.Length > 0)
                    {
                        var stateInfo = animator.GetCurrentAnimatorStateInfo(li);
                        clipLength = clips[0].clip.length / Mathf.Max(0.0001f, stateInfo.speed);
                        break;
                    }
                }
            }

            // fallback if we couldn't determine length (or animator was null)
            if (clipLength <= 0f) clipLength = 0.6f;

            yield return new WaitForSeconds(clipLength);

            // If animator was destroyed while waiting, ensure we still clear attack state
            if (_animator == null)
            {
                _isAttacking = false;
                _queuedAttack = false;
                yield break;
            }

            // signal animator to go back to movement/combat blend (use OnAttackFinished helper)
            OnAttackFinishedInternal();
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
        }

        private void OnLand(AnimationEvent animationEvent)
        {
        }

        // Called from animation event or internally when attack finishes
        public void OnAttackFinished()
        {
            // public wrapper for animator events
            OnAttackFinishedInternal();
        }

        private void OnAttackFinishedInternal()
        {
            // if player queued another attack during the animation, immediately play it
            if (_queuedAttack)
            {
                _queuedAttack = false;
                // start next attack without disabling root motion; AttackCoroutine will set root motion
                StartCoroutine(AttackCoroutine());
                return;
            }
            if (_animator != null)
            {
                // trigger move if parameter exists
                if (AnimatorHasParameter(_paramMove))
                    _animator.SetTrigger(_animIDMove);
                else
                {
                    var ownerName = _animator != null ? _animator.gameObject.name : gameObject.name;
                    Debug.LogWarning($"Animator parameter '{_paramMove}' not found on '{ownerName}'");
                }

                // disable root motion after attack
                _animator.applyRootMotion = false;
            }

            _isAttacking = false;

            // fallback: ensure animator returns to combat blend if transition didn't occur
            ForceCombatBlendIfNeeded();
        }

        // If the animator didn't transition correctly, force the combat blend state on the combat layer
        // This is a fallback for setups where the transition conditions are not set or the state machine is different.
        private void ForceCombatBlendIfNeeded()
        {
            if (_animator == null) return;
            if (_combatLayerIndex < 0) return;

            // get current state on combat layer
            var info = _animator.GetCurrentAnimatorStateInfo(_combatLayerIndex);
            // if we're still in an attack-like state (heuristic: state's length > 0 and not the blend tree), force the blend
            // Here we attempt to play a state named "Combat Blend Tree" as a safe default — adjust if your state has another name.
            string targetStateName = "Combat Blend Tree";
            if (!info.IsName(targetStateName))
            {
                Debug.Log("Forcing animator to play '" + targetStateName + "' on layer " + _combatLayerIndex);
                _animator.Play(targetStateName, _combatLayerIndex, 0f);
            }
        }
    }
}
