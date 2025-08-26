using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NarrativePopup : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Button closeButton;

    private void Awake()
    {
        hide();
        closeButton.onClick.AddListener(() => hide());
    }

    public void Show(string title, string body)
    {
        gameObject.SetActive(true);
        titleText.text = title;
        bodyText.text = body;
    }

    public void hide()
    {
        gameObject.SetActive(false);
    }
}