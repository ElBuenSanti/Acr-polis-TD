using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class EventManager : MonoBehaviour
{

    private Dictionary<string, TriggerAction> eventDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindAnyObjectByType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    eventManager = new GameObject("eventManager").AddComponent<EventManager>();
                    eventManager.Init();
                    //Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, TriggerAction>();
        }
    }

    public static void StartListening(string eventName, UnityAction<object> listener)
    {
        //Debug.Log("Listening " + listener.Method.Name);
        if (instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new TriggerAction();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<object> listener)
    {
        if (eventManager == null) return;
        if (instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName, object property = null)
    {
        if (instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent.Invoke(property);
        }
    }
}


public class TriggerAction : UnityEvent<object>
{
    
}