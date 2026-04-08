using UnityEngine;
using UnityEngine.EventSystems;

// Handles the fullscreen blessing selection placeholder
public class BlessingSelectionUI : MonoBehaviour
{
    [Header("Content Root")]
    [SerializeField] private GameObject contentRoot;

    [Header("Flow References")]
    [SerializeField] private UIScreenFlowController screenFlowController;

    [Header("Return State")]
    [SerializeField] private GameState returnStateAfterSelection = GameState.Preparation;

    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.StateChanged, OnStateChanged);
        RefreshVisibility();
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);
    }

    private void Start()
    {
        if (contentRoot != null)
        {
            contentRoot.SetActive(false);
        }

        RefreshVisibility();
    }

    private void OnStateChanged(object eventData)
    {
        if (!(eventData is GameState))
        {
            return;
        }

        RefreshVisibility();
    }

    private void RefreshVisibility()
    {
        if (contentRoot == null)
        {
            Debug.LogWarning("BlessingSelectionUI: Content Root is missing.");
            return;
        }

        if (GameStateManager.Instance == null)
        {
            return;
        }

        bool shouldShow = GameStateManager.Instance.CurrentState == GameState.BlessingSelection;

        contentRoot.SetActive(shouldShow);

        if (shouldShow)
        {
            if (screenFlowController != null)
            {
                screenFlowController.ShowBlessingSelection();
            }

            SelectDefaultIfPossible();
            Debug.Log("BlessingSelectionUI: BlessingSelection content shown.");
        }
        else
        {
            Debug.Log("BlessingSelectionUI: BlessingSelection content hidden.");
        }
    }

    public void SelectBlessingA()
    {
        ConfirmBlessing("Blessing A");
    }

    public void SelectBlessingB()
    {
        ConfirmBlessing("Blessing B");
    }

    public void SelectBlessingC()
    {
        ConfirmBlessing("Blessing C");
    }

    public void CancelBlessingSelection()
    {
        if (screenFlowController != null)
        {
            screenFlowController.HideBlessingSelection();
            screenFlowController.ShowGameplay();
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(returnStateAfterSelection);
        }

        ClearSelectedUI();
        Debug.Log("Blessing selection cancelled.");
    }

    private void ConfirmBlessing(string blessingName)
    {
        Debug.Log("Blessing selected: " + blessingName);

        if (screenFlowController != null)
        {
            screenFlowController.HideBlessingSelection();
            screenFlowController.ShowGameplay();
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(returnStateAfterSelection);
        }

        ClearSelectedUI();
    }

    private void SelectDefaultIfPossible()
    {
        if (contentRoot == null)
        {
            return;
        }

        MenuNavigationUI navigationUI = contentRoot.GetComponent<MenuNavigationUI>();

        if (navigationUI != null)
        {
            navigationUI.SelectDefault();
        }
    }

    private void ClearSelectedUI()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}