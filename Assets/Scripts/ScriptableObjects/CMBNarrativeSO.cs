using UnityEngine;

[CreateAssetMenu(menuName = "CMB/Narrative")]
public class CMBNarrativeSO : ScriptableObject
{
    public string id;
    public string title;
    [TextArea(4, 12)] public string body;
}