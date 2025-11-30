using UnityEngine;
using UnityEngine.UI;

public class InteractionMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private Text _npcNameText;

    private NPCInteraction _currentNpc;

    public NPCInteraction CurrentNpc => _currentNpc;

    public void Open(NPCInteraction npc)
    {
        _currentNpc = npc;

        if (_npcNameText != null && npc != null && npc.SuspectData != null)
        {
            _npcNameText.text = npc.SuspectData.DisplayName;
        }

        if (_root != null)
        {
            _root.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
        }

        var gsm = GameStateManager.Instance;
        if (gsm != null)
        {
            gsm.SetState(GameState.InteractionMenu);
        }

        Debug.Log($"Interaction menu opened for NPC: {GetNpcNameForLog()}");
    }

    public void Close()
    {
        _currentNpc = null;

        if (_root != null)
        {
            _root.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }

        var gsm = GameStateManager.Instance;
        if (gsm != null && gsm.IsState(GameState.InteractionMenu))
        {
            gsm.SetState(GameState.FreeRoam);
        }

        Debug.Log("Interaction menu closed.");
    }

    public void OnAskQuestions()
    {
        if (_currentNpc == null)
            return;

        var dm = DialogueManager.Instance;
        if (dm == null)
        {
            Debug.LogWarning("InteractionMenuUI: DialogueManager.Instance is null.");
            return;
        }

        dm.StartDialogueForNPC(_currentNpc);
    }

    public void OnUseRecordPlayer()
    {
        if (_currentNpc == null)
            return;

        var rpc = RecordPlayerController.Instance;
        if (rpc == null)
        {
            Debug.LogWarning("InteractionMenuUI: RecordPlayerController.Instance is null.");
            return;
        }

        rpc.TryUseOnNPC(_currentNpc);
    }

    public void OnExitInteraction()
    {
        Close();
    }

    private string GetNpcNameForLog()
    {
        if (_currentNpc == null)
            return "(no NPC)";

        var data = _currentNpc.SuspectData;
        if (data != null && !string.IsNullOrEmpty(data.DisplayName))
            return data.DisplayName;

        return _currentNpc.name;
    }
}
