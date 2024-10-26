using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace InfiniteRunner
{
    public class PlayerController : MonoBehaviour
    {
        private enum PlayerPosition
        {
            Left,
            Middle,
            Right
        }

        #region Fields

        [Header("Speed Configs")]
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _sideSpeed = 20f;
        [SerializeField] private float _jumpForce = 10f;

        [Header("Grounded Configs")]
        [SerializeField] private float _groundCheckDistance = 0.1f;

        [Header("Move Configs")]
        [SerializeField] private float _leftPositionX = -5;
        [SerializeField] private float _rightPositionX = 5;
        [SerializeField] private float _middlePositionX = 0;

        private Rigidbody _rigidBody;
        private LayerMask _groundLayer;
        private Collider _playerCollider;

        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _pauseAction;

        private bool _isMoving = false;
        private PlayerPosition _playerPosition = PlayerPosition.Middle;
        private bool _isGameOver = false;
        private bool _isPaused = false;

        #endregion

        #region Properties

        #endregion

        #region Events/Delegates
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Get the components
            _rigidBody = GetComponent<Rigidbody>();
            _playerCollider = GetComponent<Collider>();
            _playerInput = GetComponent<PlayerInput>();

            // Get the input actions
            _moveAction = _playerInput.actions["Move"];
            _jumpAction = _playerInput.actions["Jump"];
            _pauseAction = _playerInput.actions["Pause"];

            _moveAction.Enable();
            _jumpAction.Enable();
            _pauseAction.Enable();

            _isGameOver = false;
            _isPaused = false;
        }

        private void OnEnable()
        {
            _moveAction.performed += OnMove;
            _jumpAction.performed += OnJump;
            _pauseAction.performed += OnPause;
        }

        private void OnDisable()
        {
            _moveAction.performed -= OnMove;
            _jumpAction.performed -= OnJump;
            _pauseAction.performed -= OnPause;
        }

        private void Start()
        {
            // Set the ground layer
            _groundLayer = LayerMask.GetMask("Ground");
        }

        private void Update()
        {
            // Move the player forward
            transform.position += Vector3.forward * _speed * Time.deltaTime;

            // Smoothly move the player to the target position
            if (_isMoving)
            {
                Vector3 targetPosition = Vector3.zero;
                switch (_playerPosition)
                {
                    case PlayerPosition.Left:
                        targetPosition = new Vector3(_leftPositionX, transform.position.y, transform.position.z);
                        break;
                    case PlayerPosition.Middle:
                        targetPosition = new Vector3(_middlePositionX, transform.position.y, transform.position.z);
                        break;
                    case PlayerPosition.Right:
                        targetPosition = new Vector3(_rightPositionX, transform.position.y, transform.position.z);
                        break;
                }

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _sideSpeed * Time.deltaTime);

                if (transform.position == targetPosition)
                    _isMoving = false;
            }

            // Check if player fell off the platform
            if (transform.position.y < -5)
            {
                OnGameOver();
            }
        }
        private void LateUpdate()
        {
            if (Time.timeScale == 0)
            {
                _jumpAction.Disable();
                _moveAction.Disable();
            }
            else
            {
                _jumpAction.Enable();
                _moveAction.Enable();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Obstacle"))
            {
                OnGameOver();
            }
        }

        private void OnDrawGizmos()
        {
            // Draw point at player position
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.1f);

            // Draw raycast for ground check
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * _groundCheckDistance);
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        private void OnMove(InputAction.CallbackContext context)
        {
            // Get the input value
            float moveValue = context.ReadValue<float>();
            if (moveValue < 0) // Move Left
            {
                MoveLeft();
            }
            else if (moveValue > 0) // Move Right
            {
                MoveRight();
            }
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            float jumpValue = context.ReadValue<float>();
            if (jumpValue > 0) // Jump
                Jump();
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            // Pause the game
            GameManager.Instance.PauseGame();
        }

        private void OnGameOver()
        {
            if (_isGameOver)
                return;
            GameManager.Instance.GameOver();
            _isGameOver = true;
        }

        private void MoveLeft()
        {
            // Check if the player is already at the left position
            if (_playerPosition == PlayerPosition.Left)
                return;

            if (_playerPosition == PlayerPosition.Middle)
                _playerPosition = PlayerPosition.Left;
            else
                _playerPosition = PlayerPosition.Middle;

            _isMoving = true;
        }

        private void MoveRight()
        {
            if (_playerPosition == PlayerPosition.Right)
                return;

            if (_playerPosition == PlayerPosition.Middle)
                _playerPosition = PlayerPosition.Right;
            else
                _playerPosition = PlayerPosition.Middle;

            _isMoving = true;
        }

        private void Jump()
        {
            if (!IsGrounded())
                return;

            // Add force to the player rigidbody
            _rigidBody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }

        private bool IsGrounded()
        {
            // using a raycast to check if the player is grounded
            return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, _groundLayer);
        }
        #endregion
    }
}