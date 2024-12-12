using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private TMP_Text _highscoreText;
    public const string HighscoreKey = "Highscore";

    [SerializeField] private Transform _logo;
    [SerializeField] private Transform _logoAnchor;
    [SerializeField] private GameObject _shieldPanel;

    [SerializeField] private AudioSource _audioSource;
    
    private void OnEnable()
    {
        _highscoreText.text = $"Highscore: {PlayerPrefs.GetInt(HighscoreKey, 0)}";
        _logo.position = (Vector2) Camera.main.ScreenToWorldPoint(_logoAnchor.position);
        if (AdsManager.Instance != null) _shieldPanel.SetActive(AdsManager.Instance.IsRewardedAdLoaded() && PlayerPrefs.GetInt(Player.ShieldKey, 0) == 0);
    }

    private void Start()
    {
        _audioSource.mute = PlayerPrefs.GetInt("MusicKey", 1) == 0;
    }


    public void WatchAdForShield()
    {
        AdsManager.Instance.ShowRewardedAd(() =>
        {
            PlayerPrefs.SetInt(Player.ShieldKey, 1);
            _shieldPanel.SetActive(false);
        });
    }
}
