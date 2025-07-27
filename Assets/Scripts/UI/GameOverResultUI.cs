using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverResultUI : MonoBehaviour
{
    Color[] _rankColors = {
        new(230, 255, 0),
        new(171, 171, 171),
        new(216, 118, 56),
        new(128, 128, 128)
    };

    [SerializeField] Image _panel, _rankPanel;
    [SerializeField] TextMeshProUGUI _playerName, _rank;
    public PlayerInfo Player { get; set; }

    public void ShowResult(int rank)
    {
        _playerName.text = Player.transform.name;
        _panel.color = Player.MaterialColor;
        _rank.text = rank.ToString();
        _rankPanel.color = _rankColors[Mathf.Clamp(rank - 1,0,3)];

        _panel.DOFade(1, 0.5f).From(0);
        _playerName.DOFade(1, 0.5f).From(0);
    }
}
