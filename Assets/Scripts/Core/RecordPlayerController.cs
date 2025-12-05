using System;
using UnityEngine;

public class RecordPlayerController : MonoBehaviour
{
    private static RecordPlayerController _instance;
    public static RecordPlayerController Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("RecordPlayerController: No instance in scene.");
            }
            return _instance;
        }
    }

    [SerializeField] private int _maxUses = 3;
    [SerializeField] private RecordPlayerAudioProfile _audioProfile;
    [SerializeField] private RecordPlayerItem _recordPlayerItem;
    [SerializeField] private float _npcAudioVolume = 1f;

    private int _usesLeft;

    public int UsesLeft => _usesLeft;
    public int MaxUses => _maxUses;

    public event Action<int, int> OnUsesChanged;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("RecordPlayerController: Multiple instances detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _usesLeft = Mathf.Max(0, _maxUses);
    }

    public bool CanUseOnNPC => _usesLeft > 0;

    public void ResetUses()
    {
        _usesLeft = Mathf.Max(0, _maxUses);
        OnUsesChanged?.Invoke(_usesLeft, _maxUses);
    }

    public void TryUseOnNPC(NPCInteraction npc)
    {
        if (npc == null)
        {
            Debug.LogWarning("RecordPlayerController.TryUseOnNPC was given a null NPC.");
            return;
        }

        if (!CanUseOnNPC)
        {
            Debug.Log("RecordPlayer: No uses left.");
            return;
        }

        _usesLeft = Mathf.Max(0, _usesLeft - 1);
        OnUsesChanged?.Invoke(_usesLeft, _maxUses);

        PlayEmotionTrackForNPC(npc);

        var suspect = npc.SuspectData;
        if (suspect != null && JournalManager.Instance != null)
        {
            JournalManager.Instance.MarkRecordUsed(suspect);
        }
    }

    private void PlayEmotionTrackForNPC(NPCInteraction npc)
    {
        if (_audioProfile == null)
        {
            Debug.LogWarning("RecordPlayerController: No audio profile assigned.");
            return;
        }

        var suspect = npc.SuspectData;
        if (suspect == null)
            return;

        var clip = _audioProfile.GetClipForEmotion(suspect.Emotion);
        if (clip == null)
            return;

        var pos = npc.transform.position;
        AudioSource.PlayClipAtPoint(clip, pos, _npcAudioVolume);
    }

    public void PlayDetectiveThemeAt(Vector3 position)
    {
        if (_audioProfile == null || _audioProfile.DetectiveTheme == null)
            return;

        AudioSource.PlayClipAtPoint(_audioProfile.DetectiveTheme, position, _npcAudioVolume);
    }
}
