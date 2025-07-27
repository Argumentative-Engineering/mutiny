using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

    private void Update()
    {
        if(transform.position.y < -10f)
        {
            Die(Random.insideUnitCircle.normalized);
        }
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
        GameManager.Instance.PlayerDied(gameObject);

        GetComponent<PlayerController>().enabled = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up * 2, Health.ToString());
    }
#endif 
}
