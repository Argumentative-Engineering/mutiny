
using DG.Tweening;
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
    [SerializeField] GameObject _stunnedStatusUI, _deadStatusUI;

    bool _isShaking = false;
    Tween _shakeTween;

    public void UpdateText()
    {
        _bgUI.color = Player.MaterialColor;
        _colorUIText.text = Player.transform.name;
        _healthUIText.text = PlayerHealth.Health.ToString("N1");
    }

    void Update()
    {
        _healthUIText.text = PlayerHealth.Health.ToString("N1");
        _healthUIText.color = Color.Lerp(Color.red, Color.white, PlayerHealth.Health / 100);

        _stunnedStatusUI.SetActive(PlayerHealth.IsStunned);
        _deadStatusUI.SetActive(PlayerHealth.Health <= 0);

        if (PlayerHealth.Health <= 20 && PlayerHealth.Health > 0 & !_isShaking)
        {
            _isShaking = true;
            _shakeTween = _healthUIText.transform.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.OutBounce).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            _isShaking = false;
            if (_shakeTween != null)
            {
                _shakeTween.Kill();
                _shakeTween = null;
            }
        }

    }
}
