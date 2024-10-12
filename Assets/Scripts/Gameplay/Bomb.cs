using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float _timeTillExplode;
    [SerializeField] float _explosionForce;
    [SerializeField] float _explosionRadius;
    float _timer;

    Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _timer = _timeTillExplode;
    }

    void Update()
    {
        if (_timer <= 0)
        {
            print("Explode");
            _rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
            Destroy(gameObject);
        }
        else
        {
            _timer -= Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
