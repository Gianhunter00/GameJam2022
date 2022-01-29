using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public static class EventMGR
{
    public static UnityEvent OnPlayerSwitch;
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
    }
}
