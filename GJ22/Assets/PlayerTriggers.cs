using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggers : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "PlayerSwitch")
        {
            collision.enabled = false;
            EventMGR.OnPlayerSwitch.Invoke();
            EventMGR.OnPlayerCheckPoint.Invoke(this.transform, collision.transform);
        }
        else if (collision.gameObject.tag == "Message")
        {
            collision.enabled = false;
            
            EventMGR.OnMessage.Invoke(collision.transform);
        }
        else if (collision.gameObject.tag == "MessageWithFocus")
        {
            collision.enabled = false;
            EventMGR.OnMessageWithFocus.Invoke(collision.transform);
            EventMGR.OnMessage.Invoke(collision.transform);
        }
    }

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            transform.parent = collision.transform;
        }
        else if (collision.gameObject.tag == "DeathZones")
        {
            EventMGR.OnPlayerDeath.Invoke(this.transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            transform.parent = null;
        }
    }
}
