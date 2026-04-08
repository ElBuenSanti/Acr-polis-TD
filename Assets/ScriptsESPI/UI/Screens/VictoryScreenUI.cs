//using UnityEngine;
//using UnityEngine.EventSystems;

//// Handles the basic victory placeholder screen
//public class VictoryScreenUI : MonoBehaviour
//{
//    [Header("Content Root")]
//    [SerializeField] private GameObject contentRoot;

//    [Header("UI References")]
//    [SerializeField] private GameObject mainMenuPanel;
//    [SerializeField] private GameObject mainMenuContent;
//    [SerializeField] private GameObject gameplayHudRoot;
//    [SerializeField] private GameObject pauseMenuPanel;
//    [SerializeField] private GameObject settingsMenuPanel;
//    [SerializeField] private GameObject controlsMenuPanel;

//    private void OnEnable()
//    {
//        GameEvents.StartListening(EventNames.StateChanged, OnStateChanged);
//    }

//    private void OnDisable()
//    {
//        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);
//    }

//    private void Start()
//    {
//        RefreshVisibility();
//    }

//    private void OnStateChanged(object eventData)
//    {
//        RefreshVisibility();
//    }

//    private void RefreshVisibility()
//    {
//        if (contentRoot == null || GameStateManager.Instance == null)
//        {
//            return;
//        }

//        bool shouldShow = GameStateManager.Instance.CurrentState == GameState.Victory;
//        contentRoot.SetActive(shouldShow);
//    }

//    public void GoToMainMenu()
//    {
//        ShowMainMenuLayout();

//        if (GameStateManager.Instance != null)
//        {
//            GameStateManager.Instance.ChangeState(GameState.Preparation);
//        }

//        ClearSelectedUI();
//        Debug.Log("VictoryScreenUI: Returned to Main Menu placeholder.");
//    }

//    public void RestartRun()
//    {
//        HideAllOverlayMenus();

//        if (mainMenuContent != null)
//        {
//            mainMenuContent.SetActive(false);
//        }

//        if (mainMenuPanel != null)
//        {
//            mainMenuPanel.SetActive(false);
//        }

//        if (gameplayHudRoot != null)
//        {
//            gameplayHudRoot.SetActive(true);
//        }

//        if (GameStateManager.Instance != null)
//        {
//            GameStateManager.Instance.ChangeState(GameState.Preparation);
//        }

//        ClearSelectedUI();
//        Debug.Log("VictoryScreenUI: Restart placeholder.");
//    }

//    private void ShowMainMenuLayout()
//    {
//        if (contentRoot != null)
//        {
//            contentRoot.SetActive(false);
//        }

//        if (gameplayHudRoot != null)
//        {
//            gameplayHudRoot.SetActive(false);
//        }

//        HideAllOverlayMenus();

//        if (mainMenuPanel != null)
//        {
//            mainMenuPanel.SetActive(true);
//        }

//        if (mainMenuContent != null)
//        {
//            mainMenuContent.SetActive(true);
//        }
//    }

//    private void HideAllOverlayMenus()
//    {
//        if (pauseMenuPanel != null)
//        {
//            pauseMenuPanel.SetActive(false);
//        }

//        if (settingsMenuPanel != null)
//        {
//            settingsMenuPanel.SetActive(false);
//        }

//        if (controlsMenuPanel != null)
//        {
//            controlsMenuPanel.SetActive(false);
//        }
//    }

//    private void ClearSelectedUI()
//    {
//        if (EventSystem.current != null)
//        {
//            EventSystem.current.SetSelectedGameObject(null);
//        }
//    }
//}


using UnityEngine;
using UnityEngine.EventSystems;

// Handles the basic victory placeholder screen
public class VictoryScreenUI : MonoBehaviour
{
    [Header("Content Root")]
    [SerializeField] private GameObject contentRoot;

    [Header("Flow References")]
    [SerializeField] private UIScreenFlowController screenFlowController;

    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.StateChanged, OnStateChanged);
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);
    }

    private void Start()
    {
        RefreshVisibility();
    }

    private void OnStateChanged(object eventData)
    {
        RefreshVisibility();
    }

    private void RefreshVisibility()
    {
        if (contentRoot == null || GameStateManager.Instance == null)
        {
            return;
        }

        bool shouldShow = GameStateManager.Instance.CurrentState == GameState.Victory;
        contentRoot.SetActive(shouldShow);

        if (shouldShow && screenFlowController != null)
        {
            screenFlowController.ShowVictory();
        }
    }

    public void GoToMainMenu()
    {
        if (screenFlowController != null)
        {
            screenFlowController.ShowMainMenu();
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.Preparation);
        }

        ClearSelectedUI();
        Debug.Log("VictoryScreenUI: Returned to Main Menu.");
    }

    public void RestartRun()
    {
        if (screenFlowController != null)
        {
            screenFlowController.ShowGameplay();
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.Preparation);
        }

        ClearSelectedUI();
        Debug.Log("VictoryScreenUI: Restart placeholder.");
    }

    private void ClearSelectedUI()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}