using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievements", menuName = "Scriptable Objects/Achievement")]
public class Achievements : ScriptableObject
{
    
#if  UNITY_EDITOR
    public bool GenerateIDs;

    private void OnValidate()
    {
        if (GenerateIDs)
        {
            GenerateIDs = false;
            for (int i = 0; i < Entries.Length; i++)
            {
                Entries[i].Key = $"A{i}";
            }
        }
    }
    
    public AchievementEntry[] Entries;
    
#endif
    
    
}

[Serializable]
public class AchievementEntry
{
    public string Key;
    public string Description;
    public EAchievementType AchievementType;
    public string ExtraInformation;
    public int TargetProgress;
}

public enum EAchievementType
{
    LastSeconds,
    Win,
    WinWithNinja,
    UnlockNinja,
    Lose,
    WatchAds,
    Play,
    PlayWithNinja,
    UseShield
}
