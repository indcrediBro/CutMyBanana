using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button buyButton;

    public void Setup(string title, string desc, int cost, int level)
    {
        titleText.text = title;
        descText.text = desc;
        costText.text = cost.ToString();
        levelText.text = level.ToString();
        buyButton.onClick.AddListener(OnBuyClick);
    }

    private void OnBuyClick()
    {

    }
}