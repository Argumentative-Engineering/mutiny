using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public Color MaterialColor { get; set; }
    [SerializeField] Image _playerBadge;
    [SerializeField] TextMeshProUGUI _playerNameText;

    public void SetColor(int index, Color color)
    {
        transform.name = $"Player {index + 1}";
        MaterialColor = color;
        _playerBadge.color = color;
        _playerNameText.text = $"P{index + 1}";
    }
}
