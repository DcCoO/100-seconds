using System;
using TMPro;
using UnityEngine;

public class Menu : SingletonMonoBehaviour<Menu>
{
    public const string HighscoreKey = "Highscore";
    public const string SelectedSkin = "SelectedSkin";

    
    [SerializeField] private LocalizedText _highscoreLocText;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _logo;
    [SerializeField] private Transform _logoAnchor;
    [SerializeField] private GameObject _shieldPanel;
    
    [SerializeField] private NinjaSettings[] _skins;
    [SerializeField] private Animator _previewAnimator;

    [SerializeField] private FTUEPanel _ftuePanel;
    [SerializeField] private Transform _ftueParent;
    [SerializeField] private GameObject _game;
    [SerializeField] private GameObject _field;
    
    private void OnEnable()
    {
        _highscoreLocText.RefreshParameters(0, PlayerPrefs.GetInt(HighscoreKey, 0));
        if (AdsManager.Instance != null) _shieldPanel.SetActive(AdsManager.Instance.IsRewardedAdLoaded() && PlayerPrefs.GetInt(Player.ShieldKey, 0) == 0);

        _previewAnimator.SetTrigger(GetSelectedSkinID());
        
        EventController.OnGameLost += GameLost;
        EventController.OnGameWon += GameWon;
    }

    private void Start()
    {
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

    public void OnTapPlay()
    {
        if (GetFTUE())
        {
            Instantiate(_ftuePanel, _ftueParent).Setup(OnTapPlay);
        }
        else
        {
            gameObject.SetActive(false);
            _game.SetActive(true);
            _field.SetActive(true);
            EventController.Instance.StartGame();
        }
    }

    public void WatchAdForShield()
    {
        if (GetNoAds())
        {
            PlayerPrefs.SetInt(Player.ShieldKey, 1);
            _shieldPanel.SetActive(false);
            return;
        }
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

    private bool GetFTUE()
    {
        return PlayerPrefs.GetInt("FTUE", 1) == 1;
    }

    public static void SetFTUE()
    {
        PlayerPrefs.SetInt("FTUE", 0);
    }


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
        if (GetNoAds()) return true;
        if (GetRoundsLeft(skin.ID) <= 0) return true;
        if (GetAdsLeft(skin.ID) <= 0) return true;
        return false;
    }

    public static bool GetNoAds()
    {
        return PlayerPrefs.GetInt("NoAds", 0) == 1;
    }

    public static void SetNoAds(bool state)
    {
        PlayerPrefs.SetInt("NoAds", state ? 1 : 0);
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
