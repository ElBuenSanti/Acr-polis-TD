using System.Collections.Generic;
using UnityEngine;

public enum Wave
{
    Tutorial,
    Standard,
    Breach
}

[System.Serializable]
public class EnemyWaveEntry
{
    public EnemyData enemy;
    public float porcentageInWave;
}

[CreateAssetMenu(menuName = "Wave/Data")]
public class WaveData : ScriptableObject
{
    public int waveIndex;
    public Wave waveType;
    public float waveDuration;
    public float spawnFrequency;
    public List<EnemyWaveEntry> enemies;

}