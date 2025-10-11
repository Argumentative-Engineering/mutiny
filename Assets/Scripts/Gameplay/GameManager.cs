using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    public List<GameObject> AlivePlayers { get; private set; } = new();
    public readonly Stack<GameObject> WinnerStack = new();

    [Header("References")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private PlayerInputManager _inputManager;
    [SerializeField] private GameObject _healthBarsUIGroup;
    [SerializeField] private GameObject _healthBarPrefab;
    [SerializeField] private GameOverUI _gameOverUIController;
    [SerializeField] private List<Transform> _playerSpawnPoints;
    private int _nextSpawn;

    public static GameManager Instance { get; private set; }
    private bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        _inputManager.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDisable()
    {
        _inputManager.onPlayerJoined -= OnPlayerJoined;
    }

    private void Start()
    {
        _nextSpawn = 0;
        AlivePlayers.Clear();
        WinnerStack.Clear();

        PlayerRoster.Instance.StageReset();

        foreach (var slot in PlayerRoster.Instance.Slots)
        {
            if (slot.handle == null || !slot.isActive) continue;
            SpawnPlayer(slot);
        }
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        PlayerRoster.Instance.RegisterPlayer(input);

        var slot = PlayerRoster.Instance.Slots[input.playerIndex];
        if (slot.handle != null)
            SpawnPlayer(slot);
    }

    public void SpawnPlayer(PlayerRoster.PlayerSlot slot)
    {
        Vector3 spawnPos = _playerSpawnPoints[_nextSpawn].position;
        _nextSpawn = (_nextSpawn + 1) % _playerSpawnPoints.Count;

        GameObject player = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);

        var connector = slot.handle;
        if (connector != null)
        {
            connector.AttachToPlayer(player);
        }

        var info = player.GetComponent<PlayerInfo>();
        info.SetColor(slot.playerIndex, slot.playerColor);

        var healthUI = Instantiate(_healthBarPrefab, _healthBarsUIGroup.transform)
            .GetComponent<PlayerHealthUI>();
        healthUI.Player = info;
        healthUI.PlayerHealth = player.GetComponent<PlayerHealth>();
        healthUI.UpdateText();

        AlivePlayers.Add(player);
    }

    public void PlayerDied(GameObject player)
    {
        if (isGameOver) return;

        WinnerStack.Push(player);
        AlivePlayers.Remove(player);
        FindPlayer(PlayerRoster.Instance.Slots, player.GetComponent<PlayerController>());

        if (AlivePlayers.Count <= 1)
        {
            Invoke(nameof(SetGameOver), 2f);
            Vector3 finalKillPos = player.transform.position;
            StartCoroutine(GameOver(finalKillPos));
        }
    }

    void SetGameOver()
    {
        isGameOver = true;
    }

    private void FindPlayer(PlayerRoster.PlayerSlot[] slots, PlayerController player)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].handle == null) continue;
            if (slots[i].handle._player == player)
            {
                slots[i].diedThisRound = true;
                break;
            }
        }
    }

    private IEnumerator GameOver(Vector3 finalKillPosition)
    {
        foreach (var player in AlivePlayers)
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

    public void Restart()
    {
        if(!isGameOver) return;
        SceneLoader.Instance.LoadRandomScene();
    }
}
