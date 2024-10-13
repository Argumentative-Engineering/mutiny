using System;
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

    [SerializeField]
    List<Transform> _playerSpawnPoints;

    [Header("References")]
    [SerializeField] PlayerInputManager _inputManager;
    [SerializeField] GameObject _healthBarsUIGroup;
    [SerializeField] GameObject _healthBarPrefab;

    public static GameManager Instance { get; private set; }
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
        Players.Add(input.gameObject);
        AlivePlayers.Add(input.gameObject);
        if (_playerSpawnPoints.Count == 0) return;

        var spawn = _playerSpawnPoints[UnityEngine.Random.Range(0, _playerSpawnPoints.Count)];
        input.GetComponent<Rigidbody>().position = spawn.position;

        var number = Players.Count - 1;
        input.transform.name = $"Player {number + 1}";
        input.GetComponent<PlayerInfo>().SetColor(number, _playerColors[number]);

        var health = Instantiate(_healthBarPrefab, _healthBarsUIGroup.transform).GetComponent<PlayerHealthUI>();
        health.Player = input.GetComponent<PlayerInfo>();
        health.PlayerHealth = input.GetComponent<PlayerHealth>();
        health.UpdateText();
    }
}
