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

[System.Serializable]
public class BossEntry
{
    public GodType god;
    public EnemyData boss;
}


//All atributes that EVERY wave has
[CreateAssetMenu(menuName = "Wave/Data")]
public class WaveData : ScriptableObject
{
    public int waveIndex;
    public Wave waveType;
    public float waveDuration;
    public float spawnFrequency;
    public List<EnemyWaveEntry> enemies;
    public List<BossEntry> possibleBosses;


    //Extracts boss depending on the selected god for the temple
    public EnemyData GetBossForGod(GodType god)
    {
        foreach (var entry in possibleBosses)
        {
            if (entry.god == god)
                return entry.boss;
        }

        return null;
    }

}

