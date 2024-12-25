using UnityEngine;

[CreateAssetMenu(fileName = "Ninja Settings", menuName = "Scriptable Objects/Ninja Settings")]
public class NinjaSettings : ScriptableObject
{
    public string ID;
    public string Name;
    public string DescriptionLocKey;
    public int AdsToUnlock;
    public int TimeToUnlock;
    public int RoundsToUnlock;
    public Gradient TrailColor;
    public ESkill Skill;
    public int DodgesToChargeSkill;
    public AudioClip SelectSound;
}