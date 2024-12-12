using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private TMP_Text _musicText;

    [SerializeField] private Image[] _skins;
    [SerializeField] private SpriteRenderer[] _renderersToApplySkin;
    
    public void ToggleMusic()
    {
        _audioSource.mute = !_audioSource.mute;
        _musicText.text = _audioSource.mute ? "music in off" : "music is on";
        PlayerPrefs.SetInt("MusicKey", _audioSource.mute ? 0 : 1);
    }
    
    public void ApplySkin(int skinIndex)
    {
        foreach (var spriteRenderer in _renderersToApplySkin)
        {
            spriteRenderer.color = _skins[skinIndex].color;
        }
    }
}
