using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private Achievements _achievements;
    [SerializeField] private AchievementItem _achievementPrefab;
    [SerializeField] private Transform _content;

    [SerializeField] private Sprite _incompleteSprite;
    [SerializeField] private Sprite _completeSprite;

    [SerializeField] private ScrollRect _scrollRect;

    private void OnEnable()
    {
        foreach (var achievement in _achievements.Entries)
        {
            var achievementItem = Instantiate(_achievementPrefab, _content);
            var progress = PlayerPrefs.GetInt(achievement.Key, 0);
            var completed = progress >= achievement.TargetProgress;
            if (completed)
            {
                achievementItem.Init(_completeSprite, achievement.Description, $"{achievement.TargetProgress}/{achievement.TargetProgress}");
            }
            else
            {
                achievementItem.Init(_incompleteSprite, achievement.Description, $"{progress}/{achievement.TargetProgress}");
            }
        }
        
        _scrollRect.verticalNormalizedPosition = 1;
    }

    private void OnDisable()
    {
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }
    }
}
