using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessaggingSystem : MonoBehaviour
{
    // Start is called before the first frame update

    private Transform currentMessage;
    private TextMeshProUGUI currentText;
    private bool showingMessage;
    private float timer;
    private bool disposeCurrentMessage;
    public float MessageDuration = 5;
    private void OnEnable()
    {

        EventMGR.OnMessage.AddListener(SendMessage);

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (showingMessage)
        {
            timer += Time.deltaTime;
            currentText.alpha += currentText.alpha >= 1 ? 0 : 0.02f;
            if (timer >= MessageDuration || (timer >= MessageDuration * 0.5f && Input.anyKey))
            {
                EventMGR.OnEndMessage.Invoke();
                showingMessage = false;
                timer = 0;
                disposeCurrentMessage = true;
            }
        }
        if (disposeCurrentMessage)
        {
            currentText.alpha -= 0.02f;
            if (currentText.alpha <= 0)
            {
                currentMessage.gameObject.SetActive(false);
                disposeCurrentMessage = false;
                currentText = null;
                currentMessage = null;
            }
        }
        
        
    }

    public void SendMessage(Transform t)
    {
        currentMessage = t.GetChild(0);
        currentText = currentMessage.gameObject.GetComponent<TextMeshProUGUI>();
        currentText.alpha = 0;
        currentMessage.gameObject.SetActive(true);
        showingMessage = true;
    }
}
