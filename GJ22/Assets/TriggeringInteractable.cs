using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class TriggeringInteractable : MonoBehaviour
{
    public KeyCode InteractKey = KeyCode.E;
    public UnityEvent TriggeredEvent;
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Input.GetKeyDown(InteractKey))
        {
            TriggeredEvent?.Invoke();
        }
    }
}
