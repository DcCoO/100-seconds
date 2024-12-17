using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementItem : MonoBehaviour
{
    [SerializeField] private Image _imageCheckmark;
    [SerializeField] private TMP_Text _textDescription;
    [SerializeField] private TMP_Text _textProgress;
    
    public void Init(Sprite checkmark, string description, string progress)
    {
        _imageCheckmark.sprite = checkmark;
        _textDescription.text = description;
        _textProgress.text = progress;
    }
}
