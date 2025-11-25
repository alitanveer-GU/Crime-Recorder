using System;
using UnityEngine;

/// <summary>
/// All high-level modes the game can be in.
/// </summary>
public enum GameState
{
    FreeRoam,        // Player can walk around and interact.
    InteractionMenu, // The 3-option NPC interaction menu is open.
    Dialogue,        // In a dialogue with an NPC.
    Journal,         // Viewing or editing the journal.
    Accusation,      // Making the final accusation.
    Paused           // Pause menu open.
}

/// <summary>
/// Singleton component that tracks the current GameState and provides events for other systems to react to state changes.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    private static GameStateManager _instance;

    /// <summary>
    /// Global access to the active GameStateManager.
    /// </summary>
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameStateManager: No instance in scene. " +
                               "Make sure there is exactly one GameStateManager object.");
            }
            return _instance;
        }
    }
    
    [Tooltip("Initial state when the scene starts.")]
    [SerializeField] private GameState _initialState = GameState.FreeRoam;

    [Tooltip("If true, this object will persist across scene loads.")]
    [SerializeField] private bool _dontDestroyOnLoad = true;
    
    /// <summary>
    /// The current active game state.
    /// </summary>
    public GameState CurrentState { get; private set; }

    /// <summary>
    /// Raised whenever the game state changes.
    /// Parameters: (previousState, newState).
    /// </summary>
    public event Action<GameState, GameState> OnGameStateChanged;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("GameStateManager: Multiple instances detected. " + "Destroying the newer one on object: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        _instance = this;

        if (_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
        SetState(_initialState, force: true);
    }
    
    /// <summary>
    /// Change the current game state.
    /// </summary>
    /// <param name="newState">The new state to switch to.</param>
    /// <param name="force">If true, will fire the event even if the state is the same as before.</param>
    public void SetState(GameState newState, bool force = false)
    {
        if (!force && newState == CurrentState)
            return;

        var previousState = CurrentState;
        CurrentState = newState;

        ApplyGlobalStateSideEffects(previousState, newState);

        OnGameStateChanged?.Invoke(previousState, newState);
    }

    /// <summary>
    /// Convenience check: are we currently in the given state?
    /// </summary>
    public bool IsState(GameState state)
    {
        return CurrentState == state;
    }

    /// <summary>
    /// Convenience check: are we currently in any of these states?
    /// </summary>
    public bool IsAnyState(params GameState[] states)
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (CurrentState == states[i])
                return true;
        }
        return false;
    }

    /// <summary>
    /// Apply side effects whenever we enter/leave certain states.
    /// For example: locking/unlocking cursor, pausing time, etc.
    /// </summary>
    private void ApplyGlobalStateSideEffects(GameState previous, GameState next)
    {
        // Cursor handling: FreeRoam = locked, gameplay; UI states = unlocked.
        bool usingUI =
            next == GameState.InteractionMenu ||
            next == GameState.Dialogue ||
            next == GameState.Journal ||
            next == GameState.Accusation ||
            next == GameState.Paused;

        if (usingUI)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Time handling: Paused = freeze time, others = normal.
        if (next == GameState.Paused)
        {
            Time.timeScale = 0f;
        }
        else if (previous == GameState.Paused && next != GameState.Paused)
        {
            Time.timeScale = 1f;
        }
    }
}
