using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;

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

                if (col.TryGetComponent(out PlayerHealth health))
                {
                    var dist = Vector3.Distance(col.transform.position, transform.position);
                    var dir = col.transform.position - transform.position;
                    float dmg = Mathf.Max(_explosionForce / (1 + dist), 0);

                    health.Damage(dmg, dir);
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
