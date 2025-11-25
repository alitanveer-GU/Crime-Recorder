using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NPCInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private SuspectData _suspectData;

    [Tooltip("Optional override for the HUD prompt. If empty, a default will be generated.")]
    [SerializeField] private string _customPrompt;

    public SuspectData SuspectData => _suspectData;

    public string GetInteractionPrompt()
    {
        if (!string.IsNullOrWhiteSpace(_customPrompt))
            return _customPrompt;

        if (_suspectData != null && !string.IsNullOrWhiteSpace(_suspectData.DisplayName))
            return $"Talk to {_suspectData.DisplayName}";

        return "Talk";
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (_suspectData == null)
        {
            Debug.LogWarning($"NPCInteraction on {name} has no SuspectData assigned.");
            return;
        }

        // Switch to InteractionMenu state. Later I’ll hook this into UI.
        var gsm = GameStateManager.Instance;
        if (gsm != null)
        {
            gsm.SetState(GameState.InteractionMenu);
        }

        Debug.Log($"Interacting with suspect: {_suspectData.DisplayName} (ID: {_suspectData.SuspectId})");
    }
}