using UnityEngine;

public class GameSceneAudioStarter : MonoBehaviour
{
    private void Start()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayConstructionMusic();
    }
}