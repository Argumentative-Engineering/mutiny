using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] PlayerBombThrow _thrower;
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] Animator _anim;

    void Update()
    {
        _anim.SetFloat("Horizontal", Mathf.Abs(_rb.linearVelocity.x));
        _anim.SetBool("IsJumping", Mathf.Abs(_rb.linearVelocity.normalized.y) > 0.1f);
        _anim.SetBool("IsAiming", _thrower.IsAiming);

        var vec = _thrower.IsAiming ? _thrower.AimVector : _rb.linearVelocity;
        if (vec.magnitude > 0.5f)
        {
            _renderer.flipX = vec.x < -0.2f;
        }
    }
}
