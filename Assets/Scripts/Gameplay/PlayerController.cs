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
    [SerializeField] float _velocityLimit = 1000f;
    [SerializeField] float _gravityMultiplier = 0.2f;

    [Header("Collision")]
    [SerializeField] Vector3 _groundCheckSize;
    [SerializeField] LayerMask _groundLayer;
    float _coyoteTimer = 0.5f;

    [Header("References")]
    [SerializeField] Transform _groundCheckPoint;
    [SerializeField] PlayerBombThrow _thrower;
    [SerializeField] PlayerInput _inputs;
    [SerializeField] PlayerHealth _health;
    [SerializeField] GameObject _jumpPoof;
    [SerializeField] AudioSource _audio;
    [SerializeField] AudioClip _jumpSFX, _throwSFX;

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
        if (_thrower.StartAiming() && !_health.IsStunned)
        {
            _prevVel = _rb.linearVelocity;
            _rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnFireCancelled(InputAction.CallbackContext context)
    {
        EndHover();
    }

    public void EndHover()
    {
        if (_thrower.EndAiming())
        {
            _audio.PlayOneShot(_throwSFX, 3);
            _rb.linearVelocity = _prevVel;
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (_health.IsStunned) return;
        _inputVec = context.ReadValue<Vector2>();
        if (_isMouse && _inputVec.y > 0) Jump();

        _inputVec.y = 0;
    }

    private void OnMoveCancelled(InputAction.CallbackContext ctx) => _inputVec = Vector2.zero;

    private void OnJumpPerformed(InputAction.CallbackContext context) => Jump();

    void Jump()
    {
        if (!_thrower.IsAiming && _jumpCount != 1)
        {
            float force = _jumpForce;
            if (_rb.linearVelocity.y < 0) force -= _rb.linearVelocity.y;

            _rb.AddForce(Vector3.up * force, ForceMode.Impulse);
            _jumpCount++;

            Destroy(Instantiate(_jumpPoof, transform.position, Quaternion.identity), 5);
            _audio.PlayOneShot(_jumpSFX, 2f);
        }
    }

    private void Update()
    {
        if (Physics.OverlapBox(_groundCheckPoint.position, _groundCheckSize, Quaternion.identity, _groundLayer).Length > 0)
        {
            _coyoteTimer = _coyoteTime;
            _health.IsStunned = false;
            _jumpCount = 0;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (_thrower.IsAiming)
        {
            if (_rb.linearVelocity.y < 0) _rb.AddForce(Vector2.up * Physics.gravity * (_gravityMultiplier - 1) * _rb.mass, ForceMode.Force);
            return;
        }

        var trgSpeed = _inputVec.x * _moveSpeed;
        trgSpeed = Mathf.Lerp(_rb.linearVelocity.x, trgSpeed, 1);
        var speedDiff = trgSpeed - _rb.linearVelocity.x;
        var accel = Mathf.Abs(trgSpeed) > 0.01f ? _accelRate : _deccelRate;
        var movement = speedDiff * accel;

        _rb.AddForce(movement * Vector3.right);

        if (_rb.linearVelocity.magnitude >= _velocityLimit)
        {
            _rb.linearVelocity *= _velocityLimit / _rb.linearVelocity.magnitude;
        }

        if (Mathf.Abs(_inputVec.x) < 0.01f)
        {
            float friction = Mathf.Min(Mathf.Abs(_rb.linearVelocity.x), Mathf.Abs(_frictionAmount));
            friction *= Mathf.Sign(_rb.linearVelocity.x);
            _rb.AddForce(Vector3.right * -friction, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
}
