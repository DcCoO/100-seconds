using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ninjas : MonoBehaviour
{
    private int _selectedSkinIndex;
    private int _currentSkinIndex;
    [SerializeField] private NinjaSettings[] _skins;

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _previewTransform;
    [SerializeField] private Transform _previewAnchor;
    [SerializeField] private Animator _previewAnimator;
    [SerializeField] private float _animatorSpeed;
    
    [SerializeField] private TMP_Text _textName;
    [SerializeField] private TMP_Text _textDescription;
    [SerializeField] private GameObject _imageEldest;

    // Locked Panel
    [SerializeField] private GameObject _lockedPanel;
    [SerializeField] private TMP_Text _textUnlocked;
    [SerializeField] private TMP_Text _textUnlockedAds;
    [SerializeField] private Button _buttonWatchAd;
    
    // Unlocked Panel
    [SerializeField] private GameObject _unlockedPanel;
    [SerializeField] private TMP_Text _textSelected;
    [SerializeField] private Button _buttonSelected;

    [SerializeField] private Button _left;
    [SerializeField] private Button _right;

    
    
    private void OnEnable()
    {
        var currentSkin = Menu.GetSelectedSkinID();
        _currentSkinIndex = Array.FindIndex(_skins, s => s.ID == currentSkin);
        _selectedSkinIndex = _currentSkinIndex;
        UpdateScreen(_selectedSkinIndex, _selectedSkinIndex);

        AdsManager.OnAdReady += OnAdLoaded;
    }

    private void OnDisable()
    {
        AdsManager.OnAdReady -= OnAdLoaded;
    }

    private void UpdateScreen(int currentSkin, int selectedSkin)
    {
        var skinToShow = _skins[currentSkin];
        
        NinjaSettings skin = _skins[currentSkin];
        
        bool available = Menu.IsSkinAvailable(skinToShow);
        bool selected = currentSkin == selectedSkin;
        _lockedPanel.SetActive(!available);
        _unlockedPanel.SetActive(available);
        
        if (available)
        {
            _textName.text = skinToShow.Name;
            _textDescription.text = skinToShow.Description;
            
            if (selected)
            {
                _textSelected.text = $"selected";
                _buttonSelected.interactable = false;
            }
            else
            {
                _textSelected.text = $"select";
                _buttonSelected.interactable = true;
            }
        }
        else
        {
            _textName.text = "?";
            _textDescription.text = string.Empty;
            
            var roundsLeft = Menu.GetRoundsLeft(skin.ID);
            var adsLeft = Menu.GetAdsLeft(skin.ID);
            _textUnlocked.text = $"unlock: {skin.TimeToUnlock} seconds {roundsLeft} {(roundsLeft == 1 ? "time" : "times")}";
            _textUnlockedAds.text = $"unlock: watch {adsLeft} {(adsLeft == 1 ? "ad" : "ads")}";
            
            // Handle Ads
            bool adLoaded = AdsManager.Instance.IsRewardedAdLoaded();
            _buttonWatchAd.interactable = adLoaded;
            if (!adLoaded) AdsManager.Instance.LoadRewardedAd();
        }
        
        // Preview
        _previewTransform.position = (Vector2) _mainCamera.ScreenToWorldPoint(_previewAnchor.position);
        _previewAnimator.SetTrigger(available ? skin.ID : "Unknown");
        _previewAnimator.speed = _animatorSpeed;
    }

    private void OnAdLoaded()
    {
        _buttonWatchAd.interactable = true;
    }
    
    public void PreviousSkin()
    {
        _currentSkinIndex--;
        if (_currentSkinIndex < 0) _currentSkinIndex = _skins.Length - 1;
        UpdateScreen(_currentSkinIndex, _selectedSkinIndex);
    }
    
    public void NextSkin()
    {
        _currentSkinIndex++;
        if (_currentSkinIndex >= _skins.Length) _currentSkinIndex = 0;
        UpdateScreen(_currentSkinIndex, _selectedSkinIndex);
    }
    
    public void SelectSkin()
    {
        var skin = _skins[_currentSkinIndex].ID;
        Menu.SetSelectedSkinID(skin);
        _textSelected.text = $"selected";
        _buttonSelected.interactable = false;
        _selectedSkinIndex = _currentSkinIndex;
        UpdateScreen(_currentSkinIndex, _selectedSkinIndex);
    }

    public void WatchAd()
    {
        _buttonWatchAd.interactable = false;
        AdsManager.Instance.ShowRewardedAd(ProgressAds);
    }
    
    private void ProgressAds()
    {
        var skin = _skins[_currentSkinIndex];
        var adsLeft = Menu.GetAdsLeft(skin.ID) - 1;
        Menu.SetAdsLeft(skin.ID, adsLeft);
        if (adsLeft <= 0) AchievementController.Instance.OnNinjaUnlocked(skin.ID);
        UpdateScreen(_currentSkinIndex, _selectedSkinIndex);
    }
}
