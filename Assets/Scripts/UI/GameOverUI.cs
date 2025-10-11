using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] Image _bg, _gameOverHeader;
    [SerializeField] GameObject _resultsGroup;
    [SerializeField] GameObject _resultsPrefab;

    void Start()
    {
        gameObject.SetActive(false);
        _gameOverHeader.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        gameObject.SetActive(true);
        var seq = DOTween.Sequence();
        seq.Append(_bg.DOFade(1, 0.5f).From(0))
            .Append(_gameOverHeader.DOFade(1, 0.5f).From(0).OnPlay(() => _gameOverHeader.gameObject.SetActive(true)))
            .Append(_gameOverHeader.transform.DOLocalMoveY(360, 1f).SetEase(Ease.OutBounce));

        seq.Play().OnComplete(() =>
        {
            StartCoroutine(ShowResults());
        });
    }

    IEnumerator ShowResults()
    {
        int count = 0;
        while (GameManager.Instance.WinnerStack.Count > 0)
        {
            var winner = GameManager.Instance.WinnerStack.Pop();
            var result = Instantiate(_resultsPrefab, _resultsGroup.transform).GetComponent<GameOverResultUI>();
            result.Player = winner.GetComponent<PlayerInfo>();
            count++;
            result.ShowResult(count);
            yield return new WaitForEndOfFrame();
        }
    }

}
