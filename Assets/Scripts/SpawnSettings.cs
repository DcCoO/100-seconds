using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Settings", menuName = "Scriptable Objects/Spawn Settings")]
public class SpawnSettings : ScriptableObject
{
    public Enemy EnemyPrefab;
    public int MaxInstancesOnScreen;
    [Range(0f, 1f)] public float[] ChanceAlongTime;
    
    public float GetSpawnChanceAtSeconds(float seconds)
    {
        return ChanceAlongTime[Mathf.FloorToInt(seconds)];
    }
}
