using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
/// <summary>
/// PopupManager: instantiate a prefab popup. Provide your own prefab with fields:
/// - "Title" (TMP), "Message" (TMP), "OkButton" (Button)
/// </summary>
public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }
    public GameObject popup; // assign small popup prefab (panel with title, message, ok button)
    public TMP_Text title, message;
public Button okButton,closeButton;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Show(string _title, string _message, Action onConfirm = null)
    {
        if (popup == null)
        {
            Debug.Log("[PopupManager] popupPrefab not assigned. Falling back to Debug.Log:");
            Debug.Log($"{title}: {message}");
            onConfirm?.Invoke();
            return;
        }

        popup.SetActive(true);
        if (title!=null) title.text = _title;
        if (message!=null) message.text = _message;
        if (okButton!=null)
        {
            okButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke();
                popup.SetActive(false);
            });
        }
        if (closeButton!=null)
        {
            closeButton.onClick.AddListener(() =>
            {
                popup.SetActive(false);
            });
        }
    }

    // Utility confirmation popup
    public void Confirm(string _title, string _message, Action onYes, Action onNo = null)
    {
        if (popup == null)
        {
            onYes?.Invoke();
            return;
        }
        popup.SetActive(true);

        if (title!=null) title.text = _title;
        if (message!=null) message.text = _message;
        if (okButton!=null)
        {
            okButton.onClick.AddListener(() =>
            {
                onYes?.Invoke();
                popup.SetActive(false);
            });
        }
        if (closeButton!=null)
        {
            closeButton.onClick.AddListener(() =>
            {
                onNo?.Invoke();
                popup.SetActive(false);
            });
        }
    }
}
