using System;
using TMPro;
using UnityEngine;

public class Menu : SingletonMonoBehaviour<Menu>
{
    public const string HighscoreKey = "Highscore";
    public const string SelectedSkin = "SelectedSkin";

    
    [SerializeField] private TMP_Text _highscoreText;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _logo;
    [SerializeField] private Transform _logoAnchor;
    [SerializeField] private GameObject _shieldPanel;

    [SerializeField] private AudioSource _audioSource;
    
    [SerializeField] private NinjaSettings[] _skins;
    [SerializeField] private Animator _previewAnimator;
    
    private void OnEnable()
    {
        _highscoreText.text = $"Highscore: {PlayerPrefs.GetInt(HighscoreKey, 0)}";
        if (AdsManager.Instance != null) _shieldPanel.SetActive(AdsManager.Instance.IsRewardedAdLoaded() && PlayerPrefs.GetInt(Player.ShieldKey, 0) == 0);

        _previewAnimator.SetTrigger(GetSelectedSkinID());
        
        EventController.OnGameLost += GameLost;
        EventController.OnGameWon += GameWon;
    }

    private void Start()
    {
        _audioSource.mute = PlayerPrefs.GetInt("MusicKey", 1) == 0;
        SetupCamera();
        _logo.position = (Vector2) _mainCamera.ScreenToWorldPoint(_logoAnchor.position);
        InitPlayerPrefs();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventController.OnGameLost -= GameLost;
        EventController.OnGameWon -= GameWon;
    }

    private void SetupCamera()
    {
        // Size of player in world units is 0.32
        float orthographicSize = _mainCamera.orthographicSize;
        float aspect = _mainCamera.aspect;
        
        // The height of the screen in world units must be at least 3.2
        var osHeight = 1.6f;
        
        // The width of the screen in world units must be at least 1.76
        var heightForWidth = 1.76f / aspect;
        var osWidth = heightForWidth / 2;

        _mainCamera.orthographicSize = Mathf.Max(orthographicSize, Mathf.Max(osHeight, osWidth));
    }
    
    //TODO: remove this
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            UnlockAllSkins();
        }

        if (Input.touchCount == 3)
        {
            UnlockAllSkins();
        }
    }



    public void WatchAdForShield()
    {
        AdsManager.Instance.ShowRewardedAd(() =>
        {
            PlayerPrefs.SetInt(Player.ShieldKey, 1);
            _shieldPanel.SetActive(false);
        });
    }

    private void GameLost(int seconds, float exactTime) => UpdateSkinProgress(seconds);

    private void GameWon() => UpdateSkinProgress(100);

    private void UpdateSkinProgress(int seconds)
    {
        foreach (var skin in _skins)
        {
            if (seconds < skin.TimeToUnlock) continue;
            var currentRoundsLeft = GetRoundsLeft(skin.ID);
            SetRoundsLeft(skin.ID, currentRoundsLeft - 1);
        }
    }


    public NinjaSettings GetSelectedSkin()
    {
        var skinID = PlayerPrefs.GetString(SelectedSkin);
        return Array.Find(_skins, s => s.ID == skinID);
    }
    
#region Player Prefs

    // Each ninja has to store the rounds left

    private void InitPlayerPrefs()
    {
        foreach (var skin in _skins)
        {
            if (!PlayerPrefs.HasKey($"RoundsLeft_{skin.ID}"))
            {
                SetRoundsLeft(skin.ID, skin.RoundsToUnlock);
                PlayerPrefs.SetInt($"RoundsLeft_{skin.ID}", skin.RoundsToUnlock);
                PlayerPrefs.SetInt($"AdsLeft_{skin.ID}", skin.AdsToUnlock);
            } 
        }
        
        if (PlayerPrefs.HasKey(SelectedSkin)) return;
        PlayerPrefs.SetString(SelectedSkin, _skins[0].ID);
    }
    
    public static string GetSelectedSkinID()
    {
        return PlayerPrefs.GetString(SelectedSkin, Instance._skins[0].ID);
    }
    
    public static void SetSelectedSkinID(string skin)
    {
        PlayerPrefs.SetString(SelectedSkin, skin);
    }
    
    // To unlock a skin, you need to last a certain amount of time for a certain number of rounds
    // The initial (duration, rounds) are defined in the NinjaSettings, but the rounds left are stored in PlayerPrefs
    public static int GetRoundsLeft(string skin) => PlayerPrefs.GetInt($"RoundsLeft_{skin}");
    private static void SetRoundsLeft(string skin, int rounds) => PlayerPrefs.SetInt($"RoundsLeft_{skin}", rounds);
    
    public static int GetAdsLeft(string skin) => PlayerPrefs.GetInt($"AdsLeft_{skin}");
    public static void SetAdsLeft(string skin, int rounds) => PlayerPrefs.SetInt($"AdsLeft_{skin}", rounds);

    public static bool IsSkinAvailable(NinjaSettings skin)
    {
        if (GetRoundsLeft(skin.ID) <= 0) return true;
        if (GetAdsLeft(skin.ID) <= 0) return true;
        return false;
    }
    

    
    public void UnlockAllSkins()
    {
        foreach (var skin in _skins)
        {
            SetRoundsLeft(skin.ID, 0);
            SetAdsLeft(skin.ID, 0);
        }
    }


    
#endregion
}
