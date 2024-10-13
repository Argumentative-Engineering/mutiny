using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public PlayerColor TeamColor { get; set; }
    public Color MaterialColor { get; set; }
    [SerializeField] Renderer _renderer;

    public void SetColor(int index, Color color)
    {
        TeamColor = (PlayerColor)index;
        MaterialColor = color;
        _renderer.material.SetColor("BaseColor", color);
    }
}
