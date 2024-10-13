using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public Color MaterialColor { get; set; }
    [SerializeField] Renderer _renderer;

    public void SetColor(int index, Color color)
    {
        transform.name = $"Player {index + 1}";
        MaterialColor = color;
        _renderer.material.color = color;
    }
}
