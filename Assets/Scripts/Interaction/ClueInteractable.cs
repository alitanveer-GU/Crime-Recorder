using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ClueInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private ClueData _clue;
    [SerializeField] private string _customPrompt;

    public ClueData Clue => _clue;

    public string GetInteractionPrompt()
    {
        if (!string.IsNullOrEmpty(_customPrompt))
            return _customPrompt;

        if (_clue != null && !string.IsNullOrEmpty(_clue.Title))
            return $"Inspect {_clue.Title}";

        return "Inspect";
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (_clue == null)
        {
            Debug.LogWarning($"ClueInteractable on {name} has no ClueData assigned.");
            return;
        }

        if (JournalManager.Instance != null)
        {
            JournalManager.Instance.RegisterClueFound(_clue);
        }

        Debug.Log($"Clue found: {_clue.Title} (ID: {_clue.ClueId})");
    }
}