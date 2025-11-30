using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("UIManager: No instance in scene.");
            }
            return _instance;
        }
    }

    [SerializeField] private InteractionMenuUI _interactionMenu;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("UIManager: Multiple instances detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    public void ShowInteractionMenu(NPCInteraction npc)
    {
        if (_interactionMenu == null)
        {
            Debug.LogWarning("UIManager: InteractionMenuUI is not assigned.");
            return;
        }

        if (npc == null)
            return;

        _interactionMenu.Open(npc);
    }

    public void HideInteractionMenu()
    {
        if (_interactionMenu == null)
            return;

        _interactionMenu.Close();
    }
}