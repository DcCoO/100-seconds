using TMPro;
using UnityEngine;

public class Options : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private TMP_Text _musicText;


    private void OnEnable()
    {
        var mute = PlayerPrefs.GetInt("MusicKey", 1) == 0;
        _musicText.text = mute ? "music in off" : "music is on";
    }

    public void ToggleMusic()
    {
        _audioSource.mute = !_audioSource.mute;
        _musicText.text = _audioSource.mute ? "music in off" : "music is on";
        PlayerPrefs.SetInt("MusicKey", _audioSource.mute ? 0 : 1);
    }
}
