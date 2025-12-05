using UnityEngine;
using UnityEngine.UI;

public class AccusationUI : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private Text _titleText;
    [SerializeField] private Text _bodyText;

    private SuspectData _currentSuspect;
    private bool _lastResultCorrect;

    private void Start()
    {
        if (_root != null)
            _root.SetActive(false);

        var am = AccusationManager.Instance;
        if (am != null)
        {
            am.OnAccusationResult += HandleAccusationResult;
        }
    }

    private void OnDestroy()
    {
        var am = AccusationManager.Instance;
        if (am != null)
        {
            am.OnAccusationResult -= HandleAccusationResult;
        }
    }

    private void HandleAccusationResult(bool correct, SuspectData suspect)
    {
        _currentSuspect = suspect;
        _lastResultCorrect = correct;

        if (_root != null)
            _root.SetActive(true);

        if (_titleText != null)
        {
            _titleText.text = correct ? "You solved the case!" : "You accused the wrong person.";
        }

        if (_bodyText != null)
        {
            if (suspect != null)
            {
                _bodyText.text = correct
                    ? $"You correctly identified {suspect.DisplayName} as the culprit."
                    : $"{suspect.DisplayName} was not the real culprit.";
            }
            else
            {
                _bodyText.text = correct
                    ? "You solved the case."
                    : "That was not the real culprit.";
            }
        }
    }

    public void OnClose()
    {
        if (_root != null)
            _root.SetActive(false);

        var gsm = GameStateManager.Instance;
        if (gsm != null)
        {
            gsm.SetState(GameState.FreeRoam);
        }
    }
}