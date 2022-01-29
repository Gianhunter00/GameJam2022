using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.IMGUI;
using UnityEngine.Events;

public class TriggeredEventType : MonoBehaviour
{
    Animator MyAnim;
    Collider2D MyCollider;
    CompositeCollider2D MyCompositeCollider;
    public GameObject BridgePrefab;
    public List<Sprite> BridgeSpritesAfterFirst;
    public float Speed = 1;
    public bool ClockWise = true;
    public List<Vector3> Edges = new List<Vector3>();
    public bool MovingPlatform = false;
    Vector3 startingPos;
    Transform parent;
    Vector3 offset;
    private int index = 0;
    private bool elevatorOn;
    private bool regenerateCollider;
    private CameraBehaviors mainCamera;
    private void Start()
    {
        mainCamera = Camera.main.GetComponent<CameraBehaviors>();
        MyCompositeCollider = GetComponent<CompositeCollider2D>();
        MyAnim = GetComponent<Animator>();
        MyCollider = GetComponent<Collider2D>();
        if (!ClockWise)
        {
            Edges.Reverse();
        }
        offset = Vector3.zero;
        parent = transform.parent;
        if (parent != null)
            startingPos = parent.position;
        else
            startingPos = Vector3.zero;
    }
    public void DoorEvent()
    {
        MyAnim?.SetTrigger("Open");
        if (MyCollider != null)
        {
            MyCollider.isTrigger = true;
            MyCollider.enabled = true;
            EventMGR.OnTriggeredEvent?.Invoke(transform);
        }

    }

    public void ElevatorEvent()
    {
        elevatorOn = true;
    }

    public void BridgeEvent()
    {
        gameObject.SetActive(true);
        if (BridgeSpritesAfterFirst.Count == 0)
            return;
        GameObject current;
        for (int i = 0; i < BridgeSpritesAfterFirst.Count; i++)
        {
            if (BridgeSpritesAfterFirst[i] == null)
                return;
            current = Instantiate(BridgePrefab, transform);
            current.GetComponent<SpriteRenderer>().sprite = BridgeSpritesAfterFirst[i];
            current.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
            current.transform.position = new Vector2(transform.position.x + ((i + 1)), transform.position.y);

        }
        EventMGR.OnTriggeredEvent?.Invoke(transform);
        regenerateCollider = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("CIAO");
    }


    private void Update()
    {
        if (regenerateCollider)
        {
            MyCompositeCollider.GenerateGeometry();
            regenerateCollider = false;
        }
        if (elevatorOn || MovingPlatform)
        {
            if (Edges.Count > 1)
            {
                if (parent != null && Vector3.Distance(startingPos, parent.position) > 0.0001f)
                {
                    offset = parent.position - startingPos;
                    for (int i = 0; i < Edges.Count; i++)
                    {
                        Edges[i] += offset;
                    }
                    startingPos = parent.position;
                }
                float distance = Vector3.Distance(transform.position, Edges[index]);
                if (distance > 0.01f)
                    transform.position = Vector3.MoveTowards(transform.position, Edges[index], Time.deltaTime * Speed);
                else
                    index = (index + 1) % Edges.Count;
            }
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(TriggeredEventType)), CanEditMultipleObjects]
public class FreeMoveHandleMoveAroundPerimeter : Editor
{
    List<Vector3> Edges;
    Vector3 parent;
    TriggeredEventType instance;

    private void OnEnable()
    {
        instance = (TriggeredEventType)target;
        Edges = instance.Edges;
    }
    protected virtual void OnSceneGUI()
    {
        if (Edges.Count == 0) return;
        float size = HandleUtility.GetHandleSize(Edges[0]) * 0.2f;
        Vector3 snap = Vector3.one * 0.5f;

        EditorGUI.BeginChangeCheck();
        for (int i = 0; i < Edges.Count; i++)
        {
            Vector3 newTargetPosition = Handles.FreeMoveHandle(Edges[i], Quaternion.identity, size, snap, Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Edges[i] = newTargetPosition;
            }
        }
        if (Edges.Count > 1)
        {
            Handles.DrawAAPolyLine(5, Edges.ToArray());
            Handles.DrawLine(Edges[0], Edges[Edges.Count - 1], 2);
        }
    }

    private void OnValidate()
    {
        Edges = instance.Edges;
    }
}
#endif
