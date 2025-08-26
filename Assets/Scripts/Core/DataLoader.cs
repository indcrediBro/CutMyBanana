using UnityEngine;

public static class DataLoader
{
    public static T[] LoadAll<T>() where T : ScriptableObject
    {
        return Resources.LoadAll<T>("Data");
    }
}