using System.Collections.Generic;
using UnityEngine;

public enum Wave
{
    Tutorial,
    Standard,
    Breach,
    FinalBattle
}

[System.Serializable]
public class EnemyWaveEntry
{
    public EnemyData enemy;
    public float percentageInWave;
}

[CreateAssetMenu(menuName = "Wave/Data")]
public class WaveData : ScriptableObject
{
    public int waveIndex;
    public Wave waveType;
    public float waveDuration;
    public float spawnFrequency;
    public List<EnemyWaveEntry> enemies;
    public List<EnemyData> possibleBosses;

}