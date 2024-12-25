using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string[] _keys;
    [SerializeField] private TMP_Text _text;
    
    [Tooltip(
@"If true, this LocalizationText won't be initialized in Start() because one of the following reasons:
    - It needs parameters (read from PlayerPrefs, for example)
    - It needs to be initialized in another script that will provide the parameters")]
    [SerializeField] private bool _hasParameters;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_text == null) _text = GetComponent<TMP_Text>();
    }
#endif

    private void Start()
    {
        if (!_hasParameters) _text.text = LocalizationManager.Instance.GetLocalizedText(_keys[0]);
    }
    
    public void ChangeKey(int keyIndex)
    {
        _text.text = LocalizationManager.Instance.GetLocalizedText(_keys[keyIndex]);
    }
    
    public void RefreshParameters(int keyIndex, params object[] parameters)
    {
        //print($"key index: {keyIndex}, parameters: {parameters.Length} => {parameters[0]}");
        _text.text = LocalizationManager.Instance.GetLocalizedTextWithParameters(_keys[keyIndex], parameters);
    }
}