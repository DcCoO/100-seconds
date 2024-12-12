using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Settings", menuName = "Scriptable Objects/Spawn Settings")]
public class SpawnSettings : ScriptableObject
{
    public Enemy EnemyPrefab;
    public int MaxInstancesOnScreen;
    public AnimationCurve SpawnChance;
    [Range(0f, 1f)] public float[] ChanceAlongTime;

    public float GetSpawnChance(float seconds)
    {
        float t = seconds / 100f;
        return SpawnChance.Evaluate(t);
    }
}
