using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombThrow : MonoBehaviour
{
    public bool IsAiming { get; set; }
    public Vector3 InputVector { get; set; }
    public bool IsMouse { get; set; }

    public float CooldownLeft { get; set; }

    [Header("Settings")]
    [SerializeField] float _bombThrowForce = 10;
    [SerializeField] float _cooldown = 1;

    [Header("References")]
    [SerializeField] GameObject _arrow;
    [SerializeField] Transform _bombThrowPoint;
    [SerializeField] GameObject _bombPrefab;
    [SerializeField] Rigidbody _rb;

    Vector3 _aimVector;

    void Start()
    {
        _arrow.SetActive(false);
    }

    public void StartAiming()
    {
        IsAiming = true;
        _arrow.SetActive(true);
    }

    public void EndAiming()
    {
        IsAiming = false;
        _arrow.SetActive(false);

        if (CooldownLeft <= 0)
        {
            // throw bomb and push me back
            var bomb = Instantiate(_bombPrefab, _bombThrowPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            bomb.AddForce(_aimVector * _bombThrowForce, ForceMode.Force);
            _rb.AddForce(-_aimVector * (_bombThrowForce * 0.01f), ForceMode.Impulse);
            CooldownLeft = _cooldown;
        }
    }

    void Update()
    {
        Vector3 dir = IsMouse ? GetMousePosition() - transform.position : InputVector;
        dir.z = 0;

        if (dir.magnitude > (IsMouse ? 0 : 0.2f))
        {
            _aimVector = dir.normalized;
            float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _arrow.transform.rotation = Quaternion.Euler(0, 0, rot);
        }

        if (CooldownLeft > 0)
            CooldownLeft -= Time.deltaTime;
    }

    Vector3 GetMousePosition()
    {
        var mousePos = InputVector;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
