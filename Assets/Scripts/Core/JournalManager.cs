using System;
using System.Collections.Generic;
using UnityEngine;

public class JournalManager : MonoBehaviour
{
    private static JournalManager _instance;
    public static JournalManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("JournalManager: No instance in scene.");
            }
            return _instance;
        }
    }

    private readonly Dictionary<string, JournalEntryRuntime> _entries = new Dictionary<string, JournalEntryRuntime>();
    private readonly HashSet<string> _foundClueIds = new HashSet<string>();

    public event Action<JournalEntryRuntime> OnJournalEntryChanged;
    public event Action<ClueData> OnClueFound;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("JournalManager: Multiple instances detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public JournalEntryRuntime GetOrCreateEntry(SuspectData suspect)
    {
        if (suspect == null || string.IsNullOrEmpty(suspect.SuspectId))
        {
            Debug.LogWarning("JournalManager.GetOrCreateEntry called with invalid suspect.");
            return null;
        }

        if (!_entries.TryGetValue(suspect.SuspectId, out var entry))
        {
            entry = new JournalEntryRuntime(suspect);
            _entries.Add(suspect.SuspectId, entry);
        }

        return entry;
    }

    public void SetAnswer(SuspectData suspect, string questionId, bool answer)
    {
        if (suspect == null || string.IsNullOrEmpty(questionId))
            return;

        var entry = GetOrCreateEntry(suspect);
        if (entry == null)
            return;

        entry.Answers[questionId] = answer;
        OnJournalEntryChanged?.Invoke(entry);
    }

    public void MarkRecordUsed(SuspectData suspect)
    {
        if (suspect == null)
            return;

        var entry = GetOrCreateEntry(suspect);
        if (entry == null)
            return;

        if (!entry.RecordUsed)
        {
            entry.RecordUsed = true;
            OnJournalEntryChanged?.Invoke(entry);
        }
    }

    public void MarkInterrogated(SuspectData suspect)
    {
        if (suspect == null)
            return;

        var entry = GetOrCreateEntry(suspect);
        if (entry == null)
            return;

        if (!entry.HasInterrogated)
        {
            entry.HasInterrogated = true;
            OnJournalEntryChanged?.Invoke(entry);
        }
    }

    public bool HasInterrogated(SuspectData suspect)
    {
        if (suspect == null || string.IsNullOrEmpty(suspect.SuspectId))
            return false;

        if (_entries.TryGetValue(suspect.SuspectId, out var entry))
        {
            return entry.HasInterrogated;
        }

        return false;
    }

    public void RegisterClueFound(ClueData clue)
    {
        if (clue == null || string.IsNullOrEmpty(clue.ClueId))
            return;

        if (_foundClueIds.Add(clue.ClueId))
        {
            OnClueFound?.Invoke(clue);
        }
    }

    public bool HasFoundClue(string clueId)
    {
        if (string.IsNullOrEmpty(clueId))
            return false;

        return _foundClueIds.Contains(clueId);
    }
}
