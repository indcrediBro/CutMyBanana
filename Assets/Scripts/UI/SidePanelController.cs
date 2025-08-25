using UnityEngine;
using DG.Tweening;

public class SidePanelController : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private Vector2 hiddenPosition; // Off-screen position
    [SerializeField] private Vector2 shownPosition;  // On-screen position
    [SerializeField] private GameObject invisibleClosePanelButton;
    [SerializeField] private GameObject tasksPanel, upgradesPanel;
    private bool isVisible = false;
    private Tween panelTween;

    private void Start()
    {
        panel.anchoredPosition = hiddenPosition;
        invisibleClosePanelButton.SetActive(isVisible);
    }

    public void OpenPanel(int index)
    {
        if (index == 0)
        {
            tasksPanel.SetActive(true);
            upgradesPanel.SetActive(false);
        }
        else if (index == 1)
        {
            tasksPanel.SetActive(false);
            upgradesPanel.SetActive(true);
        }
        ActivatePanel();
    }
    
    public void ActivatePanel()
    {
        if (!isVisible)
        {
            isVisible = true;
            invisibleClosePanelButton.SetActive(isVisible);
            panelTween?.Kill();
            panelTween = panel.DOAnchorPos(isVisible ? shownPosition : hiddenPosition, slideDuration)
                .SetEase(Ease.OutCubic);
        }
    }
    
    public void DeactivatePanel()
    {
        if (isVisible)
        {
            isVisible = false;
            invisibleClosePanelButton.SetActive(isVisible);
            panelTween?.Kill();
            panelTween = panel.DOAnchorPos(isVisible ? shownPosition : hiddenPosition, slideDuration)
                .SetEase(Ease.OutCubic);
        }
    }
}