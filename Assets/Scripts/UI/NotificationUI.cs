// NotificationUI v1

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private Text _messageText;
    [SerializeField] private float _defaultDuration = 2f;

    private Coroutine _activeRoutine;

    private static NotificationUI _instance;
    public static NotificationUI Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("NotificationUI: No instance in scene.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("NotificationUI: Multiple instances detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        _instance = this;

        if (_root != null)
            _root.SetActive(false);
    }

    public void ShowMessage(string message, float duration = -1f)
    {
        if (_messageText == null || _root == null)
            return;

        if (duration <= 0f)
            duration = _defaultDuration;

        _messageText.text = message;
        _root.SetActive(true);

        if (_activeRoutine != null)
            StopCoroutine(_activeRoutine);

        _activeRoutine = StartCoroutine(HideAfterDelay(duration));
    }

    private IEnumerator HideAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (_root != null)
            _root.SetActive(false);
        _activeRoutine = null;
    }
}