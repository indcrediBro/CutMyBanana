using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Button))]
public class UpgradeUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI levelText;
    public Button buyButton;

    private string upgradeId;
    private UpgradeSO so;
    private UpgradeManager upgradeManager;

    public void Setup(UpgradeSO up)
    {
        so = up;
        upgradeId = up.id;
        titleText.text = up.title;
        descText.text = up.description;
        buyButton = buyButton ?? GetComponent<Button>();
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyClicked);

        upgradeManager = FindFirstObjectByType<UpgradeManager>();
        Refresh();
    }

    public void Refresh()
    {
        var saved = SaveSystem.Load() ?? new SaveData();
        var st = saved.upgrades.FirstOrDefault(x => x.id == upgradeId);
        int lvl = st?.level ?? 0;
        levelText.text = $"Lv {lvl}";
        long cost = upgradeManager.CostForNextLevel(upgradeId);
        costText.text = $"{cost} 🍌";
        buyButton.interactable = upgradeManager.IsUpgradeUnlocked(upgradeId) && upgradeManager.CanPurchase(upgradeId);
    }

    private void OnBuyClicked()
    {
        if (upgradeManager.Purchase(upgradeId))
        {
            Refresh();
            // notify parent controller to refresh lists
            var main = FindFirstObjectByType<UIController>();
            main?.RefreshAll();
        }
    }
}