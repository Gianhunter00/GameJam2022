using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviors : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> Players;
    public Vector3 offSet;
    public AnimationCurve FadeCurve;
    public float FadeDuration = 2f;
    public float DoubleFadeDuration = 4f;
    public float CameraSpeed = 2;
    public float TopPanning = 5.7f;
    public float BotPanning = 5.5f;
    public float RightLeftPanning = 10.5f;
    [SerializeField]
    public LayerMask Mask;

    private GameObject target;
    private int index;
    private bool startFading;
    private float alpha;
    private Texture2D fadeOutTexture;
    private float multiplier;


    private float normalizedTime;
    private bool fadedOut;
    private bool doubleFadeOut;
    private GameObject prevTarget;
    private int doubleFadeIndex = 0;
    private Transform eventTransform;
    private ContactFilter2D filter;
    private List<RaycastHit2D> raycastHitPoints;
    private bool notFading;

    private void OnEnable()
    {
        EventMGR.OnPlayerSwitch?.AddListener(() => startFading = true);
        EventMGR.OnTriggeredEvent?.AddListener(OnDoubleFadeOut);

        EventMGR.OnMessageWithFocus?.AddListener(OnSwitchFocus);
        EventMGR.OnEndMessage.AddListener(() => target = prevTarget);
    }
    void Start()
    {

        raycastHitPoints = new List<RaycastHit2D>();
        filter.layerMask = Mask;
        filter.useLayerMask = true;
        fadedOut = false;
        index = 0;
        target = Players[0];
        target.GetComponent<PlayerMovement>().Controllable = true;
        fadeOutTexture = new Texture2D(1, 1);
        fadeOutTexture.SetPixel(0, 0, Color.black);
        fadeOutTexture.Apply();
        multiplier = 1;
        notFading = false;
    }

    public void OnSwitchFocus(Transform t)
    {
        prevTarget = target;
        target = t.GetChild(0).gameObject;
    }
    // Update is called once per frame
    void Update()
    {

        //Delegate

        if (startFading)
        {
            FadeOut();
        }
        if (doubleFadeOut)
        {
            DoubleFadeOut(eventTransform);
        }

    }
    private void FixedUpdate()
    {
        Vector3 finalTargetPos = target.transform.position + offSet;


        if (Physics2D.Raycast(target.transform.position, new Vector3(0, -1, 0), filter, raycastHitPoints, 5f) > 0
            && raycastHitPoints[0].transform.gameObject.layer == 8)
        {
            finalTargetPos.y = raycastHitPoints[0].point.y + BotPanning;
        }
        if (Physics2D.Raycast(target.transform.position, new Vector3(0, 1, 0), filter, raycastHitPoints, 5.5f) > 0
            && raycastHitPoints[0].transform.gameObject.layer == 8)
        {
            finalTargetPos.y = raycastHitPoints[0].point.y - TopPanning;
        }
        if (Physics2D.Raycast(target.transform.position, new Vector3(-1, 0, 0), filter, raycastHitPoints, 10.5f) > 0
            && raycastHitPoints[0].transform.gameObject.layer == 8)
        {
            finalTargetPos.x = raycastHitPoints[0].point.x + RightLeftPanning;

        }
        if (Physics2D.Raycast(target.transform.position, new Vector3(1, 0, 0), filter, raycastHitPoints, 10.5f) > 0
           && raycastHitPoints[0].transform.gameObject.layer == 8)
        {
            finalTargetPos.x = raycastHitPoints[0].point.x - RightLeftPanning;

        }


        transform.position = Vector3.Lerp(transform.position, finalTargetPos, Time.deltaTime * CameraSpeed);


        //if (hitInfo = Physics2D.Raycast(target.transform.position, new Vector3(-1, 0, 0), 10.5f, layerMaskBorders))
        //{
        //    finalTargetPos.x = hitInfo.point.x + RightLeftPanning;
        //}
        //if (hitInfo = Physics2D.Raycast(target.transform.position, new Vector3(1, 0, 0), 10.5f, layerMaskBorders))
        //{
        //    finalTargetPos.x = hitInfo.point.x - RightLeftPanning;
        //}
        //if (hitInfo = Physics2D.Raycast(target.transform.position, new Vector3(0, -1, 0), 5f, layerMask))
        //{
        //    finalTargetPos.y = hitInfo.point.y + BotPanning;
        //}
        //if (hitInfo = Physics2D.Raycast(target.transform.position, new Vector3(0, 1, 0), 5.5f, layerMaskBorders))
        //{
        //    finalTargetPos.y = hitInfo.point.y - TopPanning;
        //}
        //if (!Physics2D.Raycast(target.transform.position, new Vector3(-1, 0, 0), 10, layerMask) || !Physics2D.Raycast(target.transform.position, new Vector3(1, 0, 0), 10, layerMask)||  
        //    !Physics2D.Raycast(target.transform.position, new Vector3(0, 1, 0), 10, layerMask) || !Physics2D.Raycast(target.transform.position, new Vector3(0, -1, 0), 2, layerMask))  
        //{
        //    transform.position = Vector3.Lerp(transform.position, new Vector3(target.transform.position.x + offSet.x, target.transform.position.y + offSet.y, offSet.z), Time.deltaTime * CameraSpeed);
        //    Debug.Log("CIAO");
        //}
    }

    private void DoubleFadeOut(Transform transform)
    {

        alpha = (FadeCurve.Evaluate(normalizedTime));
        //Debug.Log(alpha);
        fadeOutTexture.SetPixel(0, 0, new Color(0, 0, 0, alpha));
        fadeOutTexture.Apply();
        if (normalizedTime >= 0.5f && !fadedOut)
        {
            fadedOut = true;
            target = transform.gameObject;
            this.transform.position = target.transform.position;
            if (doubleFadeIndex == 1)
            {
                target = prevTarget;
                this.transform.position = target.transform.position;
            }
        }
        if (alpha < 0)
        {
            normalizedTime = 0;
            fadedOut = false;
            alpha = 0;
            if (doubleFadeIndex == 1)
            {
                doubleFadeOut = false;
                return;
            }
            doubleFadeIndex++;
        }
        normalizedTime += Time.deltaTime / DoubleFadeDuration;
    }

    private void FadeOut()
    {
        alpha = (FadeCurve.Evaluate(normalizedTime));
        //Debug.Log(alpha);
        fadeOutTexture.SetPixel(0, 0, new Color(0, 0, 0, alpha));
        fadeOutTexture.Apply();
        if (normalizedTime >= 0.5f && !fadedOut)
        {
            fadedOut = true;
            OnPlayerSwitch();
        }
        if (alpha < 0)
        {
            normalizedTime = 0;
            startFading = false;
            fadedOut = false;
            alpha = 0;
            return;
        }
        normalizedTime += Time.deltaTime / FadeDuration;
    }

    private void OnGUI()
    {
        if (alpha > 0f)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }


    public void OnPlayerSwitch()
    {
        index++;
        if (index > 1)
            index = 0;
        target = Players[index];
        transform.position = target.transform.position;
    }


    public void OnDoubleFadeOut(Transform transform)
    {
        prevTarget = target;
        doubleFadeOut = true;
        eventTransform = transform;
        doubleFadeIndex = 0;

    }


}
