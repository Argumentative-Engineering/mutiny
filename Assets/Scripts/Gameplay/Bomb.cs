using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float _timeTillExplode;
    [SerializeField] float _explosionForce;
    [SerializeField] float _explosionRadius;
    float _timer;


    void Start()
    {
        _timer = _timeTillExplode;
    }

    void Update()
    {
        if (_timer <= 0)
        {
            // non-alloc my ass fuck dat
            var cols = Physics.OverlapSphere(transform.position, _explosionRadius);
            foreach (var col in cols)
            {
                if (col.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, 1, ForceMode.Impulse);
                }
            }
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
