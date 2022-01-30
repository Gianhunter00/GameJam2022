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
    public AnimationCurve DoubleFadeCurve;
    public float FadeDuration = 2f;
    public float DoubleFadeDuration = 4f;
    public float CameraSpeed = 2;
    public float TopPanning = 5.7f;
    public float BotPanning = 5.5f;
    public float RightLeftPanning = 10.5f;
    public float DoubleFadeOutWaitDuration; 
    [SerializeField]
    public LayerMask Mask;

    private GameObject target;
    private int index;
    private bool startFading;
    private float alpha;
    private Texture2D fadeOutTexture;

    private float timer;
    private float normalizedTime;
    private bool fadedOut;
    private bool doubleFadeOut;
    private GameObject prevTarget;
    private int doubleFadeIndex = 0;
    private Transform eventTransform;
    private ContactFilter2D filter;
    private RaycastHit2D hitInfo;
    private List<RaycastHit2D> raycastHitPoints;
    private bool WaitingDoubleFadeOut;
    private bool isFaiding;
    private bool playerDead;

    private void OnEnable()
    {
        EventMGR.OnPlayerSwitch?.AddListener(() => startFading = true);
        EventMGR.OnTriggeredEvent?.AddListener(OnDoubleFadeOut);
        EventMGR.OnMessageWithFocus?.AddListener(OnSwitchFocus);
        EventMGR.OnEndMessage.AddListener(() => target = prevTarget);
        EventMGR.OnPlayerDeath.AddListener((x) => playerDead = true);
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
        isFaiding = false;

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
            FadeOutWithSwitch();
        }
        if (doubleFadeOut)
        {
            DoubleFadeOut(eventTransform);
        }
        if (playerDead)
        {
            PlayerDeathFadeOut();
        }
    }
    private void FixedUpdate()
    {
            Vector3 finalTargetPos = target.transform.position + offSet;

            #region FirstBorderdHIt

            //if (Physics2D.Raycast(target.transform.position, new Vector3(0, -1, 0), filter, raycastHitPoints, 5f) > 0
            //    && raycastHitPoints[0].transform.gameObject.layer == 8)
            //{
            //    finalTargetPos.y = raycastHitPoints[0].point.y + BotPanning;
            //}
            //if (Physics2D.Raycast(target.transform.position, new Vector3(0, 1, 0), filter, raycastHitPoints, 5.5f) > 0
            //    && raycastHitPoints[0].transform.gameObject.layer == 8)
            //{
            //    finalTargetPos.y = raycastHitPoints[0].point.y - TopPanning;
            //}
            //if (Physics2D.Raycast(target.transform.position, new Vector3(-1, 0, 0), filter, raycastHitPoints, 10.5f) > 0
            //    && raycastHitPoints[0].transform.gameObject.layer == 8)
            //{
            //    finalTargetPos.x = raycastHitPoints[0].point.x + RightLeftPanning;

            //}
            //if (Physics2D.Raycast(target.transform.position, new Vector3(1, 0, 0), filter, raycastHitPoints, 10.5f) > 0
            //   && raycastHitPoints[0].transform.gameObject.layer == 8)
            //{
            //    finalTargetPos.x = raycastHitPoints[0].point.x - RightLeftPanning;

            //} 
            #endregion
            if (hitInfo = Physics2D.Raycast(target.transform.position, new Vector3(-1, 0, 0), 10.5f, Mask))
            {
                finalTargetPos.x = hitInfo.point.x + RightLeftPanning;
            }
            if (hitInfo = Physics2D.Raycast(target.transform.position, new Vector3(1, 0, 0), 10.5f, Mask))
            {
                finalTargetPos.x = hitInfo.point.x - RightLeftPanning;
            }
            if (hitInfo = Physics2D.Raycast(target.transform.position, new Vector3(0, -1, 0), 5f, Mask))
            {
                finalTargetPos.y = hitInfo.point.y + BotPanning;
            }
            if (hitInfo = Physics2D.Raycast(target.transform.position, new Vector3(0, 1, 0), 5.5f, Mask))
            {
                finalTargetPos.y = hitInfo.point.y - TopPanning;
            }
            transform.position = Vector3.Lerp(transform.position, finalTargetPos, Time.deltaTime * CameraSpeed);
        
        
    }

    private void DoubleFadeOut(Transform transform)
    {

        alpha = DoubleFadeCurve.Evaluate(normalizedTime);
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
                EventMGR.OnEndFocusedTriggeredEvent.Invoke();
                return;
            }
            else
            {
                WaitingDoubleFadeOut = true;
            }

            doubleFadeIndex++;
        }


        if (WaitingDoubleFadeOut)
        {
            timer += Time.deltaTime;
            if (timer >= DoubleFadeOutWaitDuration)
            {
                timer = 0;
                WaitingDoubleFadeOut = false;
            }
        }

        if (!WaitingDoubleFadeOut)
            normalizedTime += Time.deltaTime / DoubleFadeDuration;
    }

    private void PlayerDeathFadeOut()
    {
        alpha = (FadeCurve.Evaluate(normalizedTime));
        //Debug.Log(alpha);
        fadeOutTexture.SetPixel(0, 0, new Color(0, 0, 0, alpha));
        fadeOutTexture.Apply();
        if (normalizedTime >= 0.5f && !fadedOut)
        {
            fadedOut = true;
            this.transform.position = target.transform.position;
        }
        if (alpha < 0)
        {
            normalizedTime = 0;
            playerDead = false;
            fadedOut = false;
            alpha = 0;
            return;
        }
        normalizedTime += Time.deltaTime / FadeDuration;
    }

    private void FadeOutWithSwitch()
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
