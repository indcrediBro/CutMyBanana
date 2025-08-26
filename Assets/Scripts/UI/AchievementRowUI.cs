using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementRowUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    public Button showButton;

    private AchievementSO so;

    public void Setup(AchievementSO a)
    {
        so = a;
        titleText.text = a.title;
        descText.text = a.description;
        showButton.onClick.RemoveAllListeners();
        showButton.onClick.AddListener(() => {
            FindFirstObjectByType<NarrativePopup>()?.Show(a.title, a.description);
        });
    }
}