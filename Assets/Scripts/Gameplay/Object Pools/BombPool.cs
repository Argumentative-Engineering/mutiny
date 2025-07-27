using System.Collections.Generic;
using UnityEngine;

public class BombPool : MonoBehaviour
{
    public static BombPool Instance;

    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> bombPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject bomb = Instantiate(bombPrefab);
            bomb.SetActive(false);
            bombPool.Enqueue(bomb);
        }
    }

    public GameObject SpawnBomb(Vector3 position, Quaternion rotation)
    {
        GameObject bomb;

        if (bombPool.Count > 0)
        {
            bomb = bombPool.Dequeue();
        }
        else
        {
            bomb = Instantiate(bombPrefab);
        }

        bomb.transform.position = position;
        bomb.transform.rotation = rotation;
        bomb.SetActive(true);

        return bomb;
    }

    public void ReturnToPool(GameObject bomb)
    {
        bomb.SetActive(false);
        bombPool.Enqueue(bomb);
    }
}
