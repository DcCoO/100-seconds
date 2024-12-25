using System.Linq;
using UnityEngine;

public class LocalizationManager : SingletonMonoBehaviour<LocalizationManager>
{
    [SerializeField] private LocalizationAsset _asset;
    private SystemLanguage _currentLanguage;
    [SerializeField] private SystemLanguage[] _supportedLanguages;

#if UNITY_EDITOR
    private static LocalizationManager _editorInstance => FindObjectOfType<LocalizationManager>();
#endif
    
    protected override void Awake()
    {
        base.Awake();
        _currentLanguage = Application.systemLanguage;
        if (!_supportedLanguages.Contains(_currentLanguage))
        {
            _currentLanguage = SystemLanguage.English;
        }
    }

    public string GetLocalizedText(string key)
    {
        return _asset.Data[key][_currentLanguage].Replace('\\', '\n');
    }
    
    public string GetLocalizedTextWithParameters(string key, params object[] parameters)
    {
        var format = _asset.Data[key][_currentLanguage];
        return string.Format(format, parameters).Replace('\\', '\n');
    }

#if UNITY_EDITOR
    public static string[] AllKeys()
    {
        return _editorInstance._asset.Data.Keys.ToArray();
    }
#endif
}
