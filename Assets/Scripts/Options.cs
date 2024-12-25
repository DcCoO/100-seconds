using UnityEngine;

public class Options : MonoBehaviour
{
    [SerializeField] private LocalizedText _musicLocText;
    [SerializeField] private LocalizedText _sfxLocText;

    [SerializeField] private GameObject _noAdsButton;

    private void OnEnable()
    {
        _musicLocText.ChangeKey(AudioManager.Instance.IsMusicOn() ? 1 : 0);
        _sfxLocText.ChangeKey(AudioManager.Instance.IsSFXOn() ? 1 : 0);
        _noAdsButton.SetActive(!Menu.GetNoAds());
    }

    public void ChangeMusicState()
    {
        bool isOn = !AudioManager.Instance.IsMusicOn();
        EventController.Instance.MusicStateChanged(isOn);
        _musicLocText.ChangeKey(isOn ? 1 : 0);
        
    }
    
    public void ChangeSFXState()
    {
        bool isOn = !AudioManager.Instance.IsSFXOn();
        EventController.Instance.SFXStateChanged(isOn);
        _sfxLocText.ChangeKey(isOn ? 1 : 0);
    }
}
