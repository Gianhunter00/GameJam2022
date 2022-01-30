using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
using UnityEngine;
using UnityEngine.Events;

public class TriggeringInteractable : MonoBehaviour
{
    public KeyCode InteractKey = KeyCode.E;
    public Sprite On;
    public UnityEvent TriggeredEvent;
    bool triggerable = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggerable = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        triggerable = false;
    }

    private void Update()
    {
        if (triggerable && Input.GetKeyDown(InteractKey))
        {
            GetComponent<SpriteRenderer>().sprite = On;
            TriggeredEvent?.Invoke();
#if UNITY_EDITOR
            while (TriggeredEvent.GetPersistentEventCount() > 0)
                UnityEventTools.RemovePersistentListener(TriggeredEvent, 0);
#endif
        }
    }
}
