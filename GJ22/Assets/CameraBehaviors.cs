using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviors : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> Players;
    public Vector3 offSet;
    public AnimationCurve FadeCurve;
    public float FadeDuration;
    
    
    private GameObject target;
    private int index;
    private bool startFadeOut;
    private float alpha;
    private Texture2D fadeOutTexture;
    private float multiplier;

    private float normalizedTime;
    private bool fadedOut;

    void Start()
    {
        fadedOut = false;
        index = 0;
        target = Players[0];
        target.GetComponent<PlayerMovement>().Controllable = true;
        fadeOutTexture = new Texture2D(1, 1);
        fadeOutTexture.SetPixel(0, 0, Color.black);
        fadeOutTexture.Apply();
        multiplier = 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.transform.position.x + offSet.x, target.transform.position.y + offSet.y, offSet.z);
        //Delegate
        if (Input.GetKeyDown(KeyCode.Alpha1) && !startFadeOut)
        {
            OnFadeOut();
        }


       

        if (startFadeOut)
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
                startFadeOut = false;
                fadedOut = false;
                alpha = 0;

            }
            normalizedTime += Time.deltaTime / FadeDuration;
        }


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
        target.GetComponent<PlayerMovement>().Controllable = false;
        target = Players[index];
        target.GetComponent<PlayerMovement>().Controllable = true;
    }

    public void OnFadeOut()
    {
        startFadeOut = true;
    }
}
