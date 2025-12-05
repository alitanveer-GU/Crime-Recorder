using System;
using UnityEngine;

public class AccusationManager : MonoBehaviour
{
    private static AccusationManager _instance;
    public static AccusationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("AccusationManager: No instance in scene.");
            }
            return _instance;
        }
    }

    public event Action<bool, SuspectData> OnAccusationResult;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("AccusationManager: Multiple instances detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Accuse(SuspectData suspect)
    {
        if (suspect == null)
        {
            Debug.LogWarning("AccusationManager.Accuse called with null suspect.");
            return;
        }

        bool correct = suspect.IsCulprit;

        Debug.Log($"Accusation made: {suspect.DisplayName}. Correct: {correct}");

        OnAccusationResult?.Invoke(correct, suspect);

        var gsm = GameStateManager.Instance;
        if (gsm != null)
        {
            gsm.SetState(GameState.Accusation);
        }
    }
}