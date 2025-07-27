using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] List<Color> _playerColors = new();
    [field: SerializeField]
    public List<GameObject> Players { get; private set; }
    public List<GameObject> AlivePlayers { get; set; } = new();

    public readonly Stack<GameObject> WinnerStack = new();

    [SerializeField] List<Transform> _playerSpawnPoints;
    int _nextSpawn;

    [Header("References")]
    [SerializeField] PlayerInputManager _inputManager;
    [SerializeField] GameObject _healthBarsUIGroup;
    [SerializeField] GameObject _healthBarPrefab;
    [SerializeField] GameOverUI _gameOverUIController;

    public static GameManager Instance { get; private set; }
    private bool isGameOver = false;
    private void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        _inputManager.onPlayerJoined += OnPlayerJoined;
        _inputManager.onPlayerLeft += OnPlayerLeft;
    }

    private void OnDisable()
    {
        _inputManager.onPlayerJoined -= OnPlayerJoined;
        _inputManager.onPlayerLeft -= OnPlayerLeft;
    }

    private void OnPlayerLeft(PlayerInput input)
    {
        Players.Remove(input.gameObject);
    }

    private void OnPlayerJoined(PlayerInput input)
    {
        if (Players.Count == 0) MusicManager.Instance.PlayMusic();
        Players.Add(input.gameObject);
        AlivePlayers.Add(input.gameObject);
        if (_playerSpawnPoints.Count == 0) return;

        var spawn = _playerSpawnPoints[_nextSpawn];
        _nextSpawn = (_nextSpawn + 1) % _playerSpawnPoints.Count;
        input.GetComponent<Rigidbody>().position = spawn.position;

        var number = Players.Count - 1;
        input.GetComponent<PlayerInfo>().SetColor(number, _playerColors[number]);

        var health = Instantiate(_healthBarPrefab, _healthBarsUIGroup.transform).GetComponent<PlayerHealthUI>();
        health.Player = input.GetComponent<PlayerInfo>();
        health.PlayerHealth = input.GetComponent<PlayerHealth>();
        health.UpdateText();
    }

    public void PlayerDied(GameObject player)
    {
        if (isGameOver) return;

        WinnerStack.Push(player);
        AlivePlayers.Remove(player);

        if (AlivePlayers.Count <= 1)
        {
            isGameOver = true;
            Vector3 finalKillPos = player.transform.position;
            StartCoroutine(GameOver(finalKillPos));
        }
    }

    public IEnumerator GameOver(Vector3 finalKillPosition)
    {
        foreach (var player in Players)
        {
            player.GetComponent<PlayerController>().enabled = false;
        }

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.15f);

        Time.timeScale = 0.1f;
        CameraController cam = Camera.main.GetComponent<CameraController>();
        cam.ZoomToPoint(finalKillPosition, zoomZOffset: 15f);
        yield return new WaitForSecondsRealtime(1.2f);

        Time.timeScale = 1f;
        cam.ResetZoom();

        foreach (var alive in AlivePlayers)
        {
            WinnerStack.Push(alive);
        }

        _gameOverUIController.gameObject.SetActive(true);
        _gameOverUIController.GameOver();
    }
}
