using UnityEngine;

// Central controller for major screen/layout visibility
public class UIScreenFlowController : MonoBehaviour
{
    [Header("Screen Roots")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject mainMenuContent;
    [SerializeField] private GameObject gameplayHUDRoot;
    [SerializeField] private GameObject pauseMenuContent;
    [SerializeField] private GameObject victoryPanelContent;
    [SerializeField] private GameObject defeatPanelContent;
    [SerializeField] private GameObject blessingSelectionContent;
    [SerializeField] private GameObject settingsMenuContent;
    [SerializeField] private GameObject controlsMenuContent;

    public void ShowMainMenu()
    {
        SetMainMenuVisible(true);
        SetGameplayHUDVisible(false);
        SetPauseVisible(false);
        SetVictoryVisible(false);
        SetDefeatVisible(false);
        SetBlessingVisible(false);
        SetSettingsVisible(false);
        SetControlsVisible(false);
    }

    public void ShowGameplay()
    {
        SetMainMenuVisible(false);
        SetGameplayHUDVisible(true);
        SetPauseVisible(false);
        SetVictoryVisible(false);
        SetDefeatVisible(false);
        SetBlessingVisible(false);
        SetSettingsVisible(false);
        SetControlsVisible(false);
    }

    public void ShowPause()
    {
        SetMainMenuVisible(false);
        SetGameplayHUDVisible(true);
        SetPauseVisible(true);
        SetVictoryVisible(false);
        SetDefeatVisible(false);
        SetBlessingVisible(false);
        SetSettingsVisible(false);
        SetControlsVisible(false);
    }

    public void HidePause()
    {
        SetPauseVisible(false);
        SetSettingsVisible(false);
        SetControlsVisible(false);
    }

    public void ShowVictory()
    {
        SetVictoryVisible(true);
        SetDefeatVisible(false);
        SetBlessingVisible(false);
        SetPauseVisible(false);
        SetSettingsVisible(false);
        SetControlsVisible(false);
    }

    public void ShowDefeat()
    {
        SetDefeatVisible(true);
        SetVictoryVisible(false);
        SetBlessingVisible(false);
        SetPauseVisible(false);
        SetSettingsVisible(false);
        SetControlsVisible(false);
    }

    public void ShowBlessingSelection()
    {
        SetBlessingVisible(true);
        SetVictoryVisible(false);
        SetDefeatVisible(false);
        SetPauseVisible(false);
        SetSettingsVisible(false);
        SetControlsVisible(false);
    }

    public void HideBlessingSelection()
    {
        SetBlessingVisible(false);
    }

    public void ShowSettingsFromMainMenu()
    {
        SetMainMenuVisible(false);
        SetSettingsVisible(true);
        SetControlsVisible(false);
        SetPauseVisible(false);
    }

    public void ShowControlsFromMainMenu()
    {
        SetMainMenuVisible(false);
        SetSettingsVisible(false);
        SetControlsVisible(true);
        SetPauseVisible(false);
    }

    public void ShowSettingsFromPause()
    {
        SetPauseVisible(false);
        SetSettingsVisible(true);
        SetControlsVisible(false);
    }

    public void ShowControlsFromPause()
    {
        SetPauseVisible(false);
        SetSettingsVisible(false);
        SetControlsVisible(true);
    }

    public void BackToPauseScreen()
    {
        SetPauseVisible(true);
        SetSettingsVisible(false);
        SetControlsVisible(false);
    }

    public void BackToMainMenuScreen()
    {
        SetSettingsVisible(false);
        SetControlsVisible(false);
        SetPauseVisible(false);
        SetMainMenuVisible(true);
    }

    private void SetMainMenuVisible(bool isVisible)
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(isVisible);
        }

        if (mainMenuContent != null)
        {
            mainMenuContent.SetActive(isVisible);
        }
    }

    private void SetGameplayHUDVisible(bool isVisible)
    {
        if (gameplayHUDRoot != null)
        {
            gameplayHUDRoot.SetActive(isVisible);
        }
    }

    private void SetPauseVisible(bool isVisible)
    {
        if (pauseMenuContent != null)
        {
            pauseMenuContent.SetActive(isVisible);
        }
    }

    private void SetVictoryVisible(bool isVisible)
    {
        if (victoryPanelContent != null)
        {
            victoryPanelContent.SetActive(isVisible);
        }
    }

    private void SetDefeatVisible(bool isVisible)
    {
        if (defeatPanelContent != null)
        {
            defeatPanelContent.SetActive(isVisible);
        }
    }

    private void SetBlessingVisible(bool isVisible)
    {
        if (blessingSelectionContent != null)
        {
            blessingSelectionContent.SetActive(isVisible);
        }
    }

    private void SetSettingsVisible(bool isVisible)
    {
        if (settingsMenuContent != null)
        {
            settingsMenuContent.SetActive(isVisible);
        }
    }

    private void SetControlsVisible(bool isVisible)
    {
        if (controlsMenuContent != null)
        {
            controlsMenuContent.SetActive(isVisible);
        }
    }
}