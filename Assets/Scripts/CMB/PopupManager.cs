using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CMB
{
    [Serializable]
    public class PopupManager
    {
        public GameObject popup; // assign small popup prefab (panel with title, message, ok button)
        public TMP_Text title, message;
        public Button okButton,closeButton;

        public void Show(string _title, string _message, Action onConfirm = null)
        {
            if (popup == null)
            {
                Debug.Log("[PopupManager] popupPrefab not assigned. Falling back to Debug.Log:");
                Debug.Log($"{title}: {message}");
                onConfirm?.Invoke();
                return;
            }

            Time.timeScale = 0;
            popup.SetActive(true);
            if (title!=null) title.text = _title;
            if (message!=null) message.text = _message;
            if (okButton!=null)
            {
                okButton.onClick.AddListener(() =>
                {
                    onConfirm?.Invoke();
                    popup.SetActive(false);
                    Time.timeScale = 1;
                });
            }
            if (closeButton!=null)
            {
                closeButton.onClick.AddListener(() =>
                {
                    popup.SetActive(false);
                    Time.timeScale = 1;
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
            Time.timeScale = 0;

            if (title!=null) title.text = _title;
            if (message!=null) message.text = _message;
            if (okButton!=null)
            {
                okButton.onClick.AddListener(() =>
                {
                    onYes?.Invoke();
                    popup.SetActive(false);
                    Time.timeScale = 1;
                });
            }
            if (closeButton!=null)
            {
                closeButton.onClick.AddListener(() =>
                {
                    onNo?.Invoke();
                    popup.SetActive(false);
                    Time.timeScale = 1;
                });
            }
        }
    }
}