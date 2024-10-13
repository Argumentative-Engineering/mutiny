using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// (zsfer): i dont want the hassle of abstracting the input management to another class becauwe IM LAZY 
// and its not like we're gonna continue this in the future LOL
// sorry nalang lol i'll do my best in making this at least readable
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float _moveSpeed = 5;
    [SerializeField] float _accelRate, _deccelRate;
    [SerializeField] float _frictionAmount = 0.2f;
    [SerializeField] float _coyoteTime = 0.5f;
    [SerializeField] float _jumpForce = 10f;

    [Header("Collision")]
    [SerializeField] Vector3 _groundCheckSize;
    [SerializeField] LayerMask _groundLayer;
    float _coyoteTimer = 0.5f;

    [Header("References")]
    [SerializeField] Transform _groundCheckPoint;
    [SerializeField] PlayerBombThrow _thrower;
    [SerializeField] PlayerInput _inputs;
    [SerializeField] GameObject _jumpPoof;
    [SerializeField] AudioSource _audio;
    [SerializeField] AudioClip _jumpSFX, _throwSFX;

    public bool IsStunned { get; set; } = false;

    bool _isMouse;

    InputActionMap _playerInput;
    Vector2 _inputVec;
    Rigidbody _rb;

    int _jumpCount = 0;
    Vector3 _prevVel;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInput = _inputs.actions.FindActionMap("Player");
        _isMouse = _inputs.currentControlScheme == "Keyboard&Mouse";
        _thrower.IsMouse = _isMouse;

        _playerInput.FindAction("Move").performed += OnMovePerformed;
        _playerInput.FindAction("Move").canceled += OnMoveCancelled;
        _playerInput.FindAction("Jump").performed += OnJumpPerformed;

        _playerInput.FindAction("Aim").performed += OnAimPerformed;

        _playerInput.FindAction("Fire").performed += OnFirePerformed;
        _playerInput.FindAction("Fire").canceled += OnFireCancelled;
    }

    private void OnAimPerformed(InputAction.CallbackContext context)
    {
        _thrower.InputVector = context.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        _playerInput.FindAction("Move").performed -= OnMovePerformed;
        _playerInput.FindAction("Move").canceled -= OnMoveCancelled;
        _playerInput.FindAction("Jump").performed -= OnJumpPerformed;

        _playerInput.FindAction("Aim").performed -= OnAimPerformed;

        _playerInput.FindAction("Fire").performed -= OnFirePerformed;
        _playerInput.FindAction("Fire").canceled -= OnFireCancelled;
        _playerInput.Dispose();
    }

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        _thrower.StartAiming();

        _rb.useGravity = false;
        _prevVel = _rb.velocity;
        _rb.velocity = Vector3.zero;
    }

    private void OnFireCancelled(InputAction.CallbackContext context)
    {
        _thrower.EndAiming();

        _rb.useGravity = true;
        _rb.velocity = _prevVel;
        _audio.PlayOneShot(_throwSFX, 3);
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (IsStunned) return;
        _inputVec = context.ReadValue<Vector2>();
        // if (_inputVec.y > 0.7f) Jump();
    }

    private void OnMoveCancelled(InputAction.CallbackContext ctx) => _inputVec = Vector2.zero;

    private void OnJumpPerformed(InputAction.CallbackContext context) => Jump();

    void Jump()
    {
        if (!_thrower.IsAiming && _jumpCount != 1)
        {
            float force = _jumpForce;
            if (_rb.velocity.y < 0) force -= _rb.velocity.y;

            _rb.AddForce(Vector3.up * force, ForceMode.Impulse);
            _jumpCount++;

            Destroy(Instantiate(_jumpPoof, transform.position + (-Vector3.up * 0.5f), Quaternion.identity), 5);
            _audio.PlayOneShot(_jumpSFX, 1f);
        }
    }

    private void Update()
    {
        if (Physics.OverlapBox(_groundCheckPoint.position, _groundCheckSize, Quaternion.identity, _groundLayer).Length > 0)
        {
            _coyoteTimer = _coyoteTime;
            IsStunned = false;
            _jumpCount = 0;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (_thrower.IsAiming) return;

        var trgSpeed = _inputVec.x * _moveSpeed;
        trgSpeed = Mathf.Lerp(_rb.velocity.x, trgSpeed, 1);
        var speedDiff = trgSpeed - _rb.velocity.x;
        var accel = Mathf.Abs(trgSpeed) > 0.01f ? _accelRate : _deccelRate;
        var movement = speedDiff * accel;

        _rb.AddForce(movement * Vector3.right);

        if (Mathf.Abs(_inputVec.x) < 0.01f)
        {
            float friction = Mathf.Min(Mathf.Abs(_rb.velocity.x), Mathf.Abs(_frictionAmount));
            friction *= Mathf.Sign(_rb.velocity.x);
            _rb.AddForce(Vector3.right * -friction, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
}
