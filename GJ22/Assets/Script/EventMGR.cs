using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public static class EventMGR
{
    public static UnityEvent OnPlayerSwitch;
    public static UnityEvent<Transform> OnPlayerDeath;
    public static UnityEvent<Transform,Transform> OnPlayerCheckPoint;
    public static UnityEvent<Transform> OnFocusedTriggeredEvent;
    public static UnityEvent OnEndFocusedTriggeredEvent;
    public static UnityEvent<Transform> OnTriggeredEvent;
    public static UnityEvent<Transform> OnMessage;
    public static UnityEvent<Transform> OnMessageWithFocus;
    public static UnityEvent OnEndMessage;

    static EventMGR()
    {
        OnPlayerSwitch = new UnityEvent();
        OnTriggeredEvent = new UnityEvent<Transform>();
        OnMessage = new UnityEvent<Transform>();
        OnMessageWithFocus = new UnityEvent<Transform>();
        OnEndMessage = new UnityEvent();
        OnFocusedTriggeredEvent = new UnityEvent<Transform>();
        OnEndFocusedTriggeredEvent = new UnityEvent();
        OnPlayerDeath = new UnityEvent<Transform>();
        OnPlayerCheckPoint = new UnityEvent<Transform, Transform>();
    }
}
