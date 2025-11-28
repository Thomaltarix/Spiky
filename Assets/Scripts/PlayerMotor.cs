using UnityEngine;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : MonoBehaviour
    {
        public float MoveSpeed = 2f;
        public float SprintSpeed = 5f;
        public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10f;

        public float JumpHeight = 1.2f;
        public float Gravity = -15f;
        public float FallTimeout = 0.15f;
        public float JumpTimeout = 0.5f;

        [Header("Stamina Settings")]
        public float staminaMaxMultiplier = 1f;   // multiplier basé sur la stat
        public float staminaDrainRate = 1f;       // consommation * stat
        public float staminaRegenRate = 1f;       // regen * stat
        public float regenDelay = 0.8f;           // délai avant la régénération

        private float stamina;
        private float regenTimer;
        private bool staminaLocked = false;

        public bool Grounded { get; private set; }

        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        private CharacterController _controller;
        private PlayerInputHandler _input;
        private PlayerAnimationController _anim;
        private PlayerStatManager _statManager;

        private float _speed;
        private float _verticalVelocity;
        private float _targetRotation;
        private float _rotationVelocity;

        //little timeout before playing animations
        private float _fallTimeoutDelta;
        private float _jumpTimeoutDelta;

        private Camera _cam;

        private void Awake()
        {
            _statManager = GetComponent<PlayerStatManager>();
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputHandler>();
            _anim = GetComponent<PlayerAnimationController>();
            _cam = Camera.main;
        }

        private void Start()
        {
            stamina = _statManager.stamina.Value * staminaMaxMultiplier;
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            MoveSpeed = _statManager.movementSpeed.Value;
            SprintSpeed = _statManager.sprintSpeed.Value;

            GroundCheck();
            ApplyGravity();
            Jump();
            Move();
        }

        private void GroundCheck()
        {
            Vector3 pos = transform.position + Vector3.up * GroundedOffset;
            Grounded = Physics.CheckSphere(pos, GroundedRadius, GroundLayers);

            _anim.SetGrounded(Grounded);
        }

        private void HandleStamina(bool isSprinting)
        {
            float stat = _statManager.stamina.Value;
            float maxStamina = stat * staminaMaxMultiplier;

            if (stamina <= 0f)
            {
                staminaLocked = true;
                stamina = 0f;
            }

            if (!isSprinting)
                staminaLocked = false;

            if (isSprinting && !staminaLocked)
            {
                stamina -= staminaDrainRate * stat * Time.deltaTime;
                regenTimer = regenDelay;
            }
            else
            {
                if (regenTimer > 0)
                    regenTimer -= Time.deltaTime;
                else
                    stamina += staminaRegenRate * stat * Time.deltaTime;
            }

            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }

        private void Move()
        {
            HandleStamina(_input.Sprint);
            bool canSprint = _input.Sprint && stamina > 0f && !staminaLocked;
            float targetSpeed = canSprint ? SprintSpeed : MoveSpeed;


            if (_input.Move == Vector2.zero) targetSpeed = 0;

            float currentHorizontalSpeed =
                new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);

            Vector3 inputDir = new Vector3(_input.Move.x, 0, _input.Move.y).normalized;

            if (_input.Move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + _cam.transform.eulerAngles.y;

                float rotation = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    _targetRotation,
                    ref _rotationVelocity,
                    RotationSmoothTime
                 );

                transform.rotation = Quaternion.Euler(0, rotation, 0);
            }

            Vector3 move = Quaternion.Euler(0, _targetRotation, 0) * Vector3.forward;

            _controller.Move(move.normalized * (_speed * Time.deltaTime)
                             + Vector3.up * (_verticalVelocity * Time.deltaTime));

            _anim.SetSpeed(_speed);
        }

        private void Jump()
        {
            if (Grounded)
            {
                if (_input.Jump && _jumpTimeoutDelta <= 0)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    _anim.TriggerJump();
                }

                if (_jumpTimeoutDelta > 0) _jumpTimeoutDelta -= Time.deltaTime;
                _fallTimeoutDelta = FallTimeout;
            }
            else //player is jumping
            {
                _jumpTimeoutDelta = JumpTimeout;
                if (_verticalVelocity < 0f)
                {
                    if (_fallTimeoutDelta > 0)
                        _fallTimeoutDelta -= Time.deltaTime;
                    else
                        _anim.TriggerFreeFall();
                }
            }
            _input.ResetJump();
        }

        private void ApplyGravity()
        {
            if (Grounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
                return;
            }
            if (_verticalVelocity < 53f)
                _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
}
