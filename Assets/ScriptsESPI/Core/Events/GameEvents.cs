using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class GameEvents : MonoBehaviour
{
    private Dictionary<string, GameEvent> eventDictionary;

    private static GameEvents gameEvents;

    public static GameEvents Instance
    {
        get
        {
            if (!gameEvents)
            {
                gameEvents = FindAnyObjectByType<GameEvents>();

                if (!gameEvents)
                {
                    GameObject newObject = new GameObject("GameEvents");
                    gameEvents = newObject.AddComponent<GameEvents>();
                    gameEvents.Initialize();
                }
                else
                {
                    gameEvents.Initialize();
                }
            }

            return gameEvents;
        }
    }

    // Create the dictionary if it does not exist yet
    private void Initialize()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, GameEvent>();
        }
    }

    // Add a listener to an event name
    public static void StartListening(string eventName, UnityAction<object> listener)
    {
        if (Instance.eventDictionary.TryGetValue(eventName, out GameEvent thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new GameEvent();
            thisEvent.AddListener(listener);
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    // Remove a listener from an event name
    public static void StopListening(string eventName, UnityAction<object> listener)
    {
        if (gameEvents == null)
        {
            return;
        }

        if (Instance.eventDictionary.TryGetValue(eventName, out GameEvent thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    // Trigger an event and send optional data
    public static void TriggerEvent(string eventName, object eventData = null)
    {
        if (Instance.eventDictionary.TryGetValue(eventName, out GameEvent thisEvent))
        {
            thisEvent.Invoke(eventData);
        }
    }
}