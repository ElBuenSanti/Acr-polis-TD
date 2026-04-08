using UnityEngine;

public class GameEventsDebugListener : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.StateChanged, OnStateChanged);
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);
    }

    // This method is called when the state changed event is triggered
    private void OnStateChanged(object eventData)
    {
        Debug.Log("GameEvents listener received: " + eventData);
    }
}