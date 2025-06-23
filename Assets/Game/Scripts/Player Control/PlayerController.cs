using System;
using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
using Unity.VisualScripting;
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
        [SerializeField] private float _initialSpeed = 15f;
        [SerializeField] private float _boostSpeedMultiplier = 1.5f;
        [SerializeField] private float _sideSpeed = 20f;
        [SerializeField] private float _jumpForce = 10f;

        [Header("Grounded Configs")]
        [SerializeField] private float _groundCheckDistance = 0.1f;

        [Header("Move Configs")]
        [SerializeField] private float _leftPositionX = -5;
        [SerializeField] private float _rightPositionX = 5;
        [SerializeField] private float _middlePositionX = 0;

        [Header("Other Configs")]
        [SerializeField] private int _defaultFuelRate = 1;
        [SerializeField] private int _burnFuelRate = 10;

        [Header("Sound Clips")]
        [SerializeField] private float _volume = 0.5f;
        [SerializeField] private AudioClip[] _invalidClips;
        [SerializeField] private AudioClip _obstacleClip;
        [SerializeField] private AudioClip _fallClip;
        [SerializeField] private AudioClip[] _boostClips;
        [SerializeField] private AudioClip[] _stickyClips;
        [SerializeField] private AudioClip _burningClip;
        [SerializeField] private AudioClip _suppliesClip;


        private Rigidbody _rigidBody;
        private LayerMask _groundLayer;
        private Collider _playerCollider;
        private Renderer _playerRenderer;

        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _pauseAction;

        // Cheats 
        private InputAction _invincibleAction;
        private InputAction _slowDownAction;
        private InputAction _refillFuelAction;

        private bool _isInvincible = false;
        private Coroutine _rainbowCoroutine;

        private bool _isMoving = false;
        private Vector3 _lastPosition;
        private float _speed;
        private PlayerPosition _playerPosition = PlayerPosition.Middle;

        [Header("SOAP Variables")]
        [SerializeField] private BoolVariable _isGameOver;
        [SerializeField] private BoolVariable _isPaused;
        [SerializeField] private StringVariable _playerSpeed;
        [SerializeField] private IntVariable _fuelDecreaseRate;


        #endregion

        #region Properties

        #endregion

        #region Events/Delegates
        [Header("SOAP Events")]
        [SerializeField] private ScriptableEventNoParam _onPlayerDeath;
        [SerializeField] private ScriptableEventNoParam _onGamePaused;
        [SerializeField] private ScriptableEventNoParam _onEnterSupplies;
        [SerializeField] private ScriptableEventNoParam _onPlayerSpeedChanged;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Get the components
            _rigidBody = GetComponent<Rigidbody>();
            _playerCollider = GetComponent<Collider>();
            _playerInput = GetComponent<PlayerInput>();
            _playerRenderer = GetComponent<Renderer>();

            // Get the input actions
            _moveAction = _playerInput.actions["Move"];
            _jumpAction = _playerInput.actions["Jump"];
            _pauseAction = _playerInput.actions["Pause"];
            _invincibleAction = _playerInput.actions["Invincibility"];
            _slowDownAction = _playerInput.actions["Slow"];
            _refillFuelAction = _playerInput.actions["Refill"];

            // Enable the input actions
            _moveAction.Enable();
            _jumpAction.Enable();
            _pauseAction.Enable();
            _invincibleAction.Enable();
        }

        private void OnEnable()
        {
            _moveAction.performed += OnMove;
            _jumpAction.performed += OnJump;
            _pauseAction.performed += OnPause;
            _isPaused.OnValueChanged += OnGamePaused;
            _invincibleAction.performed += OnInvincible;
        }

        private void OnDisable()
        {
            _moveAction.performed -= OnMove;
            _jumpAction.performed -= OnJump;
            _pauseAction.performed -= OnPause;
            _isPaused.OnValueChanged -= OnGamePaused;
            _invincibleAction.performed -= OnInvincible;
        }

        private void Start()
        {
            // Set the ground layer
            _groundLayer = LayerMask.GetMask("Ground");
            _playerSpeed.Value = "Normal";
            _isInvincible = false;
            _speed = _initialSpeed;
            _lastPosition = transform.position;
        }

        private void Update()
        {
            if (_isPaused || _isGameOver)
                return;
            // Move the player forward
            transform.position += Vector3.forward * _speed * Time.deltaTime;

            // Increase score based on distance travelled
            float distanceTravelled = transform.position.z - _lastPosition.z;
            GameManager.Instance.IncreaseScore(distanceTravelled / 10);
            _lastPosition = transform.position;

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
        }

        private void OnCollisionEnter(Collision other)
        {
            switch (other.gameObject.tag)
            {
                case "Obstacle":
                    AudioManager.Instance.PlaySoundFXClip(_obstacleClip, other.gameObject.transform, _volume * 0.75f);
                    // if (!_isInvincible)
                    _onPlayerDeath.Raise();
                    break;
                case "Fall":
                    AudioManager.Instance.PlaySoundFXClip(_fallClip, other.gameObject.transform, _volume * 0.75f);
                    _onPlayerDeath.Raise();
                    break;
                case "Boost":
                    AudioManager.Instance.PlaySoundFXClip(_boostClips[UnityEngine.Random.Range(0, _boostClips.Length)], other.gameObject.transform, _volume * 2f);
                    _speed *= _boostSpeedMultiplier;
                    if (_playerSpeed.Value.Equals("Normal"))
                    {
                        _onPlayerSpeedChanged.Raise();
                        _playerSpeed.Value = "High";
                        _sideSpeed *= 2;
                    }
                    break;
                case "Sticky":
                    AudioManager.Instance.PlaySoundFXClip(_stickyClips[UnityEngine.Random.Range(0, _stickyClips.Length)], other.gameObject.transform, _volume);
                    if (_playerSpeed.Value.Equals("High"))
                    {
                        _onPlayerSpeedChanged.Raise();
                        _playerSpeed.Value = "Normal";
                        _sideSpeed /= 2;
                    }
                    _speed = _initialSpeed;
                    break;
                case "Burning":
                    AudioManager.Instance.PlaySoundFXClip(_burningClip, other.gameObject.transform, _volume * 0.75f);
                    _fuelDecreaseRate.Value = _burnFuelRate;
                    break;
                case "Supplies":
                    AudioManager.Instance.PlaySoundFXClip(_suppliesClip, other.gameObject.transform, _volume * 0.75f);
                    _onEnterSupplies.Raise();
                    break;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Burning"))
            {
                _fuelDecreaseRate.Value = _defaultFuelRate;
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
            if (_isPaused || _isGameOver)
                return;
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
            if (_isPaused || _isGameOver)
                return;
            float jumpValue = context.ReadValue<float>();
            if (jumpValue > 0) // Jump
                Jump();
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            // Pause the game
            _onGamePaused.Raise();
        }

        private void OnGamePaused(bool isGamePaused)
        {
            if (isGamePaused)
            {
                _moveAction.Disable();
                _jumpAction.Disable();
            }
            else
            {
                _moveAction.Enable();
                _jumpAction.Enable();
            }
        }

        private void OnInvincible(InputAction.CallbackContext context)
        {
            if (_isPaused || _isGameOver)
                return;

            if (!_isInvincible)
                StartInvincibility();
            else
                StopInvincibility();
        }

        private void OnSlowDown(InputAction.CallbackContext context)
        {
            if (_isPaused || _isGameOver)
                return;
            // if (_playerSpeed.Value.Equals("High"))
            // {
            //     _onPlayerSpeedChanged.Raise();
            //     _playerSpeed.Value = "Normal";
            //     _speed /= 2;
            // }
            // _speed /= 2;
        }

        private void OnRefillFuel(InputAction.CallbackContext context)
        {
            if (_isPaused || _isGameOver)
                return;
            // _fuelDecreaseRate.Value = _defaultFuelRate;
        }

        private void MoveLeft()
        {
            // Check if the player is already at the left position
            if (_playerPosition == PlayerPosition.Left)
            {
                AudioManager.Instance.PlaySoundFXClip(_invalidClips[UnityEngine.Random.Range(0, _invalidClips.Length)], transform, _volume);
                return;
            }

            if (_playerPosition == PlayerPosition.Middle)
                _playerPosition = PlayerPosition.Left;
            else
                _playerPosition = PlayerPosition.Middle;

            _isMoving = true;
        }

        private void MoveRight()
        {
            if (_playerPosition == PlayerPosition.Right)
            {
                AudioManager.Instance.PlaySoundFXClip(_invalidClips[UnityEngine.Random.Range(0, _invalidClips.Length)], transform, _volume);
                return;
            }

            if (_playerPosition == PlayerPosition.Middle)
                _playerPosition = PlayerPosition.Right;
            else
                _playerPosition = PlayerPosition.Middle;

            _isMoving = true;
        }

        private void Jump()
        {
            if (!IsGrounded())
            {
                AudioManager.Instance.PlaySoundFXClip(_invalidClips[UnityEngine.Random.Range(0, _invalidClips.Length)], transform, _volume);
                return;
            }

            // Add force to the player rigidbody
            _rigidBody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }

        private bool IsGrounded()
        {
            // using a raycast to check if the player is grounded
            return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, _groundLayer);
        }


        private void StartInvincibility()
        {
            _isInvincible = true;
            if (_rainbowCoroutine != null)
            {
                StopCoroutine(_rainbowCoroutine);
            }
            _rainbowCoroutine = StartCoroutine(RainbowEffect());
        }

        private void StopInvincibility()
        {
            _isInvincible = false;
            if (_rainbowCoroutine != null)
            {
                StopCoroutine(_rainbowCoroutine);
                _rainbowCoroutine = null;
            }
            _playerRenderer.material.color = Color.white; // Reset to original color
        }
        private IEnumerator RainbowEffect()
        {
            while (_isInvincible)
            {
                _playerRenderer.material.color = Color.Lerp(Color.red, Color.yellow, Mathf.PingPong(Time.time, 1));
                yield return new WaitForSeconds(0.1f);
                _playerRenderer.material.color = Color.Lerp(Color.yellow, Color.green, Mathf.PingPong(Time.time, 1));
                yield return new WaitForSeconds(0.1f);
                _playerRenderer.material.color = Color.Lerp(Color.green, Color.cyan, Mathf.PingPong(Time.time, 1));
                yield return new WaitForSeconds(0.1f);
                _playerRenderer.material.color = Color.Lerp(Color.cyan, Color.blue, Mathf.PingPong(Time.time, 1));
                yield return new WaitForSeconds(0.1f);
                _playerRenderer.material.color = Color.Lerp(Color.blue, Color.magenta, Mathf.PingPong(Time.time, 1));
                yield return new WaitForSeconds(0.1f);
                _playerRenderer.material.color = Color.Lerp(Color.magenta, Color.red, Mathf.PingPong(Time.time, 1));
                yield return new WaitForSeconds(0.1f);
            }
        }
        #endregion
    }
}