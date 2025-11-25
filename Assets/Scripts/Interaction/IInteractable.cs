public interface IInteractable
{
    /// <summary>Called when the player activates this object.</summary>
    void Interact(PlayerInteractor interactor);

    /// <summary>Short text to show in the HUD.</summary>
    string GetInteractionPrompt();
}