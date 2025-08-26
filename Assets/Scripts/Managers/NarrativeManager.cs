using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// NarrativeManager: plays narratives via popups
/// </summary>
public class NarrativeManager : MonoBehaviour
{
    public List<NarrativeSO> all;
    Dictionary<string, NarrativeSO> lookup = new Dictionary<string, NarrativeSO>();

    public void Initialize()
    {
        lookup = all.ToDictionary(n => n.id, n => n);
    }

    public void PlayNarrative(string id)
    {
        if (lookup == null) Initialize();
        if (!lookup.ContainsKey(id)) return;
        var n = lookup[id];
        PopupManager.Instance.Show(n.title, n.body);
    }
}