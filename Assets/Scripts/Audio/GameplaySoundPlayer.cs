using UnityEngine;

public class GameplaySoundPlayer : MonoBehaviour
{
    // Patrón Singleton: Permite llamar de forma global a los efectos de juego usando 'GameplaySoundPlayer.Instance'
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

    // Inicialización del Singleton para el sistema de juego actual
    private void Awake()
    {
        // LINEA RARA / IMPORTANTE: A diferencia del AudioManager anterior, aquí se usa 'Destroy(this)' en lugar de 'Destroy(gameObject)'.
        // Esto significa que si hay un duplicado, solo se destruirá este script específico (componente), pero NO borrará el objeto entero de la escena.
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

    // Evalúa qué tipo de Dios otorgó la bendición para reproducir su respectivo audio
    public void PlayBlessing(GodType god)
    {
        // LÍNEA RARA / COMPLEJA: Recibe un enumerador 'GodType' (una lista de opciones lógicas) y usa un 'switch' para evaluar el caso.
        // Dependiendo de qué dios sea el valor de la variable 'god', ejecutará el bloque de código correspondiente.
        switch (god)
        {
            case GodType.Aphrodite:
                Play(aphroditeBlessingClip);
                break; // El 'break' es obligatorio para salir de la evaluación una vez que encuentra la coincidencia

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

    // Método puente interno que centraliza la comunicación con el AudioManager global
    private void Play(AudioClip clip)
    {
        // LÍNEA RARA / COMPLEJA: Realiza un chequeo preventivo de seguridad ('!= null').
        // Si el AudioManager global no se ha cargado en la escena todavía, evita que el juego tire un error de tipo "NullReferenceException" en la consola.
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(clip); // Redirige el clip al canal de SFX (efectos de sonido)
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

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como una "Librería de Acceso Rápido" o Fachada (Facade Pattern) dedicada exclusivamente 
   a los efectos de sonido (SFX) que ocurren durante las mecánicas activas del juego (combate, construcción, oleadas, economía).

   Características clave:
   1. Interfaz Limpia para el Programador: En lugar de que los scripts de los enemigos o las torres tengan que buscar 
      e identificar clips de audio específicos, simplemente llaman a métodos lógicos y directos como 'GameplaySoundPlayer.Instance.PlayArrowHit()'.
   2. Desacoplamiento de Datos: Almacena de manera ordenada en el inspector de Unity todas las referencias a los archivos 
      de sonido comprimidos (.mp3, .wav), agrupados por categorías visuales gracias al atributo '[Header]'.
   3. Conexión Modular: No reproduce los sonidos por sí mismo; en su lugar, valida la existencia del 'AudioManager' principal 
      y le delega la responsabilidad de la reproducción física a través del canal de efectos de sonido. Esto mantiene el código organizado y modular.
   ========================================================================================================
*/