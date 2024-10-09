using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [field: SerializeField]
    public List<GameObject> Players { get; private set; }

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
