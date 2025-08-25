using TMPro;
using UnityEngine;

public class TaskUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI detailText;
    public TextMeshProUGUI progressText;

    public void Setup(string title, string detail, string progress)
    {
        titleText.text = title;
        detailText.text = detail;
        progressText.text = progress;
    }
}
