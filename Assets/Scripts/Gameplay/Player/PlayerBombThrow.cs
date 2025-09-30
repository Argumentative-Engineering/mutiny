using System;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] float _recoilMultiplier = 0.01f;
    [SerializeField] float _cooldown = 1;
    [SerializeField] float _airTime = 2;

    [Header("Charge Settings")]
    [SerializeField] float[] _chargeTimes = { 0f, 0.75f, 1.5f };
    [SerializeField] float[] _chargeMultipliers = { 1.0f, 1.5f, 2.0f };
    [SerializeField] float[] _chargeArrowSizeMult = { 1.0f, 1.25f, 1.5f };
    private float _chargeTimer;
    private int _currentChargeStage;

    [Header("References")]
    [SerializeField] GameObject _arrow;
    [SerializeField] Transform _arrowGraphic;
    [SerializeField] Transform _bombThrowPoint;
    [SerializeField] GameObject _bombPrefab;
    [SerializeField] Rigidbody _rb;
    [SerializeField] PlayerController _playerController;
    [SerializeField] Image _arrowFill;

    private float _arrowScaleX;
    private float _arrowDist;

    void Start()
    {
        _arrow.SetActive(false);
        _arrowScaleX = _arrowGraphic.transform.localScale.x;
        _arrowDist = _arrowGraphic.transform.localPosition.x;
    }

    public bool StartAiming()
    {
        if (CooldownLeft <= 0)
        {
            IsAiming = true;
            _arrow.SetActive(true);
            AirTime = _airTime;

            _chargeTimer = 0f;
            _currentChargeStage = 0;

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
                var bomb = BombPool.Instance.SpawnBomb(_bombThrowPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
                bomb.GetComponent<Bomb>().Owner = gameObject;

                float forceMultiplier = _chargeMultipliers[Mathf.Clamp(_currentChargeStage, 0, _chargeMultipliers.Length - 1)];
                bomb.AddForce(AimVector * _bombThrowForce * forceMultiplier, ForceMode.Force);
                _rb.AddForce(-AimVector * (_bombThrowForce * _recoilMultiplier) * forceMultiplier, ForceMode.Impulse);

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
        {
            CooldownLeft -= Time.deltaTime;
            _arrowFill.fillAmount = Mathf.Clamp(CooldownLeft / _cooldown, 0f, 1f);
        }

        if (IsAiming) {
            _chargeTimer += Time.deltaTime;

            for (int i = _currentChargeStage; i < _chargeTimes.Length; i++)
            {
                if (_chargeTimer >= _chargeTimes[i])
                {
                    _currentChargeStage = i;

                    _arrowGraphic.transform.localScale = new Vector3(_arrowScaleX * _chargeArrowSizeMult[i], _arrowGraphic.transform.localScale.y, _arrowGraphic.transform.localScale.z);
                    _arrowGraphic.transform.localPosition = new Vector3(_arrowDist * (1 + ((_chargeArrowSizeMult[i]-1)/2)), _arrowGraphic.transform.localPosition.y, _arrowGraphic.transform.localPosition.z); ;
                }
            }

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
