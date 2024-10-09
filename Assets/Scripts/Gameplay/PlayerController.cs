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
    float _lastOnGroundTime = 0.5f;

    [Header("References")]
    [SerializeField] Transform _groundCheckPoint;

    GameInputSettings _input;
    Vector2 _inputVec;
    Rigidbody _rb;

    bool _isJumping = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _input = new GameInputSettings();
        _input.Player.Move.performed += OnMovePerformed;
        _input.Player.Move.canceled += OnMoveCancelled;
        _input.Player.Jump.performed += OnJumpPerformed;
    }
    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
        _input.Player.Move.performed -= OnMovePerformed;
        _input.Player.Move.canceled -= OnMovePerformed;
        _input.Player.Jump.performed -= OnJumpPerformed;
        _input.Dispose();
    }

    private void OnMovePerformed(InputAction.CallbackContext context) =>
        _inputVec = context.ReadValue<Vector2>();
    private void OnMoveCancelled(InputAction.CallbackContext ctx) => _inputVec = Vector2.zero;

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (_lastOnGroundTime > 0 && !_isJumping)
        {
            _isJumping = true;
            _lastOnGroundTime = 0;
            float force = _jumpForce;
            if (_rb.velocity.y < 0) force -= _rb.velocity.y;

            _rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        if (Physics.OverlapBox(_groundCheckPoint.position, _groundCheckSize, Quaternion.identity, _groundLayer).Length > 0)
        {
            _lastOnGroundTime = _coyoteTime;
        }

        if (_isJumping && _rb.velocity.y < 0)
        {
            _isJumping = false;
        }
    }

    void FixedUpdate()
    {
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
