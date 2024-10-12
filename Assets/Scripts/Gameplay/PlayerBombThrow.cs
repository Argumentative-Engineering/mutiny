using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombThrow : MonoBehaviour
{
    public bool IsAiming { get; set; }
    public Vector3 InputVector { get; set; }
    public bool IsMouse { get; set; }
    [Header("Settings")]
    [SerializeField] float _bombThrowForce = 10;

    [Header("References")]
    [SerializeField] GameObject _arrow;
    [SerializeField] Transform _bombThrowPoint;
    [SerializeField] GameObject _bombPrefab;

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

        // throw bomb
        var bomb = Instantiate(_bombPrefab, _bombThrowPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        bomb.AddForce(_aimVector * _bombThrowForce, ForceMode.Force);
    }

    void Update()
    {
        var inputVec = IsMouse ? GetMousePosition() : InputVector;
        Vector3 dir = inputVec - transform.position;
        dir.z = 0;

        _aimVector = dir.normalized;

        float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _arrow.transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    Vector3 GetMousePosition()
    {
        var mousePos = InputVector;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
