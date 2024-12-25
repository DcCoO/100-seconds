using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource[] _sfxSource;

    private bool _sfxMute;
    private int _sfxIndex;

    [Header("SOUNDS")]
    [SerializeField] private AudioClip _teleport;
    [SerializeField] private AudioClip _becomeMouse;
    [SerializeField] private AudioClip _becomeHuman;
    [SerializeField] private AudioClip _crossWall;
    [SerializeField] private AudioClip _disablerSkill;
    [SerializeField] private AudioClip _shieldOn;
    [SerializeField] private AudioClip _shieldOff;
    [SerializeField] private AudioClip _dodge;
    [SerializeField] private AudioClip[] _die;
    [SerializeField] private AudioClip _win;
    [SerializeField] private AudioClip _eldest;

    private void OnEnable()
    {
        EventController.OnMusicStateChanged += ToggleMusic;
        EventController.OnSFXStateChanged += ToggleSFX;
    }
    
    private void OnDisable()
    {
        EventController.OnMusicStateChanged -= ToggleMusic;
        EventController.OnSFXStateChanged -= ToggleSFX;
    }

    private void Start()
    {
        _musicSource.mute = !IsMusicOn();
        _sfxMute = !IsSFXOn();
    }

    #region Play Sounds
    public void PlayTeleport() => PlaySFX(_teleport);
    public void PlayBecomeMouse() => PlaySFX(_becomeMouse);
    public void PlayBecomeHuman() => PlaySFX(_becomeHuman);
    public void PlayBoundlessSkill() => PlaySFX(_crossWall);
    public void PlayDisablerSkill() => PlaySFX(_disablerSkill);
    public void PlayShieldOn() => PlaySFX(_shieldOn);
    public void PlayShieldOff() => PlaySFX(_shieldOff);
    public void PlayDodge() => PlaySFX(_dodge);
    public void PlayDie() => PlaySFX(_die[UnityEngine.Random.Range(0, _die.Length)]);
    public void PlayWin() => PlaySFX(_win);
    public void PlayEldest() => PlaySFX(_eldest);
    #endregion
    
    
    public void PlaySFX(AudioClip clip)
    {
        if (_sfxMute) return;
        _sfxSource[_sfxIndex].clip = clip;
        _sfxSource[_sfxIndex].Play();
        _sfxIndex = (_sfxIndex + 1) % _sfxSource.Length;
    }

    public void StopAllSounds()
    {
        foreach (var source in _sfxSource) source.Stop();
    }
    
    public bool IsMusicOn()
    {
        return PlayerPrefs.GetInt("MusicKey", 1) == 1;
    }
    
    public bool IsSFXOn()
    {
        return PlayerPrefs.GetInt("SFXKey", 1) == 1;
    }
    
    private void ToggleMusic(bool state)
    {
        PlayerPrefs.SetInt("MusicKey", state ? 1 : 0);
        _musicSource.mute = !state;
        if (state) _musicSource.Play();
        else _musicSource.Stop();
    }
    
    private void ToggleSFX(bool state)
    {
        PlayerPrefs.SetInt("SFXKey", state ? 1 : 0);
        _sfxMute = !state;
    }
}
