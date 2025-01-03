using UnityEngine;
using GoogleMobileAds.Api;
using System;


public class AdsManager : SingletonMonoBehaviour<AdsManager>
{
    private bool _isInitialized;
    public bool IsInitialized => _isInitialized;
    [SerializeField] private string _adUnitId;
    
    private RewardedAd _rewardedAd;

    public static event Action OnAdReady;
    
    private void Start()
    {
        MobileAds.Initialize(_ =>
        {
            _isInitialized = true;
            LoadRewardedAd();
        });
    }

    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        //Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                //Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                return;
            }

            //Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

            _rewardedAd = ad;
            OnAdReady?.Invoke();
            
            RegisterEventHandlers(_rewardedAd);
        });
    }
    
    public bool IsRewardedAdLoaded()
    {
        return _rewardedAd != null && _rewardedAd.CanShowAd();
    }
    
    public void ShowRewardedAd(Action callback)
    {
        //const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (IsRewardedAdLoaded())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                //Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                callback?.Invoke();
                EventController.Instance.AdWatched();
            });
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        /*ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };*/
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            //Debug.Log("Rewarded ad full screen content closed.");
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            //Debug.LogError("Rewarded ad failed to open full screen content with error : " + error);
            LoadRewardedAd();
        };
    }
}