using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float Health { get; set; } = 100;

    [Header("Settings")]
    [SerializeField] float _startingHealth = 100;

    public bool IsStunned { get; set; }

    private void Start()
    {
        Health = _startingHealth;
    }

    public void Stun(float duration)
    {
        StartCoroutine(DoStun(duration));
    }

    IEnumerator DoStun(float duration)
    {
        IsStunned = true;
        yield return new WaitForSeconds(duration);
        IsStunned = false;
    }

    public void Damage(float damage, Vector3 direction)
    {
        Health -= damage;
        Health = Mathf.Max(Health, 0);

        if (Health <= 0)
            Die(direction);
    }

    public void Die(Vector3 dir)
    {
        var rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce((dir + -Vector3.forward) * 100, ForceMode.Impulse);
        StartCoroutine(nameof(RemoveFromCam));

        GetComponent<PlayerController>().enabled = false;
    }

    IEnumerator RemoveFromCam()
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.PlayerDied(gameObject);
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up * 2, Health.ToString());
    }
}
