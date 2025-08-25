using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HoverToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CanvasGroup textGroup; // The text object with CanvasGroup
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private Transform shakeTarget;
    
    private Vector3 shakeTargetOriginalPos;
    private Tween fadeTween;

    private void Start()
    {
        if (textGroup != null)
            textGroup.alpha = 0; // Hidden initially

        shakeTargetOriginalPos = shakeTarget.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToggleText(true);
        Shake(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToggleText(false);
        Shake(false);
    }

    private void ToggleText(bool show)
    {
        if (textGroup == null) return;

        fadeTween?.Kill();
        fadeTween = textGroup.DOFade(show ? 1f : 0f, fadeDuration);
        textGroup.blocksRaycasts = show;
        textGroup.interactable = show;
    }
    
    private void Shake(bool activate)
    {
        if (activate)
        {
            shakeTarget.DOShakePosition(0.5f, new Vector3(5f, 5f, 0), 10, 90, false, true).SetLoops(-1);
        }
        else
        {
            shakeTarget.DOKill();
            shakeTarget.localPosition = shakeTargetOriginalPos; // Reset position
        }
    }
}
