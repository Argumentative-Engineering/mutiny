using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [field: SerializeField]
    public List<GameObject> Players { get; private set; }

    [Header("References")]
    [SerializeField] PlayerInputManager _inputManager;

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
    }
}
