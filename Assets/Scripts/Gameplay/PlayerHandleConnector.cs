using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(PlayerInput))]
public class PlayerHandleConnector : MonoBehaviour
{
    public int playerIndex;
    public PlayerInput _player;
    public InputActionMap _playerInput { get; private set; }

    public PlayerController _controlledPlayer;

    private void Awake()
    {
        _player = GetComponent<PlayerInput>();
        _playerInput = _player.actions.FindActionMap("Player");
        playerIndex = _player.playerIndex;
        DontDestroyOnLoad(gameObject);

        _playerInput.FindAction("Move").performed += OnMovePerformed;
        _playerInput.FindAction("Move").canceled += OnMoveCancelled;
        _playerInput.FindAction("Jump").performed += OnJumpPerformed;
        _playerInput.FindAction("Aim").performed += OnAimPerformed;
        _playerInput.FindAction("Fire").performed += OnFirePerformed;
        _playerInput.FindAction("Fire").canceled += OnFireCancelled;
        _playerInput.FindAction("Leave").performed += OnLeave;
    }
    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();

        _playerInput.FindAction("Move").performed -= OnMovePerformed;
        _playerInput.FindAction("Move").canceled -= OnMoveCancelled;
        _playerInput.FindAction("Jump").performed -= OnJumpPerformed;
        _playerInput.FindAction("Aim").performed -= OnAimPerformed;
        _playerInput.FindAction("Fire").performed -= OnFirePerformed;
        _playerInput.FindAction("Fire").canceled -= OnFireCancelled;
    }

    public void AttachToPlayer(GameObject player)
    {
        _controlledPlayer = player.GetComponent<PlayerController>();
        //if (_controlledPlayer!=null && _player.currentControlScheme == "Keyboard&Mouse") _controlledPlayer.SetMouse();
    }

    public void Detach()
    {
        if (_controlledPlayer != null) _controlledPlayer = null;
    }

    public void OnLeave(InputAction.CallbackContext context)
    {
        PlayerRoster.Instance.UnregisterPlayer(_player);
        _controlledPlayer.GetComponent<PlayerHealth>().Die(Vector3.zero);
        Destroy(gameObject);
    }

    public void OnAimPerformed(InputAction.CallbackContext context) => _controlledPlayer?.XOnAimPerformed(context);
    public void OnFirePerformed(InputAction.CallbackContext context) => _controlledPlayer?.XOnFirePerformed(context);
    public void OnFireCancelled(InputAction.CallbackContext context) => _controlledPlayer?.XOnFireCancelled(context);
    public void OnMovePerformed(InputAction.CallbackContext context) => _controlledPlayer?.XOnMovePerformed(context);
    public void OnMoveCancelled(InputAction.CallbackContext context) => _controlledPlayer.XOnMoveCancelled(context);
    public void OnJumpPerformed(InputAction.CallbackContext context) => _controlledPlayer?.XOnJumpPerformed(context);
}
