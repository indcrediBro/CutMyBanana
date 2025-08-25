using UnityEngine;
using DG.Tweening;

public class ColorCycler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color[] colors;
    [SerializeField] private float durationPerColor = 1f;
    [SerializeField] private bool loop = true;

    private int currentIndex = 0;
    private Tween colorTween;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (colors != null && colors.Length > 0)
            CycleToNextColor();
    }

    private void CycleToNextColor()
    {
        if (colors.Length == 0) return;

        Color targetColor = colors[currentIndex];
        colorTween = spriteRenderer.DOColor(targetColor, durationPerColor).OnComplete(() =>
        {
            currentIndex = (currentIndex + 1) % colors.Length;
            if (loop || currentIndex != 0) // if looping or not yet reached the end
                CycleToNextColor();
        });
    }

    private void OnDestroy()
    {
        colorTween?.Kill();
    }
}
