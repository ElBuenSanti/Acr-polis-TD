using UnityEngine;

public class GameStateDebugInput : MonoBehaviour
{
    private void Update()
    {
        if (GameStateManager.Instance == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeToState(GameState.Preparation);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeToState(GameState.BuildingPlacement);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeToState(GameState.StructureSelected);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeToState(GameState.RadialUpgradeOpen);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeToState(GameState.Combat);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChangeToState(GameState.Paused);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ChangeToState(GameState.BlessingSelection);
        }
    }

    // Change the game state for debug testing
    private void ChangeToState(GameState newState)
    {
        bool stateChanged = GameStateManager.Instance.ChangeState(newState);

        if (stateChanged)
        {
            Debug.Log("Debug input changed state to: " + newState);
        }
    }
}