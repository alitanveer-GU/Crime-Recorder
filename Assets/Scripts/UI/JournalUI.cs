using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;

    [Header("Suspects")]
    [SerializeField] private Transform _suspectListContainer;
    [SerializeField] private GameObject _suspectButtonPrefab;
    [SerializeField] private List<SuspectData> _allSuspects = new List<SuspectData>();

    [Header("Questions")]
    [SerializeField] private Transform _questionListContainer;
    [SerializeField] private GameObject _questionTogglePrefab;

    [Header("Accuse Button")]
    [SerializeField] private Button _accuseButton;

    private readonly List<GameObject> _spawnedSuspectButtons = new List<GameObject>();
    private readonly List<GameObject> _spawnedQuestionRows = new List<GameObject>();

    private SuspectData _currentSuspect;

    private void Start()
    {
        if (_root != null)
            _root.SetActive(false);

        BuildSuspectList();
    }

    private void BuildSuspectList()
    {
        if (_suspectListContainer == null || _suspectButtonPrefab == null)
            return;

        foreach (var go in _spawnedSuspectButtons)
            Destroy(go);
        _spawnedSuspectButtons.Clear();

        for (int i = 0; i < _allSuspects.Count; i++)
        {
            var suspect = _allSuspects[i];
            if (suspect == null)
                continue;

            var buttonObj = Instantiate(_suspectButtonPrefab, _suspectListContainer);
            _spawnedSuspectButtons.Add(buttonObj);

            var text = buttonObj.GetComponentInChildren<Text>();
            if (text != null)
                text.text = suspect.DisplayName;

            var btn = buttonObj.GetComponent<Button>();
            if (btn != null)
            {
                var captured = suspect;
                btn.onClick.AddListener(() => OnSelectSuspect(captured));
            }
        }
    }

    private void OnSelectSuspect(SuspectData suspect)
    {
        _currentSuspect = suspect;
        RefreshQuestions();
    }

    private void RefreshQuestions()
    {
        foreach (var go in _spawnedQuestionRows)
            Destroy(go);
        _spawnedQuestionRows.Clear();

        if (_currentSuspect == null || _questionListContainer == null || _questionTogglePrefab == null)
            return;

        var jm = JournalManager.Instance;
        if (jm == null)
            return;

        var entry = jm.GetOrCreateEntry(_currentSuspect);
        if (entry == null)
            return;

        for (int i = 0; i < _currentSuspect.JournalQuestions.Count; i++)
        {
            var config = _currentSuspect.JournalQuestions[i];
            if (config == null || string.IsNullOrEmpty(config.QuestionId))
                continue;

            var rowObj = Instantiate(_questionTogglePrefab, _questionListContainer);
            _spawnedQuestionRows.Add(rowObj);

            var text = rowObj.GetComponentInChildren<Text>();
            if (text != null)
                text.text = config.QuestionText;

            var toggle = rowObj.GetComponentInChildren<Toggle>();
            if (toggle != null)
            {
                bool currentValue = config.DefaultAnswer;
                if (entry.Answers.TryGetValue(config.QuestionId, out var storedValue))
                    currentValue = storedValue;

                toggle.isOn = currentValue;

                var capturedQuestionId = config.QuestionId;
                toggle.onValueChanged.AddListener(value =>
                {
                    jm.SetAnswer(_currentSuspect, capturedQuestionId, value);
                });
            }
        }

        if (_accuseButton != null)
        {
            _accuseButton.interactable = _currentSuspect != null;
        }
    }

    public void OpenJournal()
    {
        if (_root != null)
            _root.SetActive(true);

        var gsm = GameStateManager.Instance;
        if (gsm != null)
        {
            gsm.SetState(GameState.Journal);
        }

        if (_currentSuspect == null && _allSuspects.Count > 0)
        {
            _currentSuspect = _allSuspects[0];
        }

        RefreshQuestions();
    }

    public void OpenJournalForSuspect(SuspectData suspect)
    {
        if (_root != null)
            _root.SetActive(true);

        var gsm = GameStateManager.Instance;
        if (gsm != null)
        {
            gsm.SetState(GameState.Journal);
        }

        _currentSuspect = suspect;
        RefreshQuestions();
    }

    public void CloseJournal()
    {
        if (_root != null)
            _root.SetActive(false);

        var gsm = GameStateManager.Instance;
        if (gsm != null && gsm.IsState(GameState.Journal))
        {
            gsm.SetState(GameState.FreeRoam);
        }
    }

    public void OnAccuseCurrentSuspect()
    {
        if (_currentSuspect == null)
            return;

        var am = AccusationManager.Instance;
        if (am == null)
            return;

        am.Accuse(_currentSuspect);
    }
}
