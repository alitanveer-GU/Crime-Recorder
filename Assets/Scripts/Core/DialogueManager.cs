using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager _instance;
    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("DialogueManager: No instance in scene.");
            }
            return _instance;
        }
    }

    [SerializeField] private DialogueRunner _dialogueRunner;

    private NPCInteraction _currentNpc;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("DialogueManager: Multiple instances detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        if (_dialogueRunner == null)
        {
            Debug.LogWarning("DialogueManager: DialogueRunner is not assigned.");
            return;
        }

        _dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
    }

    public void StartDialogueForNPC(NPCInteraction npc)
    {
        if (npc == null)
        {
            Debug.LogWarning("DialogueManager.StartDialogueForNPC called with null npc.");
            return;
        }

        if (_dialogueRunner == null)
        {
            Debug.LogWarning("DialogueManager: DialogueRunner is not assigned.");
            return;
        }

        var suspect = npc.SuspectData;
        if (suspect == null)
        {
            Debug.LogWarning("DialogueManager: NPC has no SuspectData.");
            return;
        }

        _currentNpc = npc;

        string nodeToStart = GetNodeForSuspect(suspect);

        if (string.IsNullOrEmpty(nodeToStart))
        {
            Debug.LogWarning($"DialogueManager: No dialogue node configured for suspect {suspect.DisplayName}.");
            return;
        }

        var gsm = GameStateManager.Instance;
        if (gsm != null)
        {
            gsm.SetState(GameState.Dialogue);
        }

        var jm = JournalManager.Instance;
        if (jm != null)
        {
            jm.MarkInterrogated(suspect);
        }

        Debug.Log($"Starting dialogue node '{nodeToStart}' for suspect {suspect.DisplayName}.");
        _dialogueRunner.StartDialogue(nodeToStart);
    }

    private string GetNodeForSuspect(SuspectData suspect)
    {
        var jm = JournalManager.Instance;
        JournalEntryRuntime entry = null;

        if (jm != null)
        {
            entry = jm.GetOrCreateEntry(suspect);
        }

        if (entry != null && entry.RecordUsed && !string.IsNullOrEmpty(suspect.AfterRecordDialogueNode))
        {
            return suspect.AfterRecordDialogueNode;
        }

        if (!string.IsNullOrEmpty(suspect.IntroDialogueNode))
        {
            return suspect.IntroDialogueNode;
        }

        if (!string.IsNullOrEmpty(suspect.AfterRecordDialogueNode))
        {
            return suspect.AfterRecordDialogueNode;
        }

        if (!string.IsNullOrEmpty(suspect.PostAccusationDialogueNode))
        {
            return suspect.PostAccusationDialogueNode;
        }

        return null;
    }

    private void OnDialogueComplete()
    {
        var gsm = GameStateManager.Instance;
        if (gsm != null && gsm.IsState(GameState.Dialogue))
        {
            gsm.SetState(GameState.FreeRoam);
        }

        Debug.Log("Dialogue complete.");
        _currentNpc = null;
    }
}
