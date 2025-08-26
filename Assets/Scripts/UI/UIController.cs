using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public CMBBootstrap bootstrap;

    [Header("HUD")]
    public TMPro.TextMeshProUGUI currencyText;
    public TMPro.TextMeshProUGUI cpsText;
    public TMPro.TextMeshProUGUI slicesText;

    [Header("Tasks List")]
    public Transform tasksParent;
    public GameObject taskRowPrefab;

    [Header("Upgrades List")]
    public Transform upgradesParent;
    public GameObject upgradeRowPrefab;

    [Header("Achievements List")]
    public Transform achParent;
    public GameObject achRowPrefab;

    [Header("Narrative Popup")]
    public NarrativePopup narrativePopupPrefab;

    private void Start()
    {
        bootstrap = bootstrap ?? FindFirstObjectByType<CMBBootstrap>();
        bootstrap.gameState.OnStateChanged += RefreshHUD;
        bootstrap.taskManager.OnTaskCompleted += (d,s) => { RefreshAll(); };
        bootstrap.upgradeManager.OnUpgradePurchased += (u,l) => { RefreshAll(); };
        bootstrap.achievementManager.Initialize(); // ensure loaded
        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshHUD();
        PopulateTasks();
        PopulateUpgrades();
        PopulateAchievements();
    }

    public void RefreshHUD()
    {
        var s = SaveSystem.Load() ?? new SaveData();
        currencyText.text = $"Bananas: {s.currency:0}";
        cpsText.text = $"Idle: {s.cps:0.##}/s";
        slicesText.text = $"Sliced: {s.totalSlices:0}";
    }

    private void PopulateTasks()
    {
        foreach (Transform ch in tasksParent) Destroy(ch.gameObject);
        int tier = bootstrap.tierManager.GetCurrentTier();
        foreach (var pair in bootstrap.taskManager.GetActiveTaskStates(tier))
        {
            var go = Instantiate(taskRowPrefab, tasksParent);
            var row = go.GetComponent<TaskUI>();
            row.Setup(pair.def, pair.state);
        }
    }

    private void PopulateUpgrades()
    {
        foreach (Transform ch in upgradesParent) Destroy(ch.gameObject);
        int tier = bootstrap.tierManager.GetCurrentTier();
        var all = bootstrap.upgradeManager.GetUpgradesForTier(tier).ToList();
        foreach (var up in all)
        {
            // Only show upgrades that are unlocked by tier or achievement
            if (!bootstrap.upgradeManager.IsUpgradeUnlocked(up.id)) continue;
            var go = Instantiate(upgradeRowPrefab, upgradesParent);
            var row = go.GetComponent<UpgradeUI>();
            row.Setup(up);
        }
    }

    private void PopulateAchievements()
    {
        foreach (Transform ch in achParent) Destroy(ch.gameObject);
        var all = Resources.LoadAll<AchievementSO>("Data");
        foreach (var a in all)
        {
            var go = Instantiate(achRowPrefab, achParent);
            var row = go.GetComponent<AchievementRowUI>();
            row.Setup(a);
        }
    }

    // UI hooks
    public void OpenNarrative(string id)
    {
        var all = Resources.LoadAll<NarrativeSO>("Data");
        var n = all.FirstOrDefault(x => x.id == id);
        if (n != null) FindFirstObjectByType<NarrativePopup>()?.Show(n.title, n.body);
    }

    public void ResetSave()
    {
        PopupManager.Instance.Confirm("Reset Progress", "Are you sure you want to reset all progress?", () =>
        {
            SaveSystem.Clear();
            //UnityEditor.EditorApplication.isPlaying = false; // stops play mode for quick refresh in editor
        }, () => { });
    }
}
