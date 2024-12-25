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

    private int _originalFPS;
    private void OnEnable()
    {
        _originalFPS = Application.targetFrameRate;
        Application.targetFrameRate = 60;
        
        foreach (var achievement in _achievements.Entries)
        {
            var achievementItem = Instantiate(_achievementPrefab, _content);
            var progress = PlayerPrefs.GetInt(achievement.Key, 0);
            var completed = progress >= achievement.TargetProgress;
            var description = LocalizationManager.Instance.GetLocalizedText(achievement.DescriptionLocKey);
            if (completed)
            {
                achievementItem.Init(_completeSprite, description, $"{achievement.TargetProgress}/{achievement.TargetProgress}");
            }
            else
            {
                achievementItem.Init(_incompleteSprite, description, $"{progress}/{achievement.TargetProgress}");
            }
        }
        
        _scrollRect.verticalNormalizedPosition = 1;
    }

    private void OnDisable()
    {
        Application.targetFrameRate = _originalFPS;
        
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }
    }
}
