using UnityEngine;

public class GameplaySoundPlayer : MonoBehaviour
{
    public static GameplaySoundPlayer Instance;

    [Header("Construction")]
    [SerializeField] private AudioClip buildStructureClip;
    [SerializeField] private AudioClip buildWallClip;
    [SerializeField] private AudioClip invalidPlacementClip;
    [SerializeField] private AudioClip sellClip;

    [Header("Wall")]
    [SerializeField] private AudioClip wallRepairClip;
    [SerializeField] private AudioClip wallDestroyedClip;

    [Header("Upgrades")]
    [SerializeField] private AudioClip upgradeClip;
    [SerializeField] private AudioClip blessingOpenClip;
    [SerializeField] private AudioClip aphroditeBlessingClip;
    [SerializeField] private AudioClip aresBlessingClip;
    [SerializeField] private AudioClip hephaestusBlessingClip;

    [Header("Waves")]
    [SerializeField] private AudioClip waveStartClip;
    [SerializeField] private AudioClip milestoneWaveClip;
    [SerializeField] private AudioClip finalWaveClip;

    [Header("End Game")]
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private AudioClip defeatClip;

    [Header("Combat")]
    [SerializeField] private AudioClip arrowShotClip;
    [SerializeField] private AudioClip arrowHitClip;
    [SerializeField] private AudioClip ballistaShotClip;
    [SerializeField] private AudioClip ballistaHitClip;
    [SerializeField] private AudioClip soldierSpawnClip;
    [SerializeField] private AudioClip soldierAttackClip;
    [SerializeField] private AudioClip soldierDeathClip;
    [SerializeField] private AudioClip enemyHitClip;
    [SerializeField] private AudioClip enemyDeathClip;
    [SerializeField] private AudioClip templeHitClip;

    [Header("Economy")]
    [SerializeField] private AudioClip willGeneratedClip;


    [Header("Boss & Spells")]
    [SerializeField] private AudioClip fireBallClip;
    [SerializeField] private AudioClip bossDeathClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void PlayWillGenerated()
    {
        Play(willGeneratedClip);
    }

    public void PlayBuildStructure()
    {
        Play(buildStructureClip);
    }

    public void PlayBuildWall()
    {
        Play(buildWallClip);
    }

    public void PlayInvalidPlacement()
    {
        Play(invalidPlacementClip);
    }

    public void PlaySell()
    {
        Play(sellClip);
    }

    public void PlayWallRepair()
    {
        Play(wallRepairClip);
    }

    public void PlayWallDestroyed()
    {
        Play(wallDestroyedClip);
    }

    public void PlayUpgrade()
    {
        Play(upgradeClip);
    }

    public void PlayBlessingOpen()
    {
        Play(blessingOpenClip);
    }

    public void PlayBlessing(GodType god)
    {
        switch (god)
        {
            case GodType.Aphrodite:
                Play(aphroditeBlessingClip);
                break;

            case GodType.Ares:
                Play(aresBlessingClip);
                break;

            case GodType.Hephaestus:
                Play(hephaestusBlessingClip);
                break;
        }
    }

    public void PlayWaveStart()
    {
        Play(waveStartClip);
    }

    public void PlayMilestoneWave()
    {
        Play(milestoneWaveClip);
    }

    public void PlayFinalWave()
    {
        Play(finalWaveClip);
    }

    public void PlayVictory()
    {
        Play(victoryClip);
    }

    public void PlayDefeat()
    {
        Play(defeatClip);
    }

    private void Play(AudioClip clip)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(clip);
    }

    public void PlayArrowShot()
    {
        Play(arrowShotClip);
    }

    public void PlayArrowHit()
    {
        Play(arrowHitClip);
    }

    public void PlayBallistaShot()
    {
        Play(ballistaShotClip);
    }

    public void PlayBallistaHit()
    {
        Play(ballistaHitClip);
    }

    public void PlaySoldierSpawn()
    {
        Play(soldierSpawnClip);
    }

    public void PlaySoldierAttack()
    {
        Play(soldierAttackClip);
    }

    public void PlaySoldierDeath()
    {
        Play(soldierDeathClip);
    }

    public void PlayEnemyHit()
    {
        Play(enemyHitClip);
    }

    public void PlayEnemyDeath()
    {
        Play(enemyDeathClip);
    }

    public void PlayTempleHit()
    {
        Play(templeHitClip);
    }

    public void PlayFire()
    {
        Play(fireBallClip);
    }

    public void PlayBossDeath()
    {
        Play(bossDeathClip);
    }
}