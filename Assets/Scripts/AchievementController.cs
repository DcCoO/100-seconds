using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementController : SingletonMonoBehaviour<AchievementController>
{

    [SerializeField] private Achievements _achievements;
    private Dictionary<EAchievementType, List<AchievementEntry>> _achievementDictionary;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    private void OnEnable()
    {
        EventController.OnGameStart += OnGameStart;
        EventController.OnGameWon += OnGameWon;
        EventController.OnGameLost += OnGameLost;
        EventController.OnAdWatched += OnAdWatched;
        EventController.OnDodge += OnDodge;
        EventController.OnSkillUsed += OnSkillUsed;
    }

    private void OnDisable()
    {
        EventController.OnGameStart -= OnGameStart;
        EventController.OnGameWon -= OnGameWon;
        EventController.OnGameLost -= OnGameLost;
        EventController.OnAdWatched -= OnAdWatched;
        EventController.OnDodge -= OnDodge;
        EventController.OnSkillUsed -= OnSkillUsed;
    }

    private void Initialize()
    {
        _achievementDictionary = new Dictionary<EAchievementType, List<AchievementEntry>>();
        foreach (var entry in _achievements.Entries)
        {
            if (!_achievementDictionary.ContainsKey(entry.AchievementType))
            {
                _achievementDictionary[entry.AchievementType] = new List<AchievementEntry>();
            }
            _achievementDictionary[entry.AchievementType].Add(entry);
        }
    }

    private void OnGameStart()
    {
        UpdateAchievementProgress(EAchievementType.Play, 1);
        UpdateAchievementProgress(EAchievementType.PlayWithNinja, 1, Menu.GetSelectedSkinID());
        UpdateAchievementProgress(EAchievementType.UseShield, Player.Instance.GetShield());
    }

    private void OnGameWon()
    {
        UpdateAchievementProgress(EAchievementType.Win, 1);
        UpdateAchievementProgress(EAchievementType.WinWithNinja, 1, Menu.GetSelectedSkinID());
        UpdateAchievementProgress(EAchievementType.LastSeconds, 100, true);
    }
    
    private void OnGameLost(int seconds, float exactTime)
    {
        UpdateAchievementProgress(EAchievementType.Lose, 1);
        UpdateAchievementProgress(EAchievementType.LastSeconds, seconds, true);
    }

    public void OnNinjaUnlocked(string ninjaID)
    {
        UpdateAchievementProgress(EAchievementType.UnlockNinja, 1, ninjaID);
    }

    private void OnAdWatched()
    {
        UpdateAchievementProgress(EAchievementType.WatchAds, 1);
    }
    
    private void OnDodge(int combo)
    {
        UpdateAchievementProgress(EAchievementType.Dodge, 1);
        UpdateAchievementProgress(EAchievementType.DodgeCombo, combo, true);
    }
    
    private void OnSkillUsed()
    {
        UpdateAchievementProgress(EAchievementType.UseSkill, 1);
    }





    private void UpdateAchievementProgress(EAchievementType type, int progress, bool compare = false)
    {
        UpdateAchievementProgress(type, progress, string.Empty, compare);
    }
    
    private void UpdateAchievementProgress(EAchievementType type, int progress, string extraInformation, bool compare = false)
    {
        var achievementByType = _achievementDictionary[type];
        foreach (var achievement in achievementByType)
        {
            if (achievement.ExtraInformation != extraInformation) continue;

            if (compare)    // If we need to compare the progress with the TargetProgress instead of incrementing it
            {
                if (progress >= achievement.TargetProgress)
                {
                    PlayerPrefs.SetInt(achievement.Key, achievement.TargetProgress);
                }
            }
            else
            {
                int currentProgress = PlayerPrefs.GetInt(achievement.Key, 0);
                PlayerPrefs.SetInt(achievement.Key, currentProgress + progress);
            }
        }
    }
}
