using System.Collections.Generic;
using UnityEngine;

public class ExplosionVFXPool : MonoBehaviour
{
    public static ExplosionVFXPool Instance;

    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> explosionPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject vfx = Instantiate(explosionPrefab);
            vfx.SetActive(false);
            explosionPool.Enqueue(vfx);
        }
    }

    public void SpawnExplosion(Vector3 position, float duration)
    {
        GameObject vfx;

        if (explosionPool.Count > 0)
        {
            vfx = explosionPool.Dequeue();
        }
        else
        {
            vfx = Instantiate(explosionPrefab);
        }

        vfx.transform.position = position;
        vfx.SetActive(true);

        StartCoroutine(ReturnAfterDelay(vfx, duration));
    }

    private System.Collections.IEnumerator ReturnAfterDelay(GameObject vfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        vfx.SetActive(false);
        explosionPool.Enqueue(vfx);
    }
}
