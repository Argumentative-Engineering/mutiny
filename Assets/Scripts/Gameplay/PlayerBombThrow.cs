using System;
using UnityEngine;

public class PlayerBombThrow : MonoBehaviour
{
    public bool IsAiming { get; set; }
    public Vector3 InputVector { get; set; }
    public Vector3 AimVector { get; private set; }
    public bool IsMouse { get; set; }

    public float CooldownLeft { get; set; }
    public float AirTime { get; set; }

    [Header("Settings")]
    [SerializeField] float _bombThrowForce = 10;
    [SerializeField] float _cooldown = 1;
    [SerializeField] float _airTime = 2;

    [Header("References")]
    [SerializeField] GameObject _arrow;
    [SerializeField] Transform _bombThrowPoint;
    [SerializeField] GameObject _bombPrefab;
    [SerializeField] Rigidbody _rb;
    [SerializeField] PlayerController _playerController;


    void Start()
    {
        _arrow.SetActive(false);
    }

    public bool StartAiming()
    {
        if (CooldownLeft <= 0)
        {
            IsAiming = true;
            _arrow.SetActive(true);
            AirTime = _airTime;
            return true;
        }

        return false;
    }

    public bool EndAiming()
    {
        if (IsAiming)
        {
            IsAiming = false;
            _arrow.SetActive(false);

            if (CooldownLeft <= 0)
            {
                // throw bomb and push me back
                var bomb = Instantiate(_bombPrefab, _bombThrowPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
                bomb.GetComponent<Bomb>().Owner = gameObject;

                bomb.AddForce(AimVector * _bombThrowForce, ForceMode.Force);
                _rb.AddForce(-AimVector * (_bombThrowForce * 0.01f), ForceMode.Impulse);

                CooldownLeft = _cooldown;
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        Vector3 dir = IsMouse ? GetMouseVector() : InputVector;
        dir.z = 0;

        if (dir.magnitude > (IsMouse ? 0 : 0.2f))
        {
            AimVector = dir.normalized;
            float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _arrow.transform.rotation = Quaternion.Euler(0, 0, rot);
        }

        if (CooldownLeft > 0)
            CooldownLeft -= Time.deltaTime;

        if (IsAiming) {
            if (AirTime > 0)
            {
                AirTime -= Time.deltaTime;
            }
            else
            {
                _playerController.EndHover();
            }
        }
    }

    Vector3 GetMouseVector()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = transform.position.z - Camera.main.transform.position.z;

        var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        var vec = worldPos - transform.position;
        vec.z = 0;
        return vec;
    }
}
