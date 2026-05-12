using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EndGamePanelUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button firstSelectedButton;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float fadeSpeed = 8f;

    private bool isOpen;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        HideInstant();
    }

    private void Update()
    {
        float targetAlpha = isOpen ? 1f : 0f;

        canvasGroup.alpha = Mathf.Lerp(
            canvasGroup.alpha,
            targetAlpha,
            Time.unscaledDeltaTime * fadeSpeed
        );
    }

    public void Open()
    {
        Time.timeScale = 0f;
        isOpen = true;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        GameStateController.Instance.SetState(GameState.EndGame);

        if (firstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void HideInstant()
    {
        isOpen = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}