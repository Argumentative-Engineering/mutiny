using System.Collections;
using System.Threading;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _timeTillExplode;
    [SerializeField] float _explosionForce;
    [SerializeField] float _damageMult = 2;
    [SerializeField] float _explosionRadius;
    [SerializeField] float _stunDuration = 1;

    [Header("Refernces")]
    [SerializeField] GameObject _bombExplodeVFX;
    [SerializeField] TextMeshPro _timerText;
    [SerializeField] Renderer _graphic;

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

                    float dmg = Mathf.Max(_explosionForce * _damageMult / (1 + dist), 0);

                    health.Damage(dmg, dir);
                    health.Stun(_stunDuration);
                }
            }
            Destroy(Instantiate(_bombExplodeVFX, transform.position, Quaternion.identity), 3);
            Destroy(gameObject);
        }
        else
        {
            _timer -= Time.deltaTime;
        }

        _timerText.text = ((int)_timer).ToString();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
