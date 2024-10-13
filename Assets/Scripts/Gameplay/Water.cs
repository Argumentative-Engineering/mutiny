using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Water : MonoBehaviour
{
    void Start()
    {
        GetComponent<Renderer>().material.DOOffset(new Vector2(0.6f, 0), 10).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }
}
