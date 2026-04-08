//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//// Handles visual cooldown feedback inside a button
//public class CooldownButtonUI : MonoBehaviour
//{
//    [Header("Cooldown Settings")]
//    [SerializeField] private CooldownType cooldownType;

//    [Header("References")]
//    [SerializeField] private CooldownSystem cooldownSystem;
//    [SerializeField] private Image cooldownOverlayImage;
//    [SerializeField] private TMP_Text cooldownValueText;

//    private void Update()
//    {
//        RefreshCooldownVisual();
//    }

//    // Update the visual cooldown overlay and optional text
//    private void RefreshCooldownVisual()
//    {
//        if (cooldownSystem == null)
//        {
//            return;
//        }

//        bool isOnCooldown = cooldownSystem.IsOnCooldown(cooldownType);
//        float progress = cooldownSystem.GetCooldownProgress(cooldownType);
//        float remainingTime = cooldownSystem.GetRemainingCooldown(cooldownType);

//        if (cooldownOverlayImage != null)
//        {
//            cooldownOverlayImage.gameObject.SetActive(isOnCooldown);
//            cooldownOverlayImage.fillAmount = progress;
//        }

//        if (cooldownValueText != null)
//        {
//            if (isOnCooldown)
//            {
//                cooldownValueText.gameObject.SetActive(true);
//                cooldownValueText.text = remainingTime.ToString("F1");
//            }
//            else
//            {
//                cooldownValueText.gameObject.SetActive(false);
//                cooldownValueText.text = "";
//            }
//        }
//    }
//}


using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Handles visual cooldown feedback inside a button
public class CooldownButtonUI : MonoBehaviour
{
    [Header("Cooldown Settings")]
    [SerializeField] private CooldownType cooldownType;

    [Header("References")]
    [SerializeField] private CooldownSystem cooldownSystem;
    [SerializeField] private Image cooldownOverlayImage;
    [SerializeField] private TMP_Text cooldownValueText;

    [Header("Refresh Settings")]
    [SerializeField] private float refreshInterval = 0.05f;

    private Coroutine refreshRoutine;

    private void OnEnable()
    {
        StartRefreshRoutine();
    }

    private void OnDisable()
    {
        StopRefreshRoutine();
    }

    private void Start()
    {
        RefreshCooldownVisual();
    }

    private void StartRefreshRoutine()
    {
        StopRefreshRoutine();
        refreshRoutine = StartCoroutine(RefreshLoop());
    }

    private void StopRefreshRoutine()
    {
        if (refreshRoutine != null)
        {
            StopCoroutine(refreshRoutine);
            refreshRoutine = null;
        }
    }

    private IEnumerator RefreshLoop()
    {
        while (true)
        {
            RefreshCooldownVisual();
            yield return new WaitForSecondsRealtime(refreshInterval);
        }
    }

    // Update the visual cooldown overlay and optional text
    private void RefreshCooldownVisual()
    {
        if (cooldownSystem == null)
        {
            return;
        }

        bool isOnCooldown = cooldownSystem.IsOnCooldown(cooldownType);
        float progress = cooldownSystem.GetCooldownProgress(cooldownType);
        float remainingTime = cooldownSystem.GetRemainingCooldown(cooldownType);

        if (cooldownOverlayImage != null)
        {
            cooldownOverlayImage.gameObject.SetActive(isOnCooldown);
            cooldownOverlayImage.fillAmount = progress;
        }

        if (cooldownValueText != null)
        {
            if (isOnCooldown)
            {
                cooldownValueText.gameObject.SetActive(true);
                cooldownValueText.text = remainingTime.ToString("F1");
            }
            else
            {
                cooldownValueText.gameObject.SetActive(false);
                cooldownValueText.text = "";
            }
        }
    }
}