using UnityEngine;
using UnityEngine.UI;

public class HUDUI : MonoBehaviour
{
    [Header("Interaction Prompt")]
    [SerializeField] private GameObject _promptRoot;
    [SerializeField] private Text _promptText;

    [Header("Record Uses")]
    [SerializeField] private Text _recordUsesText;

    [Header("Player Interactor (optional, will auto-find if null)")]
    [SerializeField] private PlayerInteractor _playerInteractor;

    private void Awake()
    {
        if (_promptRoot != null)
            _promptRoot.SetActive(false);
    }

    private void Start()
    {
        if (_playerInteractor == null)
        {
            _playerInteractor = FindFirstObjectByType<PlayerInteractor>();
        }

        if (_playerInteractor != null)
        {
            _playerInteractor.OnFocusedInteractableChanged += HandleFocusedInteractableChanged;
        }

        var rpc = RecordPlayerController.Instance;
        if (rpc != null)
        {
            rpc.OnUsesChanged += HandleRecordUsesChanged;
            HandleRecordUsesChanged(rpc.UsesLeft, rpc.MaxUses);
        }
        else
        {
            if (_recordUsesText != null)
                _recordUsesText.text = string.Empty;
        }
    }

    private void OnDestroy()
    {
        if (_playerInteractor != null)
        {
            _playerInteractor.OnFocusedInteractableChanged -= HandleFocusedInteractableChanged;
        }

        var rpc = RecordPlayerController.Instance;
        if (rpc != null)
        {
            rpc.OnUsesChanged -= HandleRecordUsesChanged;
        }
    }

    private void HandleFocusedInteractableChanged(IInteractable interactable)
    {
        if (_promptRoot == null || _promptText == null)
            return;

        if (interactable == null)
        {
            _promptRoot.SetActive(false);
            _promptText.text = string.Empty;
        }
        else
        {
            _promptRoot.SetActive(true);
            _promptText.text = interactable.GetInteractionPrompt() + " (E)";
        }
    }

    private void HandleRecordUsesChanged(int usesLeft, int maxUses)
    {
        if (_recordUsesText == null)
            return;

        _recordUsesText.text = $"Record uses: {usesLeft}/{maxUses}";
    }
}
