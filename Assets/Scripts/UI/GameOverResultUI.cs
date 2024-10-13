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
    };

    [SerializeField] Image _panel, _rankPanel;
    [SerializeField] TextMeshProUGUI _playerName, _rank;
    public PlayerInfo Player { get; set; }

    public void ShowResult(int rank)
    {
        _playerName.text = Player.transform.name;
        _panel.color = Player.MaterialColor;
        _rank.text = rank.ToString();
        _rankPanel.color = _rankColors[rank - 1];

        _panel.DOFade(1, 1f).From(0);
        _playerName.DOFade(1, 1f).From(0);
    }
}
