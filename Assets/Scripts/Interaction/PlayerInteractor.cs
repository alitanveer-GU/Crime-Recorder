using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float _interactionDistance = 3f;
    [SerializeField] private LayerMask _interactionMask = ~0;

    public event Action<IInteractable> OnFocusedInteractableChanged;

    private Camera _cam;
    private IInteractable _currentInteractable;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        if (_cam == null)
        {
            Debug.LogError("PlayerInteractor requires a Camera on the same GameObject.");
        }
    }

    private void Update()
    {
        // Only allow interactions in FreeRoam.
        var gsm = GameStateManager.Instance;
        if (gsm != null && !gsm.IsState(GameState.FreeRoam))
            return;

        UpdateFocus();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    private void UpdateFocus()
    {
        IInteractable newInteractable = null;

        if (_cam != null)
        {
            var ray = new Ray(_cam.transform.position, _cam.transform.forward);
            if (Physics.Raycast(ray, out var hit, _interactionDistance, _interactionMask, QueryTriggerInteraction.Ignore))
            {
                hit.collider.TryGetComponent<IInteractable>(out newInteractable);
            }
        }

        if (!ReferenceEquals(newInteractable, _currentInteractable))
        {
            _currentInteractable = newInteractable;
            OnFocusedInteractableChanged?.Invoke(_currentInteractable);

            // For now, just logging but later I will hook this into HUD.
            if (_currentInteractable != null)
            {
                Debug.Log($"Looking at: {_currentInteractable.GetInteractionPrompt()}");
            }
        }
    }

    private void TryInteract()
    {
        if (_currentInteractable == null)
            return;

        _currentInteractable.Interact(this);
    }
}
