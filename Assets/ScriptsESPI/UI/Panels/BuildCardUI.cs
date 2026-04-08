using UnityEngine;
using UnityEngine.UI;

// Handles a single build card button UI
public class BuildCardUI : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField] private BuildType buildType;

    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private BuildCardActionSystem buildCardActionSystem;

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnCardPressed);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnCardPressed);
        }
    }

    // Called when the build card button is pressed
    private void OnCardPressed()
    {
        if (buildCardActionSystem == null)
        {
            Debug.LogWarning("BuildCardUI: BuildCardActionSystem reference is missing.");
            return;
        }

        buildCardActionSystem.TryUseBuildCard(buildType);
    }
}