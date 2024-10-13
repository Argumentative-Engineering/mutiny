
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerInfo Player { get; set; }
    public PlayerHealth PlayerHealth { get; set; }

    [Header("References")]
    [SerializeField] Image _bgUI;
    [SerializeField] TextMeshProUGUI _healthUIText;
    [SerializeField] TextMeshProUGUI _colorUIText;

    public void UpdateText()
    {
        _bgUI.color = Player.MaterialColor;
        _colorUIText.text = Player.TeamColor.ToString();
        _healthUIText.text = PlayerHealth.Health.ToString("N1");
    }

    void Update()
    {
        _healthUIText.text = PlayerHealth.Health.ToString("N1");
    }
}
