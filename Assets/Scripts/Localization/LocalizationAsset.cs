using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Localization Asset", menuName = "Scriptable Objects/Localization Asset")]
public class LocalizationAsset : ScriptableObject
{
    public SerializedDictionary<string, SerializedDictionary<SystemLanguage, string>> Data;
}
