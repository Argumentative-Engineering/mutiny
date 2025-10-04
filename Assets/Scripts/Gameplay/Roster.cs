using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRoster : MonoBehaviour
{
    public static PlayerRoster Instance { get; private set; }

    [System.Serializable]
    public class PlayerSlot
    {
        public int playerIndex;
        public Color playerColor;
        public PlayerHandleConnector handle;
        public bool isActive;
        public bool diedThisRound;
    }

    [SerializeField] private int _maxPlayers = 10;
    public PlayerSlot[] Slots { get; private set; }

    [SerializeField] private Color[] _defaultColors;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Slots = new PlayerSlot[_maxPlayers];
        for (int i = 0; i < _maxPlayers; i++)
        {
            Slots[i] = new PlayerSlot
            {
                playerIndex = i,
                playerColor = i < _defaultColors.Length ? _defaultColors[i] : Color.white,
                handle = null,
                isActive = false,
                diedThisRound = false
            };
        }
    }

    public void StageReset()
    {
        for (int i = 0; i < _maxPlayers; i++)
        {
            Slots[i].diedThisRound = false;
        }
    }

    public void RegisterPlayer(PlayerInput input)
    {
        var connector = input.GetComponent<PlayerHandleConnector>();
        if (connector == null)
        {
            Debug.LogError("PlayerInput joined but has no PlayerHandleConnector!");
            return;
        }

        var slot = Slots[input.playerIndex];
        slot.playerIndex = input.playerIndex;
        slot.playerColor = input.playerIndex < _defaultColors.Length
            ? _defaultColors[input.playerIndex]
            : Color.white;
        slot.handle = connector;
        slot.handle.Initialize(slot);
        slot.isActive = true;
    }

    public void UnregisterPlayer(PlayerInput input)
    {
        var slot = Slots[input.playerIndex];
        slot.handle = null;
        slot.isActive = false;
        slot.diedThisRound = true;
    }
}
