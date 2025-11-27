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

        public bool Grounded { get; private set; }

        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        private CharacterController _controller;
        private PlayerInputHandler _input;
        private PlayerAnimationController _anim;

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
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputHandler>();
            _anim = GetComponent<PlayerAnimationController>();
            _cam = Camera.main;
        }

        private void Start()
        {
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
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

        private void Move()
        {
            float targetSpeed = _input.Sprint ? SprintSpeed : MoveSpeed;
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
            if (_verticalVelocity < 53f)
                _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
}
